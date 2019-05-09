using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Net.Client;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Driver : MonoBehaviour
{
	public static Driver instance = null;

	// Private references and convenience getters
	private Client client => Client.instance;
	private CardLoader _cardLoader;
	private CardLoader cardLoader => _cardLoader ?? (_cardLoader = new CardLoader());

	// Public getters
	[HideInInspector]
	public GameManager gameManager;
	public int sideIndex { get; private set; }
	public bool inGame => gameManager != null;

	[Header("Object References")]
	public UIManager uiManager;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	private List<XmlElement> GetDeltaElements(XmlElement element)
	{
		return element
			.GetElementsByTagName("delta")
			.OfType<XmlElement>()
			.ToList();
	}

	private IEnumerator Start()
	{
		Task connect = client.Connect();
		yield return new WaitUntil(() => connect.IsCompleted);

		Task joinMatch = client.JoinMatch(deckId: "TEST Undergrowth Smasher");
		yield return new WaitUntil(() => joinMatch.IsCompleted);

		Task<XmlDocument> matchStart = client.ReceiveDocument();
		yield return new WaitUntil(() => matchStart.IsCompleted);
		InitializeGameState(matchStart.Result);

		while (true) // TODO: Detect game end?
		{
			Task<XmlDocument> turnStart = client.ReceiveDocument();
			yield return new WaitUntil(() => turnStart.IsCompleted);
			yield return StartCoroutine(ProcessTurnStart(turnStart.Result));

			if (gameManager.Players[sideIndex].DeployPhases > 0)
			{
				yield return uiManager.WaitForMainButtonClick();
				client.SendPlayerActions(uiManager.myHandManager.ExportActions());
				uiManager.WaitForOpponent();
				Task<XmlDocument> confirmAction = client.ReceiveDocument();
				yield return new WaitUntil(() => confirmAction.IsCompleted);
				ProcessActionResults(confirmAction.Result);
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
		string type = document.DocumentElement.GetAttribute("type");

		if (type != "actionDeltas")
		{
			throw new Exception($"Received unexpected response type {type} when waiting for actionDeltas!");
		}

		List<XmlElement> elements = GetDeltaElements(document.DocumentElement);

		Debug.Log($"Processing {elements.Count} action deltas");

		foreach (XmlElement element in elements)
		{
			Delta delta = Delta.FromXml(element, cardLoader);
			delta.Apply();
		}

		uiManager.RenderUnits();
		uiManager.LockUnits();
	}

	private IEnumerator ProcessTurnStart(XmlDocument document)
	{
		string type = document.DocumentElement.GetAttribute("type");

		if (type != "turnStart")
		{
			throw new Exception($"Received unexpected response type {type} when waiting for turnStart!");
		}

		List<XmlElement> phaseElements = document
			.DocumentElement
			.GetElementsByTagName("phase")
			.OfType<XmlElement>()
			.ToList();

		foreach (TurnPhase phase in phaseElements.Select(element => new TurnPhase(element, cardLoader)))
		{
			if (phase.Deltas.Count == 0) continue;

			string phaseDisplayName = GetPhaseName(phase.Name);
			if (phaseDisplayName != null) yield return uiManager.ShowPhaseName(phaseDisplayName);

			Debug.Log($"Processing {phase.Deltas.Count} deltas of phase {phase.Name}");
			foreach (Delta delta in phase.Deltas)
			{
				delta.Apply();
				yield return StartCoroutine(AnimateDelta(delta));
			}
		}
	}

	private IEnumerator AnimateDelta(Delta delta)
	{
		if (delta is AddToHandDelta)
		{
			yield return uiManager.DrawPhase();
		}
		else
		{
			Debug.LogWarning($"Failed to animate delta of type {delta.GetType().Name}");
		}
		uiManager.RenderTowers();
		uiManager.RenderUnits();
	}

	private string GetPhaseName(string phaseCodeName)
	{
		switch (phaseCodeName)
		{
			case "endDeploy":
				return "End Deploy";
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
					return "???";
				}
		}
	}
}

public enum Phase
{
	DRAW, PLACEMENT, COMBAT, DONE
}