using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.Serialization;
using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Net.Client;

public class Driver : MonoBehaviour
{
	public string deckKey;

	public static Driver instance = null;

	// Private references and convenience getters
	private Client client => Client.instance;
	public CardLoader cardLoader => CardLoader.instance;

	// Public getters
	[HideInInspector]
	public GameManager gameManager;
	public int sideIndex { get; private set; }
	public bool inGame => gameManager != null;

	public UIManager uiManager => UIManager.instance;

	private List<TurnPhase> phaseStash;

	private void Awake()
	{
		Debug.Log(new RangedShield(1) == new RangedShield(1) ? 'T' : 'F');

		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		phaseStash = new List<TurnPhase>();
	}

	private List<XmlElement> GetDeltaElements(XmlElement element)
	{
		return element
			.GetElementsByTagName("delta")
			.OfType<XmlElement>()
			.ToList();
	}

	private List<XmlElement> GetInputRequestElements(XmlElement element)
	{
		return element
			.GetElementsByTagName("inputRequest")
			.OfType<XmlElement>()
			.ToList();
	}

	private IEnumerator Start()
	{
		uiManager.WaitForMatch();

		Task joinMatch = client.JoinMatch(PlayerPrefs.GetString(deckKey));
		yield return new WaitUntil(() => joinMatch.IsCompleted);

		Task<XmlDocument> matchStart = client.ReceiveDocument(type => type == "matchStart");
		yield return new WaitUntil(() => matchStart.IsCompleted);
		InitializeGameState(matchStart.Result);

		while (true) // TODO: Detect game end?
		{
			Task<XmlDocument> turnStart = client.ReceiveDocument(type => type == "turnStart");
			yield return new WaitUntil(() => turnStart.IsCompleted);
			uiManager.BeforeTurnStart();
			yield return StartCoroutine(ProcessTurnStart(turnStart.Result));

			if (gameManager.Players[sideIndex].DeployPhases > 0)
			{
				yield return uiManager.WaitForLockIn();

				client.SendPlayerActions(uiManager.myHandManager.ExportActions());
				Task<XmlDocument> confirmAction = client.ReceiveDocument(type => type == "actionDeltas");
				yield return new WaitUntil(() => confirmAction.IsCompleted);
				ProcessActionResults(confirmAction.Result);

				uiManager.WaitForOpponent();
				client.LockInTurn();
			}
		}
	}

	private void InitializeGameState(XmlDocument matchStart)
	{
		sideIndex = int.Parse(matchStart.DocumentElement.GetAttribute("sideIndex"));

		XmlElement[] players = matchStart.GetElementsByTagName("playerIds").OfType<XmlElement>().ToArray();
		XmlElement myPlayer = players.First(player => player.GetAttribute("side") == "local");
		players[Array.IndexOf(players, myPlayer)] = players[sideIndex];
		players[sideIndex] = myPlayer;

		XmlElement[] lanes = matchStart.GetElementsByTagName(Lane.TAG_NAME).OfType<XmlElement>().ToArray();

		gameManager = new GameManager(null, players, lanes);

		uiManager.InitializeUI();
	}

	private void ProcessActionResults(XmlDocument document)
	{
		List<XmlElement> deltaElements = GetDeltaElements(document.DocumentElement);

		Debug.Log($"Processing {deltaElements.Count} action deltas");
		foreach (XmlElement element in deltaElements)
		{
			Delta delta = Delta.FromXml(element, cardLoader);
			delta.Apply();
		}

		uiManager.RenderUnits();
		uiManager.LockUnits();
		uiManager.UpdateDiscardDisplay(0);
		uiManager.UpdateDiscardDisplay(1);
		uiManager.UpdateResourceDisplay(0);
		uiManager.UpdateResourceDisplay(1);
	}

	private IEnumerator ProcessTurnStart(XmlDocument document)
	{
		phaseStash.AddRange(
			document
				.DocumentElement
				.GetElementsByTagName("phase")
				.OfType<XmlElement>()
				.Select(element => new TurnPhase(element, cardLoader))
		);

		List<XmlElement> inputRequestElements = GetInputRequestElements(document.DocumentElement);
		
		if (inputRequestElements.Count > 0)
		{
			// Input request time!
			List<InputRequest> requests = inputRequestElements.Select(InputRequest.FromXml).ToList();

			Debug.Log($"Processing {inputRequestElements.Count} input requests");
			foreach (InputRequest request in requests)
			{
				yield return StartCoroutine(ProcessInputRequest(request));
			}

			client.SendInputRequestResponse(requests.ToArray());
			Task<XmlDocument> confirmInput = client.ReceiveDocument(type => type == "turnStart");
			yield return new WaitUntil(() => confirmInput.IsCompleted);

			yield return StartCoroutine(ProcessTurnStart(confirmInput.Result));
		}

		while (phaseStash.Count > 0)
		{
			TurnPhase phase = phaseStash[0];
			phaseStash.RemoveAt(0);

			List<Delta> deltas = new List<Delta>(phase.Deltas);
			for (int i = 0; i < phaseStash.Count; i++)
			{
				if (phaseStash[i].Name == phase.Name)
				{
					phaseStash.RemoveAt(i);
					deltas.AddRange(phaseStash[i].Deltas);
					i--;
				}
			}

			if (deltas.Count == 0) continue;

			string phaseDisplayName = GetPhaseName(phase.Name);
			if (phaseDisplayName != null) yield return uiManager.ShowPhaseName(phaseDisplayName);

			Debug.Log($"Processing {deltas.Count} deltas of phase {phase.Name}");
			foreach (Delta delta in deltas)
			{
				delta.Apply();
				yield return StartCoroutine(AnimateDelta(delta));
			}

			// Special case for draw phase. I don't really like this, but I don't see a way around it.
			if (phase.Name == "startDeploy")
			{
				yield return uiManager.OpponentDrawCards();
			}
		}
	}

