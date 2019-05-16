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
		Coroutine[] coroutines = callbacks.Select(callback => callback()).ToArray();

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

	public Coroutine LerpTime<T>(Func<T, T, float, T> lerp, T initial, T final, float duration, Action<T> setter)
	{
		return LerpTime(lerp, initial, final, duration, time => time, setter);
	}

	public Coroutine LerpTime<T>(Func<T, T, float, T> lerp, T initial, T final, float duration, Func<float, float> evaluate, Action<T> setter)
	{
		return StartCoroutine(AnimateLerpTime(lerp, initial, final, duration, evaluate, setter));
	}

	private IEnumerator AnimateLerpTime<T>(Func<T, T, float, T> lerp, T initial, T final, float duration, Func<float, float> evaluate, Action<T> setter)
	{
		setter(initial);

		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			setter(lerp(initial, final, evaluate((Time.time - startTime) / duration)));
			yield return null;
		}
		
		setter(final);
	}
}
