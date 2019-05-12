using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFB.Game;
using SFB.Game.Content;

public partial class UIManager : MonoBehaviour
{
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

	public Coroutine UnitDamage(Unit source, Unit target, int damageAmount)
	{
		UnitUI sourceUI = FindUnitUI(source);
		UnitUI targetUI = FindUnitUI(target);
		targetUI.RenderUnit();

		return StartCoroutine(ParallelCoroutine(
			() => sourceUI.AttackMove(AttackDirection(gameManager.GetSidePosOf(source)[1])),
			() => damageTextManager.DamageTextPopup(targetUI.transform.position, $"-{damageAmount}")
		));
	}

	public Coroutine UnitTowerDamage(Tower tower, int damageAmount)
	{
		(int laneIndex, int sideIndex) = GetPositionIdentifier(tower);
		TowerUI targetUI = towerManagers[sideIndex].towerUIs[laneIndex];
		UnitUI[] attackers = new UnitUI[] {
			FindUnitUI(gameManager.Lanes[laneIndex].Units[1-sideIndex, 0]),
			FindUnitUI(gameManager.Lanes[laneIndex].Units[1-sideIndex, 1])
		}.Where(unitUI => unitUI != null).ToArray();

		List<Func<Coroutine>> coroutines = new List<Func<Coroutine>>();
		coroutines.Add(() => damageTextManager.DamageTextPopup(
			targetUI.transform.position,
			$"-{damageAmount}"
		));
		coroutines.AddRange(
			attackers.Select<UnitUI, Func<Coroutine>>(attacker => () => attacker.AttackMove(AttackDirection(1 - sideIndex)))
		);

		targetUI.RenderTower();
		return StartCoroutine(ParallelCoroutine(coroutines.ToArray()));
	}
}
