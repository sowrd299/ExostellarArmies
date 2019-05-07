using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game.Management;

public class TowerManager : MonoBehaviour
{
	private static GameManager gameManager => Driver.instance.gameManager;
		
	[HideInInspector]
	public int sideIndex;

	[HideInInspector]
	public TowerUI[] towerUIs;

	private void Awake()
	{
		towerUIs = new TowerUI[3];
	}

	public void RenderTowers()
	{
		foreach (TowerUI towerUI in towerUIs)
		{
			towerUI.RenderTower();
		}
	}
}
