using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Management;

public class Manager : MonoBehaviour
{
    public State currentState;

    [SerializeField]
    private GameObject cardPrefab;
    public List<GameObject> cards = new List<GameObject>();
    [SerializeField]
    private GameObject enemyCardPrefab;
    public List<GameObject> enemyCards = new List<GameObject>();

    [SerializeField]
    private GameObject placeHolder;
    [SerializeField]
    private GameObject handPlaceHolder;
    [SerializeField]
    private GameObject enemyPlaceHolder;
    [SerializeField]
    private GameObject enemyHandPlaceHolder;

    [SerializeField]
    private GameObject[] cardHolders;
    [SerializeField]
    public GameObject[] myCardHolders;

    [SerializeField]
    private Text resourseText;
    [SerializeField]
    private Text dropCostSumText;
    [SerializeField]
    public Button mainButton;
    [SerializeField]
    public Text mainBtnText;
    [SerializeField]
    private Text handCapacity;


    private void FixedUpdate()
    {
        currentState.startActions();
        handCapacity.text = "Hand capacity\n" + handPlaceHolder.gameObject.transform.childCount.ToString()+"/3";
        resourseText.text = "Resources: " + Driver.instance.resoureCount.ToString();
        dropCostSumText.text = "DropCostSum: " + Driver.instance.dropCostSum.ToString();
    }

    public void mainBtn()
    {
        switch (Driver.instance.phase)
        {
            case Phase.DRAW:
                spawnCards();
                mainBtnText.text = "DRAWING...";
                break;
            case Phase.PLACEMENT:
                if(Driver.instance.myMana.CanAfford(Driver.instance.dropCostSum)){ 
                    mainBtnText.text = "COMBAT!";
                    mainButton.gameObject.GetComponent<Image>().color = Color.green;
                    Driver.instance.dropCostSum = 0;
                    flipCards();
                    applyEnemyDeltas();
                    List<PlayUnitCardAction> actions = new List<PlayUnitCardAction>();
                    for (int i = 0; i < myCardHolders.Length; i++)
                    {
                        if(myCardHolders[i].transform.childCount>0)
                        {
                            CardUI c = myCardHolders[i].transform.GetChild(0).GetComponent<CardUI>();
                            if (c.Old == false) {
                                Card back = c.cardBackEnd;
                                if (myCardHolders[i].transform.parent.name.Contains("Front"))
                                    actions.Add(new PlayUnitCardAction(back as UnitCard, Driver.instance.myLanes[i % 3], 0, 0));
                                else
                                    actions.Add(new PlayUnitCardAction(back as UnitCard, Driver.instance.myLanes[i % 3], 0, 1));
                                //myCardHolders need to be in correct order
                            }
                        }
                    }
                    foreach(PlayUnitCardAction action in actions)
                    {
                        foreach (Delta del in action.GetDeltas(Driver.instance.gameManager.Players[0]))
                            del.Apply();
                    }
                    placeAll();
                    Driver.instance.phase = Phase.COMBAT;
                }
                else
                {
                    mainBtnText.text = "Cant Afford!";
                    mainButton.gameObject.GetComponent<Image>().color = Color.red;
                    StartCoroutine(lerpColor(mainButton.gameObject));
                }
                break;
            case Phase.COMBAT:
                break;
            case Phase.DONE:
                mainBtnText.text = "DONE";
                mainButton.GetComponent<Button>().enabled = false;
                break;

        }
    }

    public IEnumerator damageAnims()
    {
        yield return new WaitForSeconds(0.5f);
        mainBtnText.text = "Combat Done!";
        yield return new WaitForSeconds(0.5f);
        mainBtnText.text = "DRAW";
    }

    public void applyEnemyDeltas()
    {
        List<PlayUnitCardAction> actions = new List<PlayUnitCardAction>();
        for (int i = 0; i < cardHolders.Length; i++)
        {
            if (cardHolders[i].transform.childCount > 0)
            {
                CardUI c = cardHolders[i].transform.GetChild(0).GetComponent<CardUI>();
                if (c.Old == false)
                {
                    Card back = c.cardBackEnd;
                    if (cardHolders[i].transform.parent.name.Contains("Front"))
                        actions.Add(new PlayUnitCardAction(back as UnitCard, Driver.instance.myLanes[i % 3], 1, 0));
                    else
                        actions.Add(new PlayUnitCardAction(back as UnitCard, Driver.instance.myLanes[i % 3], 1, 1));
                }
                c.Old = true;
            }
        }
        foreach (PlayUnitCardAction action in actions)
        {
            foreach (Delta del in action.GetDeltas(Driver.instance.gameManager.Players[1]))
                del.Apply();
        }
    }

    public void spawnCards()
    {
        while (handPlaceHolder.gameObject.transform.childCount + cards.Count < 3)
        {
            GameObject tempCard = Instantiate(cardPrefab, placeHolder.transform);
            cards.Add(tempCard);
        }
        while (enemyHandPlaceHolder.gameObject.transform.childCount + enemyCards.Count < 3)
        {
            GameObject tempCard2 = Instantiate(enemyCardPrefab, enemyPlaceHolder.transform);
            enemyCards.Add(tempCard2);
        }
        Driver.instance.drawCards();
        StartCoroutine(moveToHand());
    }

