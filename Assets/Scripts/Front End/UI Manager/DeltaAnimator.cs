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

	public Coroutine SpawnUnit(int sideIndex, int laneIndex, int positionIndex)
	{
		return unitManagers[sideIndex].unitHolders[laneIndex, positionIndex].SpawnUnit();
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

	public Coroutine UnitTowerDamage(Tower target, int damageAmount)
	{
		(int laneIndex, int sideIndex) = GetPositionIdentifier(target);
		TowerUI targetUI = towerManagers[sideIndex].towerUIs[laneIndex];
		UnitUI[] attackers = new UnitUI[] {
			FindUnitUI(gameManager.Lanes[laneIndex].Units[1-sideIndex, 0]),
			FindUnitUI(gameManager.Lanes[laneIndex].Units[1-sideIndex, 1])
		}.Where(unitUI => unitUI != null).ToArray();

		List<Func<Coroutine>> callbacks = new List<Func<Coroutine>>();
		callbacks.Add(() => damageTextManager.DamageTextPopup(
			targetUI.transform.position,
			$"-{damageAmount}"
		));
		callbacks.AddRange(
			attackers.Select<UnitUI, Func<Coroutine>>(attacker => () => attacker.AttackMove(AttackDirection(1 - sideIndex)))
		);

		targetUI.RenderTower();
		return StartCoroutine(ParallelCoroutine(callbacks.ToArray()));
	}

	public Coroutine TowerUnitDamage(Unit target, int damageAmount)
	{
		(int laneIndex, int sideIndex, int positionIndex) = GetPositionIdentifier(target);
		TowerUI sourceUI = towerManagers[1 - sideIndex].towerUIs[laneIndex];
		UnitUI targetUI = FindUnitUI(target);

		return StartCoroutine(SerialCoroutine(
			() => sourceUI.Attack(targetUI.transform.position),
			() => { targetUI.RenderUnit(); return null; },
			() => damageTextManager.DamageTextPopup(targetUI.transform.position, $"-{damageAmount}")
		));
	}

	public Coroutine TowerRespawn(Tower target)
	{
		(int laneIndex, int sideIndex) = GetPositionIdentifier(target);
		TowerUI targetUI = towerManagers[sideIndex].towerUIs[laneIndex];

		return targetUI.Respawn();
	}
}
