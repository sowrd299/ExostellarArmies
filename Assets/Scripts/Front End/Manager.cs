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
    private Text mainBtnText;
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
                    Driver.instance.phase = Phase.COMBAT;
                    flipCards();
                    placeAll();

                    List<PlayUnitCardAction> actions = new List<PlayUnitCardAction>();
                    for (int i = 0; i < myCardHolders.Length; i++)
                    {
                        if(myCardHolders[i].transform.childCount>0)
                        {
                            CardUI c = myCardHolders[i].transform.GetChild(0).GetComponent<CardUI>();
                            Card back = c.cardBackEnd;
                            actions.Add(new PlayUnitCardAction(back as UnitCard, Driver.instance.myLanes[i%3], 0, 1));
                            //myCardHolders need to be in correct order
                        }
                    }
                    foreach(PlayUnitCardAction action in actions)
                    {
                        foreach (Delta del in action.GetDeltas(Driver.instance.gameManager.Players[0]))
                            del.Apply();
                    }
                }
                else
                {
                    mainBtnText.text = "Cant Afford!";
                    mainButton.gameObject.GetComponent<Image>().color = Color.red;
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
            float timeOfTravel = 0.5f;
            float elapsedTime = 0f;
            if (cards[0] != null)
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
            if (enemyCards[0] != null)
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
        for (int i = 0; i < l.Count; i++)
        {
            StartCoroutine(moveTo(l[i], cardHolders[i]));
        }
    }

    public void flipCards()
    {
        for (int i = 0; i < cardHolders.Length; i++)
        {
            cardHolders[i].transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false); ;
        }
    }

    public void placeAll()
    {
        List<GameObject> l = new List<GameObject>();
        for (int i = 0; i < cardHolders.Length; i++)
        {
            if(myCardHolders[i].transform.childCount > 0)
                l.Add(myCardHolders[i].transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < l.Count; i++)
        {
            Vector3 v = Vector3.one * 0.25f;
            l[i].transform.localScale = v;
        }

    }
}
