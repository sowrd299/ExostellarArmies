using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
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
	private HandManager hand;
	[SerializeField]
	private HandManager enemyHand;
	[SerializeField]
	private GameObject damages;


	[SerializeField]
	[FormerlySerializedAs("myCardHolders")]
	public GameObject[] myUnitHolders;
	[SerializeField]
	[FormerlySerializedAs("cardHolders")]
	private GameObject[] enemyUnitHolders;
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
		handCapacity.text = "Hand capacity\n" + handPlaceHolder.gameObject.transform.childCount.ToString() + "/3";
		// resourseText.text = "Resources: " + Driver.instance?.gameManager?.Players[Client.instance.sideIndex]?.Mana?.Count.ToString();
		// dropCostSumText.text = "DropCostSum: " + Driver.instance?.dropCostSum.ToString();

		if (!Client.instance.initialized && !foundMatch)
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

	public void StartDrawPhase(Player[] players)
	{
		mainBtnText.text = "DRAWING...";
		mainButton.GetComponent<Image>().color = new Color(102, 255, 102);
		spawnCards(players);
		Driver.instance.updateTowerUI();
	}

	public void mainBtn()
	{
		if (mainBtnText.text.Equals("LOCK IN PLANS"))
		{
			if (Client.instance.gameManager.Players[Client.instance.sideIndex].Mana.CanAfford(Driver.instance.dropCostSum))
			{
				PlayUnitCardAction[] actions = hand.ExportActions();

				Client.instance.SendPlanningPhaseActions(actions);
				mainBtnText.text = "WAITING FOR OPPONENT";
				mainButton.GetComponent<Image>().color = new Color(153, 204, 255);
			}
			else
			{
				Debug.Log("CANNOT AFFORD");
			}
		}
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
		for (int i = 0; i < enemyUnitHolders.Length; i++)
		{
			if (enemyUnitHolders[i].transform.childCount > 0)
			{
				CardUI c = enemyUnitHolders[i].transform.GetChild(0).GetComponent<CardUI>();
				if (c.Old == false)
				{
					Card back = c.cardBackEnd;
					if (enemyUnitHolders[i].transform.parent.name.Contains("Front"))
						actions.Add(new PlayUnitCardAction(back as UnitCard, Client.instance.gameManager.Lanes[i % 3], 1, 0));
					else
						actions.Add(new PlayUnitCardAction(back as UnitCard, Client.instance.gameManager.Lanes[i % 3], 1, 1));
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
		int handCardCount = hand.GetCardCount();
		while (handCardCount + cards.Count < 3)
		{
			GameObject tempCard = Instantiate(cardPrefab, placeHolder.transform);
			cards.Add(tempCard);
		}

		int enemyHandCardCount = enemyHand.GetCardCount();
		while (enemyHandCardCount + enemyCards.Count < 3)
		{
			GameObject tempCard2 = Instantiate(enemyCardPrefab, enemyPlaceHolder.transform);
			enemyCards.Add(tempCard2);
		}

		List<CardPropertyMap> feList1 = Driver.instance.loadFrontEnd(players[Client.instance.sideIndex]);
		//List<CardFrontEnd> feList2 = Driver.instance.loadFrontEnd(players[1]);

		List<CardUI> ui1 = loadCardUIinHand(placeHolder);

		for (int i = 0; i < Mathf.Min(feList1.Count, ui1.Count); i++)
		{
			//Debug.Log("I"+i);
			ui1[i].LoadCard(feList1[i]);
		}

		//        Driver.instance.drawCards();
		StartCoroutine(moveToHand());
	}

	IEnumerator moveToHand()
	{
		Coroutine drawCoroutine = hand.MoveToHand(cards);
		Coroutine enemyDrawCoroutine = enemyHand.MoveToHand(enemyCards);
		yield return drawCoroutine;
		yield return enemyDrawCoroutine;
		cards.Clear();
		enemyCards.Clear();

		mainBtnText.text = "LOCK IN PLANS";
		mainButton.GetComponent<Image>().color = Color.green;

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
			if (enemyUnitHolders[list[i]].transform.childCount == 0)
			{
				StartCoroutine(moveTo(l[i], enemyUnitHolders[list[i]]));
			}

		}
	}

	public void flipCards()
	{
		for (int i = 0; i < enemyUnitHolders.Length; i++)
		{

			if (enemyUnitHolders[i].transform.childCount > 0)
				enemyUnitHolders[i].transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false); ;
		}
	}

	public void placeAll()
	{
		List<GameObject> l = new List<GameObject>();
		for (int i = 0; i < myUnitHolders.Length; i++)
		{
			if (myUnitHolders[i].transform.childCount > 0)
				l.Add(myUnitHolders[i].transform.GetChild(0).gameObject);
		}
		for (int i = 0; i < l.Count; i++)
		{
			Vector3 v = Vector3.one * 0.25f;
			l[i].transform.localScale = v;

			Destroy(l[i].transform.GetChild(1).GetComponent<CardInstance>());
			l[i].GetComponent<CardUI>().Old = true;
		}
		StartCoroutine(moveToFrontRow(myUnitHolders));
		StartCoroutine(moveToFrontRow(enemyUnitHolders));
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
		for (int i = 0; i < enemyUnitHolders.Length; i++)
		{
			if (enemyUnitHolders[i].transform.childCount > 0)
			{
				CardUI c = enemyUnitHolders[i].transform.GetChild(0).GetComponent<CardUI>();
				int hp;
				int.TryParse(c.propertyDisplays[6].text.text, out hp);
				if (hp <= 0)
					Destroy(enemyUnitHolders[i].transform.GetChild(0).gameObject);
			}
		}
		for (int i = 0; i < myUnitHolders.Length; i++)
		{
			if (myUnitHolders[i].transform.childCount > 0)
			{
				CardUI c2 = myUnitHolders[i].transform.GetChild(0).GetComponent<CardUI>();
				int hp;
				int.TryParse(c2.propertyDisplays[6].text.text, out hp);
				if (hp <= 0)
					Destroy(myUnitHolders[i].transform.GetChild(0).gameObject);
			}
		}
	}

	//TODO:IMPORVE IMPLEMENTATION
	public List<CardUI> loadCardUI()
	{
		List<CardUI> cu = new List<CardUI>();
		//Left Lane
		if (hasCard(myUnitHolders, 0))
			cu.Add(myUnitHolders[0].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(myUnitHolders, 3))
			cu.Add(myUnitHolders[3].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(enemyUnitHolders, 0))
			cu.Add(enemyUnitHolders[0].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(enemyUnitHolders, 3))
			cu.Add(enemyUnitHolders[3].transform.GetChild(0).GetComponent<CardUI>());
		//Middle Lane
		if (hasCard(myUnitHolders, 1))
			cu.Add(myUnitHolders[1].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(myUnitHolders, 4))
			cu.Add(myUnitHolders[4].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(enemyUnitHolders, 1))
			cu.Add(enemyUnitHolders[1].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(enemyUnitHolders, 4))
			cu.Add(enemyUnitHolders[4].transform.GetChild(0).GetComponent<CardUI>());
		//RightLane
		if (hasCard(myUnitHolders, 2))
			cu.Add(myUnitHolders[2].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(myUnitHolders, 5))
			cu.Add(myUnitHolders[5].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(enemyUnitHolders, 2))
			cu.Add(enemyUnitHolders[2].transform.GetChild(0).GetComponent<CardUI>());
		if (hasCard(enemyUnitHolders, 5))
			cu.Add(enemyUnitHolders[5].transform.GetChild(0).GetComponent<CardUI>());
		return cu;

	}

	public List<CardUI> loadCardUIinHand(GameObject g)
	{
		List<CardUI> cu = new List<CardUI>();
		for (int i = 0; i < g.transform.childCount; i++)
		{
			CardUI c = g.transform.GetChild(i).GetComponent<CardUI>();
			cu.Add(c);
		}
		return cu;
	}

	public bool hasCard(GameObject[] g, int i)
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
