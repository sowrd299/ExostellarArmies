using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Management;
using SFB.Net.Client;

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
    private GameObject damages;


    [SerializeField]
    private GameObject[] cardHolders;
    [SerializeField]
    public GameObject[] myCardHolders;
    [SerializeField]
    private GameObject[] towerHolders;

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
    private bool foundMatch = false;

    private List<int> lane1Damages = new List<int>();
    public List<int> Lane1Damages
    {
        get
        {
            return lane1Damages;
        }
    }

    private List<int> lane2Damages = new List<int>();
    public List<int> Lane2Damages
    {
        get
        {
            return lane2Damages;
        }
    }

    private List<int> lane3Damages = new List<int>();
    public List<int> Lane3Damages
    {
        get
        {
            return lane3Damages;
        }
    }

    public void addDamages(int[,] sums)
    {
        lane1Damages.Add(sums[0, 0]);
        lane1Damages.Add(sums[1, 0]);
        lane2Damages.Add(sums[0, 1]);
        lane2Damages.Add(sums[1, 1]);
        lane3Damages.Add(sums[0, 2]);
        lane3Damages.Add(sums[1, 2]);
    }

    public void clearDamages()
    {
        lane1Damages.Clear();
        lane2Damages.Clear();
        lane3Damages.Clear();
    }

    public void loadDamages(int[,] sums)
    {
        damages.SetActive(true);
        DamageAnimationController d = damages.GetComponent<DamageAnimationController>();
        List<GameObject> lanes = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            lanes.Add(damages.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < lanes.Count; i++)
        {
            Text ourTotalDamage = lanes[i].transform.GetChild(0).GetComponent<Text>();
            ourTotalDamage.text = sums[0, i].ToString();
            Text enemyTotalDamage = lanes[i].transform.GetChild(1).GetComponent<Text>();
            enemyTotalDamage.text = sums[1, i].ToString();
        }
        IEnumerator c = d.startAnim();
        StartCoroutine(c);
    }


    private void FixedUpdate()
    {
        currentState.startActions();
        handCapacity.text = "Hand capacity\n" + handPlaceHolder.gameObject.transform.childCount.ToString()+"/3";
        resourseText.text = "Resources: " + Driver.instance.resoureCount.ToString();
        dropCostSumText.text = "DropCostSum: " + Driver.instance.dropCostSum.ToString();
		if(Driver.instance.NETWORK) {
			if(!Client.Instance.DoneInitializing && !foundMatch) // Change to CLient.Instance later
			{
				mainBtnText.text = "Waiting for match!";
				//mainButton.enabled = false;
				mainButton.GetComponent<Image>().color = Color.grey;
			}/* else if(Client.Instance.DoneInitializing && !foundMatch) {
				mainBtnText.text = "Draw";
				mainButton.enabled = true;
				mainButton.GetComponent<Image>().color = Color.green;
			}*/
		}
	}

	public void StartDrawPhase(Player[] players) {
		mainBtnText.text = "DRAWING...";
		mainButton.GetComponent<Image>().color = new Color(102, 255, 102);
		spawnCards(players);
		Driver.instance.updateTowerUI();
	}

	public void mainBtn()
    {
		if(mainBtnText.text.Equals("LOCK IN PLANS")) {
			if(Driver.instance.myMana.CanAfford(Driver.instance.dropCostSum)) {
				List<PlayUnitCardAction> actions = new List<PlayUnitCardAction>();
				for(int i = 0; i < myCardHolders.Length; i++) {
					if(myCardHolders[i].transform.childCount > 0) {
						CardUI c = myCardHolders[i].transform.GetChild(0).GetComponent<CardUI>();

						if(c.Old == false) {
							Card back = c.cardBackEnd;
							if(myCardHolders[i].transform.parent.name.Contains("Front"))
								actions.Add(new PlayUnitCardAction(back as UnitCard, Client.Instance.GameManager.Lanes[i % 3], 0, 0));
							else
								actions.Add(new PlayUnitCardAction(back as UnitCard, Client.Instance.GameManager.Lanes[i % 3], 0, 1));
							//myCardHolders need to be in correct order
						}
					}
				}
				Debug.Log(actions.Count);

				Client.Instance.SendPlanningPhaseActions(actions.ToArray());
			} else {

			}
		}
		/*
        switch (Driver.instance.phase)
        {
            case Phase.DRAW:
                spawnCards();
                Driver.instance.updateTowerUI();
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

                    if (Driver.instance.NETWORK)
                        Driver.instance.client.SendPlanningPhaseActions(actions.ToArray());
                    foreach(PlayUnitCardAction action in actions)
                    {
                        foreach (Delta del in action.GetDeltas(Driver.instance.gameManager.Players[0]))
                            del.Apply();
                    }
                    placeAll();
                    //Driver.instance.phase = Phase.COMBAT;
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

        }*/
	}
	
    public IEnumerator damageAnims()
    {
        mainBtnText.text = "Range Combat!";
        Driver.instance.gameManager.CombatRangePhase();
        Driver.instance.updateCardsOntable();
        Driver.instance.updateTowerUI();
        yield return new WaitForSeconds(1.5f);
        mainBtnText.text = "Melle Combat!";
        Driver.instance.gameManager.CombatMellePhase();
        Driver.instance.updateCardsOntable();
        Driver.instance.updateTowerUI();
        yield return new WaitForSeconds(1.5f);
        mainBtnText.text = "Tower Combat!";
        Driver.instance.gameManager.CombatMellePhase();
        Driver.instance.updateCardsOntable();
        Driver.instance.updateTowerUI();
        yield return new WaitForSeconds(1.5f);
        Driver.instance.gameManager.cleanUp();
        mainBtnText.text = "Combat done1";
        yield return new WaitForSeconds(1f);
        mainBtnText.text = "Draw";
        //Driver.instance.phase = Driver.instance.gameManager.Over ? Phase.DONE : Phase.DRAW;
    }

    public void applyEnemyDeltas()
    {
		Debug.Log("ENEMY DELTAS");
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
                        actions.Add(new PlayUnitCardAction(back as UnitCard, Client.Instance.GameManager.Lanes[i % 3], 1, 0));
                    else
                        actions.Add(new PlayUnitCardAction(back as UnitCard, Client.Instance.GameManager.Lanes[i % 3], 1, 1));
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

    public void spawnCards(Player[] players)
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

		List<CardFrontEnd> feList1 = Driver.instance.loadFrontEnd(players[0]);
		//List<CardFrontEnd> feList2 = Driver.instance.loadFrontEnd(players[1]);

		List<CardUI> ui1 = loadCardUIinHand(placeHolder);

		Debug.Log(feList1.Count + " " + ui1.Count);

		for(int i = 0; i < Mathf.Min(feList1.Count, ui1.Count); i++) {
			Debug.Log("I"+i);
			ui1[i].LoadCard(feList1[i]);
		}

		//        Driver.instance.drawCards();
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
		mainBtnText.text = "LOCK IN PLANS";
		mainButton.GetComponent<Image>().color = Color.green;
		makeDraggable(true);
        yield return null;
        //enemyPlay();
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
        StartCoroutine(moveToFrontRow(myCardHolders));
        StartCoroutine(moveToFrontRow(cardHolders));
    }

    public List<TowerUI> loadTowerUI()
    {
        List<TowerUI> tu = new List<TowerUI>();
        for (int i = 0; i < towerHolders.Length; i++)
        {
            tu.Add(towerHolders[i].GetComponent<TowerUI>());
        }
        return tu;
    }

    public void cleanup()
    {
        for (int i = 0; i < cardHolders.Length; i++)
        {
            if(cardHolders[i].transform.childCount>0)
            {
                CardUI c = cardHolders[i].transform.GetChild(0).GetComponent<CardUI>();
                int hp;
                int.TryParse(c.properties[6].text.text, out hp);
                if (hp <= 0)
                    Destroy(cardHolders[i].transform.GetChild(0).gameObject);
            }
        }
        for (int i = 0; i < myCardHolders.Length; i++)
        {
            if (myCardHolders[i].transform.childCount > 0)
            {
                CardUI c2 = myCardHolders[i].transform.GetChild(0).GetComponent<CardUI>();
                int hp;
                int.TryParse(c2.properties[6].text.text, out hp);
                if (hp <= 0)
                    Destroy(myCardHolders[i].transform.GetChild(0).gameObject);
            }
        }
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

	public List<CardUI> loadCardUIinHand(GameObject g) {
		List<CardUI> cu = new List<CardUI>();
		for(int i = 0; i < g.transform.childCount; i++) {
			CardUI c = g.transform.GetChild(i).GetComponent<CardUI>();
			cu.Add(c);
		}
		return cu;
	}

	public void makeDraggable(bool b)
    {
        for (int i = 0; i < handPlaceHolder.transform.childCount; i++)
        {
            handPlaceHolder.transform.GetChild(i).GetComponent<Draggable>().enabled = b;
        }
    }

    public bool hasCard(GameObject[] g,int i)
    {
        return g[i].transform.childCount > 0;
    }

    IEnumerator moveToFrontRow(GameObject[] g)
    {
        for (int i = 3; i <= 5; i++)
        {
            if (g[i].transform.childCount > 0 && g[i - 3].transform.childCount == 0)
            {
                float timeOfTravel = 0.5f;
                float elapsedTime = 0f;
                Vector3 startingPosition = g[i].transform.GetChild(0).gameObject.transform.position;
                while (elapsedTime < timeOfTravel)
                {
                    g[i].transform.GetChild(0).gameObject.transform.position = Vector3.Lerp(startingPosition, g[i - 3].transform.position, (elapsedTime / timeOfTravel));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                g[i].transform.GetChild(0).gameObject.transform.SetParent(g[i - 3].transform);
            }
        }
        //Driver.instance.phase = Phase.COMBAT;
    }
}
