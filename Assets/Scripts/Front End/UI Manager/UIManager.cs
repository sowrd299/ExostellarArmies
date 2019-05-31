using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using SFB.Game;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Net.Client;

public partial class UIManager : MonoBehaviour
{
	public static UIManager instance;

	// Convenience accessors
	private static Player[] players => Driver.instance.gameManager.Players;
	private static GameManager gameManager => Driver.instance.gameManager;
	private static int myIndex => Driver.instance.sideIndex;
	private static int enemyIndex => 1 - Driver.instance.sideIndex;
	private static Player myPlayer => gameManager.Players[myIndex];

	private HandManager[] handManagers => (
		myIndex == 0
		? new HandManager[] { myHandManager, enemyHandManager }
		: new HandManager[] { enemyHandManager, myHandManager }
	);

	private UnitManager[] unitManagers => (
		myIndex == 0
		? new UnitManager[] { myUnitManager, enemyUnitManager }
		: new UnitManager[] { enemyUnitManager, myUnitManager }
	);

	private TowerManager[] towerManagers => (
		myIndex == 0
		? new TowerManager[] { myTowerManager, enemyTowerManager }
		: new TowerManager[] { enemyTowerManager, myTowerManager }
	);

	private ResourceDisplayController[] resourceManagers => (
		myIndex == 0
		? new ResourceDisplayController[] { myResourceManager, enemyResourceManager }
		: new ResourceDisplayController[] { enemyResourceManager, myResourceManager }
	);

	[Header("Object References")]
	public HandManager myHandManager;
	public HandManager enemyHandManager;

	[Space]
	public UnitManager myUnitManager;
	public UnitManager enemyUnitManager;

	[Space]
	public TowerManager myTowerManager;
	public TowerManager enemyTowerManager;

	[Space]
	public ResourceDisplayController myResourceManager;
	public ResourceDisplayController enemyResourceManager;

	[Space]
	public DamageTextManager damageTextManager;

	[Header("UI References")]
	public Button mainButton;
	public Text mainButtonText;
	public GameObject extraDeploymentIndicator;

	public Image phaseBackground;
	public Text phaseText;

	[Header("Animation Config")]
	[Range(0, 1)]
	public float maxPhaseOverlayOpacity;
	public float phaseFadeTime;
	public float phaseDisplayTime;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	public void WaitForMatch()
	{
		phaseBackground.gameObject.SetActive(true);
		phaseText.gameObject.SetActive(true);
		phaseText.text = "Waiting For Match";
	}

	public void InitializeUI()
	{
		mainButton.interactable = false;

		myHandManager.SetCardDraggable(false);
		enemyHandManager.SetCardDraggable(false);

		myUnitManager.sideIndex = myIndex;
		enemyUnitManager.sideIndex = enemyIndex;

		myTowerManager.sideIndex = myIndex;
		enemyTowerManager.sideIndex = enemyIndex;
		RenderTowers();

		myResourceManager.sideIndex = myIndex;
		enemyResourceManager.sideIndex = enemyIndex;

		RenderIndicators();

		phaseBackground.CrossFadeAlpha(0, 0, false);
		phaseText.CrossFadeAlpha(0, 0, false);
	}

	public Coroutine OpponentDrawCards()
	{
		return enemyHandManager.DrawUnknownCards();
	}

	public void ValidateDropCost()
	{
		if (myPlayer.ManaPool.CanAfford(myHandManager.deploymentCost))
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

	public IEnumerator WaitForLockIn()
	{
		mainButtonText.text = "LOCK IN PLANS";
		mainButton.interactable = true;
		myHandManager.SetCardDraggable(true);

		bool clicked = false;
		UnityAction listener = null;
		listener = () =>
		{
			clicked = true;
			mainButton.onClick.RemoveListener(listener);
		};
		mainButton.onClick.AddListener(listener);
		yield return new WaitUntil(() => clicked);
	}

	public void WaitForOpponent()
	{
		LockUnits();

		mainButtonText.text = "WAITING FOR OPPONENT";
		mainButton.interactable = false;
		myHandManager.SetCardDraggable(false);
	}

	public void BeforeTurnStart()
	{
		mainButtonText.text = "PROCESSING TURN EVENTS";
	}

	public void RenderUnits()
	{
		myUnitManager.RenderUnits();
		enemyUnitManager.RenderUnits();
	}

	public void RenderTowers()
	{
		myTowerManager.RenderTowers();
		enemyTowerManager.RenderTowers();
	}

	public void RenderIndicators()
	{
		extraDeploymentIndicator.SetActive(myPlayer.DeployPhases > 1);
	}

	public void LockUnits()
	{
		myUnitManager.LockUnits();
		enemyUnitManager.LockUnits();
	}

	public Coroutine ShowPhaseName(string phaseName)
	{
		return StartCoroutine(AnimateShowPhaseName(phaseName));
	}

	private IEnumerator AnimateShowPhaseName(string phaseName)
	{
		phaseText.text = phaseName;

		phaseBackground.CrossFadeAlpha(maxPhaseOverlayOpacity, phaseFadeTime, false);
		phaseText.CrossFadeAlpha(1, phaseFadeTime, false);
		yield return new WaitForSeconds(phaseFadeTime);

		yield return new WaitForSeconds(phaseDisplayTime);

		phaseBackground.CrossFadeAlpha(0, phaseFadeTime, false);
		phaseText.CrossFadeAlpha(0, phaseFadeTime, false);
		yield return new WaitForSeconds(phaseFadeTime);
	}
}
