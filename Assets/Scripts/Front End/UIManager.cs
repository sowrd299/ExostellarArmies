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

public class UIManager : MonoBehaviour
{
	public static UIManager instance => Driver.instance.uiManager;

	// Convenience accessors
	private static Player[] players => Driver.instance.gameManager.Players;
	private static GameManager gameManager => Driver.instance.gameManager;
	private static int myIndex => Driver.instance.sideIndex;
	private static int enemyIndex => 1 - Driver.instance.sideIndex;
	private static Player myPlayer => gameManager.Players[myIndex];

	[Header("Object References")]
	public HandManager myHandManager;
	public HandManager enemyHandManager;

	public UnitManager myUnitManager;
	public UnitManager enemyUnitManager;

	public TowerManager myTowerManager;
	public TowerManager enemyTowerManager;

	[Header("UI References")]
	public Button mainButton;
	public Text mainButtonText;

	public Image phaseBackground;
	public Text phaseText;

	[Header("Animation Config")]
	[Range(0, 1)]
	public float maxPhaseOverlayOpacity;
	public float phaseFadeTime;
	public float phaseDisplayTime;

	public void InitializeUI()
	{
		mainButton.interactable = true;

		myUnitManager.sideIndex = myIndex;
		enemyUnitManager.sideIndex = enemyIndex;

		myTowerManager.sideIndex = myIndex;
		enemyTowerManager.sideIndex = enemyIndex;
		RenderTowers();

		phaseBackground.CrossFadeAlpha(0, 0, false);
		phaseText.CrossFadeAlpha(0, 0, false);
	}

	public Coroutine DrawCard(Card card)
	{
		return StartCoroutine(AnimateDrawCard(card));
	}

	private IEnumerator AnimateDrawCard(Card card)
	{
		mainButtonText.text = "DRAWING...";
		mainButton.interactable = false;

		yield return myHandManager.DrawCard(card);

		mainButtonText.text = "LOCK IN PLANS";
		mainButton.interactable = true;
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

	public IEnumerator WaitForMainButtonClick()
	{
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
		mainButton.GetComponent<Image>().color = new Color(153, 204, 255);
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
		// phaseBackground.gameObject.SetActive(true);

		phaseBackground.CrossFadeAlpha(maxPhaseOverlayOpacity, phaseFadeTime, false);
		phaseText.CrossFadeAlpha(1, phaseFadeTime, false);
		yield return new WaitForSeconds(phaseFadeTime);

		yield return new WaitForSeconds(phaseDisplayTime);

		phaseBackground.CrossFadeAlpha(0, phaseFadeTime, false);
		phaseText.CrossFadeAlpha(0, phaseFadeTime, false);
		yield return new WaitForSeconds(phaseFadeTime);

		// phaseBackground.gameObject.SetActive(false);
	}
}
