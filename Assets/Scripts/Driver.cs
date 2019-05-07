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

	private List<XmlElement> GetDeltaElements(XmlDocument document)
	{
		return document
			.GetElementsByTagName("delta")
			.OfType<XmlElement>()
			.ToList();
	}

	private IEnumerator Start()
	{
		Task connect = client.Connect();
		yield return new WaitUntil(() => connect.IsCompleted);

		Task joinMatch = client.JoinMatch();
		yield return new WaitUntil(() => joinMatch.IsCompleted);

		Task<XmlDocument> matchStart = client.ReceiveDocument();
		yield return new WaitUntil(() => matchStart.IsCompleted);
		InitializeGameState(matchStart.Result);

		while (true) // TODO: Detect game end?
		{
			Task<XmlDocument> turnStart = client.ReceiveDocument();
			yield return new WaitUntil(() => turnStart.IsCompleted);
			ProcessTurnStart(turnStart.Result);

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

		List<XmlElement> elements = GetDeltaElements(document);

		Debug.Log($"Processing {elements.Count} action deltas");

		foreach (XmlElement element in elements)
		{
			Delta delta = Delta.FromXml(element, cardLoader);
			delta.Apply();
		}

		uiManager.RenderUnits();
		uiManager.LockUnits();
	}

	private void ProcessTurnStart(XmlDocument document)
	{
		string type = document.DocumentElement.GetAttribute("type");

		if (type != "turnStart")
		{
			throw new Exception($"Received unexpected response type {type} when waiting for turnStart!");
		}

		List<XmlElement> elements = GetDeltaElements(document);
		List<Tuple<XmlElement, Delta>> parsedDeltas = elements
					.Select(element => new Tuple<XmlElement, Delta>(element, Delta.FromXml(element, cardLoader)))
					.ToList();

		// X:		misc deploy phase stuff
		// R, M, T:	ranged, melee, tower
		// D:		draw
		List<Delta> deployDeltas = parsedDeltas
			.TakeWhile(pair => !IsCombatDelta(pair.Item2))
			.Select(pair => pair.Item2)
			.ToList();
		List<Tuple<XmlElement, Delta>> combatDeltas = parsedDeltas
			.Where(pair => IsCombatDelta(pair.Item2))
			.ToList();
		List<Delta> rangedCombatDeltas = combatDeltas
			.Where(pair => pair.Item1.GetAttribute("dmgType") == "R")
			.Select(pair => pair.Item2)
			.ToList();
		List<Delta> meleeCombatDeltas = combatDeltas
			.Where(pair => pair.Item1.GetAttribute("dmgType") == "M")
			.Select(pair => pair.Item2)
			.ToList();
		List<Delta> towerCombatDeltas = combatDeltas
			.Where(pair => pair.Item1.GetAttribute("dmgType") == "T")
			.Select(pair => pair.Item2)
			.ToList();
		List<Delta> drawDeltas = parsedDeltas
			.SkipWhile(pair => !IsCombatDelta(pair.Item2))
			.SkipWhile(pair => IsCombatDelta(pair.Item2))
			.Select(pair => pair.Item2)
			.ToList();

		ProcessDeployDeltas(deployDeltas);
		ProcessRangedCombatDeltas(rangedCombatDeltas);
		ProcessMeleeCombatDeltas(meleeCombatDeltas);
		ProcessTowerCombatDeltas(towerCombatDeltas);
		StartCoroutine(ProcessDrawDeltas(drawDeltas));
	}

	private bool IsCombatDelta(Delta delta)
	{
		return delta is UnitDelta || delta is TowerDelta;
	}

	private void ProcessDeployDeltas(List<Delta> deployDeltas)
	{
		Debug.Log($"Processing {deployDeltas.Count} deploy deltas.");
		foreach (Delta delta in deployDeltas)
		{
			delta.Apply();
		}
		uiManager.RenderUnits();
		uiManager.LockUnits();
	}

	private void ProcessRangedCombatDeltas(List<Delta> rangedCombatDeltas)
	{
		Debug.Log($"Processing {rangedCombatDeltas.Count} ranged combat deltas.");
		foreach (Delta delta in rangedCombatDeltas)
		{
			delta.Apply();
		}
		AfterEachCombatPhase();
	}

	private void ProcessMeleeCombatDeltas(List<Delta> meleeCombatDeltas)
	{
		Debug.Log($"Processing {meleeCombatDeltas.Count} melee combat deltas.");
		foreach (Delta delta in meleeCombatDeltas)
		{
			delta.Apply();
		}
		AfterEachCombatPhase();
	}

	private void ProcessTowerCombatDeltas(List<Delta> towerCombatDeltas)
	{
		Debug.Log($"Processing {towerCombatDeltas.Count} tower combat deltas.");
		foreach (Delta delta in towerCombatDeltas)
		{
			delta.Apply();
		}
		AfterEachCombatPhase();
	}

	private IEnumerator ProcessDrawDeltas(List<Delta> drawDeltas)
	{
		Debug.Log($"Processing {drawDeltas.Count} draw deltas.");
		foreach (Delta delta in drawDeltas)
		{
			delta.Apply();
		}
		yield return uiManager.DrawPhase();
	}

	private void AfterEachCombatPhase()
	{
		gameManager.cleanUp();
		uiManager.RenderUnits();
		uiManager.RenderTowers();
	}
}

public enum Phase
{
	DRAW, PLACEMENT, COMBAT, DONE
}