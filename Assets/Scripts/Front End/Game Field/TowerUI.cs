using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game.Content;
using SFB.Game.Management;

public class TowerUI : MonoBehaviour
{
	private GameManager gameManager => Driver.instance.gameManager;
	private Tower tower => gameManager.Lanes[laneIndex].Towers[manager.sideIndex];
	[Header("Data")]
	public int laneIndex;

	[Header("Object References")]
	public TowerManager manager;

	[Header("UI References")]
	public Text health;

	private void Start()
	{
		manager.towerUIs[laneIndex] = this;
	}

	public void RenderTower()
	{
		health.text = tower.HP.ToString();
	}
}
