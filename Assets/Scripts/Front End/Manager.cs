using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using SFB.Game;
using SFB.Game.Management;
using SFB.Net.Client;

public class Manager : MonoBehaviour
{
	public static Manager instance => Driver.instance.manager;

	private static Player[] players => Driver.instance.gameManager.Players;
	private static GameManager gameManager => Driver.instance.gameManager;
	private static Player myPlayer => gameManager.Players[Client.instance.sideIndex];

	public State currentState;

	[SerializeField]
	private HandManager myHandManager;
	[SerializeField]
	private HandManager enemyHandManager;
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

	public Button mainButton;
	public Text mainButtonText;
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
		handCapacity.text = "Hand capacity\n" + myHandManager.GetCardCount() + "/3";
		// resourseText.text = "Resources: " + Driver.instance?.gameManager?.Players[Client.instance.sideIndex]?.Mana?.Count.ToString();
		// dropCostSumText.text = "DropCostSum: " + Driver.instance?.dropCostSum.ToString();

		if (!Client.instance.initialized && !foundMatch)
		{
			mainButtonText.text = "Waiting for match!";
			mainButton.interactable = false;
		}
	}

	public void InitializeUI()
	{
		mainButton.interactable = true;

		myHandManager.TrackHand(players[Client.instance.sideIndex].Hand);
		enemyHandManager.TrackHand(players[1 - Client.instance.sideIndex].Hand);
	}

	public void AfterDrawPhase()
	{
		StartCoroutine(AnimateDrawPhase());

		Driver.instance.updateTowerUI();
	}

	private IEnumerator AnimateDrawPhase()
	{
		mainButtonText.text = "DRAWING...";
		mainButton.interactable = false;

		Coroutine myDraw = myHandManager.DrawCards();
		Coroutine enemyDraw = enemyHandManager.DrawCards();
		yield return myDraw;
		yield return enemyDraw;

		mainButtonText.text = "LOCK IN PLANS";
		mainButton.interactable = true;
	}

	public void ValidateDropCost()
	{
		if (myHandManager.deploymentCost > myPlayer.Mana.Count)
		{
			mainButton.interactable = false;
			mainButtonText.text = "Not enough resources!";
		}
		else
		{
			mainButton.interactable = true;
			mainButtonText.text = "LOCK IN PLANS";
		}
	}

	public void mainBtn()
	{
		if (mainButtonText.text.Equals("LOCK IN PLANS"))
		{
			if (Client.instance.gameManager.Players[Client.instance.sideIndex].Mana.CanAfford(Driver.instance.dropCostSum))
			{
				PlayUnitCardAction[] actions = myHandManager.ExportActions();

				Client.instance.SendPlanningPhaseActions(actions);
				mainButtonText.text = "WAITING FOR OPPONENT";
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
		mainButtonText.text = "Range Combat!";
		Driver.instance.gameManager.CombatRangePhase();
		Driver.instance.updateCardsOntable();
		Driver.instance.updateTowerUI();
		yield return new WaitForSeconds(1.5f);
		mainButtonText.text = "Melle Combat!";
		Driver.instance.gameManager.CombatMellePhase();
		Driver.instance.updateCardsOntable();
		Driver.instance.updateTowerUI();
		yield return new WaitForSeconds(1.5f);
		mainButtonText.text = "Tower Combat!";
		Driver.instance.gameManager.CombatMellePhase();
		Driver.instance.updateCardsOntable();
		Driver.instance.updateTowerUI();
		yield return new WaitForSeconds(1.5f);
		Driver.instance.gameManager.cleanUp();
		mainButtonText.text = "Combat done1";
		yield return new WaitForSeconds(1f);
		mainButtonText.text = "Draw";
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
		// TODO: Implement actual enemy action display instead of whatever this is
		// List<GameObject> l = new List<GameObject>();
		// for (int i = 0; i < enemyHandPlaceHolder.transform.childCount; i++)
		// {
		// 	l.Add(enemyHandPlaceHolder.transform.GetChild(i).gameObject);
		// }

		// //Rand List of 3ints from 1 to 6
		// List<int> list = new List<int>(new int[3]);
		// for (int j = 0; j < list.Count; j++)
		// {
		// 	int rand = Random.Range(1, 6);
		// 	while (list.Contains(rand))
		// 	{
		// 		rand = Random.Range(1, 6);
		// 	}
		// 	list[j] = rand;
		// }
		// for (int i = 0; i < l.Count; i++)
		// {
		// 	if (enemyUnitHolders[list[i]].transform.childCount == 0)
		// 	{
		// 		StartCoroutine(moveTo(l[i], enemyUnitHolders[list[i]]));
		// 	}

		// }
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