    IEnumerator moveToHand()
    {
        while (cards.Count>0 || enemyCards.Count>0)
        {
            float timeOfTravel = 0.25f;
            float elapsedTime = 0f;
            if (cards.Count>0)
            {
                Vector3 startingPosition = cards[0].transform.position;
                while (elapsedTime < timeOfTravel)
                {
                    cards[0].transform.position = Vector3.Lerp(startingPosition, handPlaceHolder.transform.position, (elapsedTime / timeOfTravel));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                cards[0].gameObject.transform.SetParent(handPlaceHolder.transform);
                cards.RemoveAt(0);
            }
            elapsedTime = 0;
            if (enemyCards.Count>0)
            {
                Vector3 startingPosition = enemyCards[0].transform.position;
                while (elapsedTime < timeOfTravel)
                {
                    enemyCards[0].transform.position = Vector3.Lerp(startingPosition, enemyHandPlaceHolder.transform.position, (elapsedTime / timeOfTravel));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                enemyCards[0].gameObject.transform.SetParent(enemyHandPlaceHolder.transform);
                enemyCards.RemoveAt(0);
            }
            yield return new WaitForSeconds(0.3f);
        }
        Driver.instance.phase = Phase.PLACEMENT;
        mainBtnText.text = "DEPLOY";
        yield return null;
        enemyPlay();
    }

    IEnumerator lerpColor(GameObject g)
    {
        yield return new WaitForSeconds(1f);
        mainBtnText.text = "";
        float ElapsedTime = 0.0f;
        float TotalTime = 0.4f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            g.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, (ElapsedTime / TotalTime));
            yield return null;
        }
        mainBtnText.text = "DEPLOY";
    }

    IEnumerator moveTo(GameObject g, GameObject v)
    {
        float timeOfTravel = 0.5f;
        float elapsedTime = 0f;
        Vector3 startingPosition = g.transform.position;
        while (elapsedTime < timeOfTravel)
        {
             g.transform.position = Vector3.Lerp(startingPosition, v.transform.position, (elapsedTime / timeOfTravel)); 
             elapsedTime += Time.deltaTime;
             yield return null;
        }
        g.transform.SetParent(v.transform);
    }

    public void enemyPlay()
    {
        List<GameObject> l = new List<GameObject>();
        for (int i = 0; i < enemyHandPlaceHolder.transform.childCount; i++)
        {
            l.Add(enemyHandPlaceHolder.transform.GetChild(i).gameObject);
        }
        //Rand List of 3ints from 1 to 6
        List<int> list = new List<int>(new int[3]);
        for (int j = 0; j < list.Count; j++)
        {
            int rand = Random.Range(1, 6);
            while (list.Contains(rand))
            {
                rand = Random.Range(1, 6);
            }
            list[j] = rand;
        }
        for (int i = 0; i < l.Count; i++)
        {
            if (cardHolders[list[i]].transform.childCount == 0)
            {
                StartCoroutine(moveTo(l[i], cardHolders[list[i]]));
            }

        }
    }

    public void flipCards()
    {
        for (int i = 0; i < cardHolders.Length; i++)
        {
            if(cardHolders[i].transform.childCount>0)
                cardHolders[i].transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false); ;
        }
    }

    public void placeAll()
    {
        List<GameObject> l = new List<GameObject>();
        for (int i = 0; i < myCardHolders.Length; i++)
        {
            if(myCardHolders[i].transform.childCount > 0)
                l.Add(myCardHolders[i].transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < l.Count; i++)
        {
            Vector3 v = Vector3.one * 0.25f;
            l[i].transform.localScale = v;
            Destroy(l[i].transform.GetChild(1).GetComponent<CardInstance>());
            l[i].transform.GetComponent<Draggable>().enabled = false;
            l[i].GetComponent<CardUI>().Old = true;
        }
        //moveToFrontRow(myCardHolders);
        //moveToFrontRow(cardHolders);
    }

    //TODO:IMPORVE IMPLEMENTATION
    public List<CardUI> loadCardUI()
    {
        List<CardUI> cu = new List<CardUI>();
        //Left Lane
        if (hasCard(myCardHolders, 0))
            cu.Add(myCardHolders[0].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(myCardHolders, 3))
            cu.Add(myCardHolders[3].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(cardHolders, 0))
            cu.Add(cardHolders[0].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(cardHolders, 3))
            cu.Add(cardHolders[3].transform.GetChild(0).GetComponent<CardUI>());
        //Middle Lane
        if (hasCard(myCardHolders, 1))
            cu.Add(myCardHolders[1].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(myCardHolders, 4))
            cu.Add(myCardHolders[4].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(cardHolders, 1))
            cu.Add(cardHolders[1].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(cardHolders, 4))
            cu.Add(cardHolders[4].transform.GetChild(0).GetComponent<CardUI>());
        //RightLane
        if (hasCard(myCardHolders, 2))
            cu.Add(myCardHolders[2].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(myCardHolders, 5))
            cu.Add(myCardHolders[5].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(cardHolders, 2))
            cu.Add(cardHolders[2].transform.GetChild(0).GetComponent<CardUI>());
        if (hasCard(cardHolders, 5))
            cu.Add(cardHolders[5].transform.GetChild(0).GetComponent<CardUI>());
        return cu;

    }

    public bool hasCard(GameObject[] g,int i)
    {
        return g[i].transform.childCount > 0;
    }

    public void moveToFrontRow(GameObject[] g)
    {
        for (int i = 3; i <= 5; i++)
        {
            if (g[i].transform.childCount > 0 && g[i - 3].transform.childCount == 0)
                StartCoroutine(moveTo(g[i].transform.GetChild(0).gameObject, g[i - 3]));
        }
    }
}
