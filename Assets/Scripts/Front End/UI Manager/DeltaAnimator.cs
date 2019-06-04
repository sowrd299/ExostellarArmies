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
		yield return myHandManager.DrawCard(card);
	}

	public void UpdateDiscardDisplay(int sideIndex)
	{
		discardManagers[sideIndex].RenderDiscardCount();
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
			() => sourceUI.AttackMove(AttackDirection(Lane.GetLaneSidePosOf(source, Driver.instance.gameManager.Lanes).Item2)),
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

	public Coroutine UnitHeal(Unit source, Unit target)
	{
		UnitUI sourceUI = FindUnitUI(source);
		UnitUI targetUI = FindUnitUI(target);

		return StartCoroutine(ParallelCoroutine(
			() => sourceUI.AttackMove((targetUI.transform.position - sourceUI.transform.position).normalized),
			() => targetUI.HealEffect()
		));
	}

	public Coroutine RemoveUnit(int laneIndex, int sideIndex, int positionIndex)
	{
		UnitHolder holder = unitManagers[sideIndex].unitHolders[laneIndex, positionIndex];
		return holder.RemoveUnit();
	}

	public Coroutine RevealOpponentCard(Card card)
	{
		return enemyHandManager.RevealCard(card);
	}

	public Coroutine TowerRespawn(Tower target)
	{
		(int laneIndex, int sideIndex) = GetPositionIdentifier(target);
		TowerUI targetUI = towerManagers[sideIndex].towerUIs[laneIndex];

		return targetUI.Respawn();
	}

	public Coroutine UpdateResourceDisplay(int sideIndex)
	{
		return StartCoroutine(AnimateUpdateResourceDisplay(sideIndex));
	}

	private IEnumerator AnimateUpdateResourceDisplay(int sideIndex)
	{
		bool finished = false;
		resourceManagers[sideIndex].UpdateResourceDisplay(() => finished = true);

		yield return new WaitUntil(() => finished);
	}
}
