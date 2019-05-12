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
		Vector3 startPosition = attackTransform.position = transform.position;

		float startTime = Time.time;
		while (Time.time - startTime < attackDuration)
		{
			attackTransform.position = Vector3.Lerp(
				startPosition,
				targetPosition,
				attackMoveCurve.Evaluate((Time.time - startTime) /attackDuration)
			);
			yield return null;
		}

		Destroy(attackParticle);
	}
}
