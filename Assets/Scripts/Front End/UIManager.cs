using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using SFB.Game;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Net.Client;

public class UIManager : MonoBehaviour
{
	public static UIManager instance => Driver.instance.manager;

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

	public Element hpElement;
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
		if (myPlayer.Mana.CanAfford(myHandManager.deploymentCost))
		{
			mainButton.interactable = true;
			mainButtonText.text = "LOCK IN PLANS";
		}
		else
		{
			mainButton.interactable = false;
			mainButtonText.text = "Not enough resources!";
		}
	}

	public void mainBtn()
	{
		if (mainButtonText.text.Equals("LOCK IN PLANS"))
		{
			if (myPlayer.Mana.CanAfford(myHandManager.deploymentCost))
			{
				PlayUnitCardAction[] actions = myHandManager.ExportActions();

				Client.instance.SendPlanningPhaseActions(actions);
				mainButtonText.text = "WAITING FOR OPPONENT";
				mainButton.GetComponent<Image>().color = new Color(153, 204, 255);

				LockUnitsOnField();
			}
			else
			{
				Debug.Log("CANNOT AFFORD");
			}
		}
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

	public void ApplyEnemyUnits()
	{
		for (int laneIndex = 0; laneIndex < gameManager.Lanes.Length; laneIndex++)
		{
			Lane lane = gameManager.Lanes[laneIndex];

			for (int unitPos = 0; unitPos < 2; unitPos++)
			{
				Unit unit = lane.Units[1 - Client.instance.sideIndex, unitPos];
				Transform unitHolder = enemyUnitHolders.First(
					holder => holder.GetComponent<UnitHolder>().laneIndex == laneIndex && holder.GetComponent<UnitHolder>().positionIndex == unitPos
				).transform;

				if (unit == null)
				{
					foreach (Transform child in unitHolder)
					{
						Destroy(child);
					}
				}
				else
				{
					GameObject unitObject;

					if (unitHolder.childCount == 0)
					{
						unitObject = unitHolder.GetComponent<UnitHolder>().InstantiateUnit(
							unit.Card,
							new CardPropertyMap(
								Driver.instance.createCardProperties(unit)
							)
						);
					}
					else
					{
						unitObject = unitHolder.GetChild(0).gameObject;
						unitObject.GetComponent<CardUI>().LoadCard(
							new CardPropertyMap(Driver.instance.createCardProperties(unit))
						);
					}
				}
			}
		}
	}

	public void LockUnitsOnField()
	{
		foreach (GameObject unitHolder in myUnitHolders)
		{
			if (unitHolder.transform.childCount > 0)
			{
				GameObject unitObject = unitHolder.transform.GetChild(0).gameObject;
				unitObject.GetComponent<DragSource>().enabled = false;
			}
		}

		foreach (GameObject unitHolder in enemyUnitHolders)
		{
			if (unitHolder.transform.childCount > 0)
			{
				GameObject unitObject = unitHolder.transform.GetChild(0).gameObject;
				unitObject.GetComponent<DragSource>().enabled = false;
			}
		}
	}

	private Transform FindUnitHolderAt(int sideIndex, int laneIndex, int positionIndex)
	{
		Debug.Log($"Looking for unit holder at side {sideIndex} lane {laneIndex} position {positionIndex}");
		GameObject[] sideUnitHolders = sideIndex == Client.instance.sideIndex ? myUnitHolders : enemyUnitHolders;

		UnitHolder unitHolder = sideUnitHolders
			.Select(holder => holder.GetComponent<UnitHolder>())
			.First(holder => holder.laneIndex == laneIndex && holder.positionIndex == positionIndex);
		
		return unitHolder.transform;
	}

	public CardUI FindUnitAt(int sideIndex, int laneIndex, int positionIndex)
	{
		Transform unitHolder = FindUnitHolderAt(sideIndex, laneIndex, positionIndex);
		
		GameObject unitObject = unitHolder.transform.GetChild(0).gameObject;

		return unitObject.GetComponent<CardUI>();
	}

	public void RemoveUnitAt(int sideIndex, int laneIndex, int positionIndex)
	{
		Transform unitHolder = FindUnitHolderAt(sideIndex, laneIndex, positionIndex);

		foreach (Transform child in unitHolder)
		{
			Destroy(child);
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
				int.TryParse(c.GetProperty(hpElement).text.text, out hp);
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
				int.TryParse(c2.GetProperty(hpElement).text.text, out hp);
				if (hp <= 0)
					Destroy(myUnitHolders[i].transform.GetChild(0).gameObject);
			}
		}
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
