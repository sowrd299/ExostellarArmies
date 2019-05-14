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

	[Header("Asset References")]
	public GameObject attackPrefab;

	[Header("Object References")]
	public TowerManager manager;

	[Header("UI References")]
	public Text health;

	[Header("Animation Config")]
	public AnimationCurve attackMoveCurve;
	public float attackDuration;
	public float respawnDuration;

	private void Start()
	{
		manager.towerUIs[laneIndex] = this;
	}

	public void RenderTower()
	{
		health.text = tower.HP.ToString();
	}

	public Coroutine Attack(Vector3 targetPosition)
	{
		return StartCoroutine(AnimateAttack(targetPosition));
	}

	private IEnumerator AnimateAttack(Vector3 targetPosition)
	{
		GameObject attackParticle = Instantiate(attackPrefab, transform);
		Transform attackTransform = attackParticle.transform;

		yield return UIManager.instance.LerpTime(
			Vector3.Lerp,
			transform.position,
			targetPosition,
			attackDuration,
			position => attackTransform.position = position
		);

		Destroy(attackParticle);
	}
}