	private IEnumerator AnimateDelta(Delta delta)
	{
		if (delta is AddToHandDelta)
		{
			yield return uiManager.DrawCard((delta as AddToHandDelta).Card);
		}
		else if (delta is AddToDiscardDelta)
		{
			uiManager.UpdateDiscardDisplay(
				Array.FindIndex(gameManager.Players, player => player.Discard == (delta as AddToDiscardDelta).Target)
			);
		}
		else if (delta is AddToLaneDelta)
		{
			AddToLaneDelta addToLaneDelta = delta as AddToLaneDelta;
			yield return uiManager.SpawnUnit(addToLaneDelta.SideIndex, Array.FindIndex(gameManager.Lanes, lane => lane.ID == addToLaneDelta.Target.ID), addToLaneDelta.Position);
		}
		else if (delta is UnitHealthDelta)
		{
			UnitHealthDelta damageDelta = delta as UnitHealthDelta;

			if (damageDelta.DmgType == Damage.Type.TOWER)
			{
				yield return uiManager.TowerUnitDamage(damageDelta.Target, damageDelta.Amount);
			}
			else if (damageDelta.DmgType == Damage.Type.HEAL)
			{
				yield return uiManager.UnitHeal(damageDelta.Source, damageDelta.Target);
			}
			else
			{
				yield return uiManager.UnitDamage(
					damageDelta.Source,
					damageDelta.Target,
					damageDelta.Amount
				);
			}
		}
		else if (delta is RemoveFromLaneDelta)
		{
			RemoveFromLaneDelta removeDelta = delta as RemoveFromLaneDelta;
			yield return uiManager.RemoveUnit(
				Array.FindIndex(gameManager.Lanes, lane => lane.ID == removeDelta.Target.ID),
				removeDelta.SideIndex,
				removeDelta.Position
			);
		}
		else if (delta is TowerDamageDelta)
		{
			TowerDamageDelta towerDamageDelta = delta as TowerDamageDelta;
			yield return uiManager.UnitTowerDamage(towerDamageDelta.Target, towerDamageDelta.Amount);
		}
		else if (delta is TowerReviveDelta)
		{
			TowerReviveDelta towerReviveDelta = delta as TowerReviveDelta;
			yield return uiManager.TowerRespawn(towerReviveDelta.Target);
		}
		else if (delta is RevealHandCardDelta)
		{
			RevealHandCardDelta revealDelta = delta as RevealHandCardDelta;
			yield return uiManager.RevealOpponentCard(revealDelta.Card);
		}
		else if (delta is RemoveFromHandDelta)
		{
			RemoveFromHandDelta removeDelta = delta as RemoveFromHandDelta;
			yield return uiManager.RemoveFromHand(
				Array.FindIndex(gameManager.Players, player => player.Owns(removeDelta.Target)),
				removeDelta.Card
			);
		}
		else if (delta is ResourcePoolDelta)
		{
			ResourcePoolDelta resourcePoolDelta = delta as ResourcePoolDelta;
			ResourcePool target = resourcePoolDelta.rp.Target;
			for (int i = 0; i < gameManager.Players.Length; i++)
			{
				if (target == gameManager.Players[i].ManaPool)
				{
					yield return uiManager.UpdateResourceDisplay(i);
				}
			}
		}
		else
		{
			// Some deltas simply can't be animated, while others could potentially indicate an error.
			if (!(delta is RemoveFromDeckDelta))
			{
				Debug.LogWarning($"Failed to animate delta of type {delta.GetType().Name}");
			}
		}

		uiManager.RenderUnits();
		uiManager.RenderTowers();
		uiManager.RenderIndicators();
	}

	private IEnumerator ProcessInputRequest(InputRequest request)
	{
		if (request is InputRequest<Card>)
		{
			InputRequest<Card> cardRequest = request as InputRequest<Card>;

			StringBuilder cardIdBuilder = new StringBuilder();
			yield return SelectCardUI.instance.ChooseCard(gameManager.Players[sideIndex].Hand.ToArray(), cardIdBuilder);
			string cardId = cardIdBuilder.ToString();

			cardRequest.MakeChoice(cardLoader.GetByID(cardId));
		}
	}

	private string GetPhaseName(string phaseCodeName)
	{
		switch (phaseCodeName)
		{
			case "endDeploy":
				return null;
			case "rangedCombat":
				return "Ranged Combat Phase";
			case "meleeCombat":
				return "Melee Combat Phase";
			case "towerCombat":
				return "Tower Combat Phase";
			case "startPhase":
				return null; // Not sure what this does?
			case "startDeploy":
				return "Turn Start";
			default:
				if (phaseCodeName.StartsWith("player"))
				{
					return "Opponent Actions";
				}
				else
				{
					Debug.LogWarning($"Unknown phase name {phaseCodeName}");
					return null;
				}
		}
	}
}