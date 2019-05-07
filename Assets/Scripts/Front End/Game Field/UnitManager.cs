using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game;
using SFB.Game.Management;

public class UnitManager : MonoBehaviour
{
	private GameManager gameManager => Driver.instance.gameManager;
	public UnitHolder[,] unitHolders;

	[HideInInspector]
	public int sideIndex;

	private void Awake()
	{
		unitHolders = new UnitHolder[3, 2];
	}

	public void RenderUnits()
	{
		foreach (UnitHolder unitHolder in unitHolders)
		{
			unitHolder.RenderUnit();
		}
	}

	public void LockUnits()
	{
		foreach (UnitHolder unitHolder in unitHolders)
		{
			unitHolder.LockUnit();
		}
	}
}
