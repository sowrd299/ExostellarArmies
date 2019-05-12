using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFB.Game;
using SFB.Game.Content;

public partial class UIManager : MonoBehaviour
{
	public static IEnumerator ParallelCoroutine(params Func<Coroutine>[] callbacks)
	{
		IEnumerable<Coroutine> coroutines = callbacks.Select(callback => callback());

		foreach (Coroutine coroutine in coroutines)
		{
			yield return coroutine;
		}
	}

	public static IEnumerator SerialCoroutine(params Func<Coroutine>[] callbacks)
	{
		foreach (Func<Coroutine> callback in callbacks)
		{
			yield return callback();
		}
	}

	private UnitUI FindUnitUI(Unit unit)
	{
		(int laneIndex, int sideIndex, int positionIndex) = GetPositionIdentifier(unit);
		UnitManager unitManager = unitManagers[sideIndex];
		return unitManager.unitHolders[laneIndex, positionIndex].GetComponentInChildren<UnitUI>();
	}

	private (int, int, int) GetPositionIdentifier(Unit unit)
	{
		int[] sidePos = gameManager.GetSidePosOf(unit);
		return (sidePos[0], sidePos[1], sidePos[2]);
	}

	private Vector3 AttackDirection(int sideIndex)
	{
		return sideIndex == myIndex ? Vector3.up : Vector3.down;
	}

	private (int, int) GetPositionIdentifier(Tower tower)
	{
		for (int laneIndex = 0; laneIndex < 3; laneIndex++)
		{
			for (int sideIndex = 0; sideIndex < 2; sideIndex++)
			{
				if (gameManager.Lanes[laneIndex].Towers[sideIndex].ID == tower.ID)
				{
					return (laneIndex, sideIndex);
				}
			}
		}

		return (-1, -1);
	}
}
