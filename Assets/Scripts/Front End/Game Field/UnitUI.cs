﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;

public class UnitUI : MonoBehaviour, IHasCard
{
	[HideInInspector]
	public UnitCard cardData;
	public Card card => cardData;
	private Unit _unit;
	public Unit unit
	{
		get { return _unit ?? (_unit = new Unit(cardData, Driver.instance.gameManager)); }
		set { _unit = value; }
	}

	[Header("Asset References")]
	public FactionRegistry factions;
	public GameObject damageTextPrefab;

	[Header("UI References")]
	public SVGImage frame;
	public SVGImage background;
	public Text unitCost;
	public Text rangedDamage;
	public Text meleeDamage;
	public Text health;
	public ParticleSystem healParticles;

	[Header("Animation Config")]
	public AnimationCurve attackMoveCurve;
	public float attackMoveMagnitude;
	public float attackDuration;
	public float damageTextMoveMagnitude;
	public float damageTextDuration;

	private void Start()
	{
		RenderUnit();
	}

	public void RenderUnit()
	{
		frame.sprite = factions[unit.Card.Faction].unitFrame;
		background.sprite = factions[unit.Card.Faction].cardBackground;
		unitCost.text = unit.Card.DeployCost.ToString();
		rangedDamage.text = unit.RangedAttack.ToString();
		meleeDamage.text = unit.MeleeAttack.ToString();
		health.text = unit.HealthPoints.ToString();
	}

	public Coroutine AttackMove(Vector3 direction)
	{
		return StartCoroutine(AnimateAttackMove(direction));
	}

	private IEnumerator AnimateAttackMove(Vector3 direction)
	{
		Vector3 originalPosition = transform.position;
		Vector3 attackMove = direction * attackMoveMagnitude;

		yield return UIManager.instance.LerpTime(
			Vector3.Lerp,
			transform.position,
			transform.position+attackMove,
			attackDuration,
			attackMoveCurve.Evaluate,
			position => transform.position = position
		);

		transform.position = originalPosition;
	}

	public Coroutine HealEffect()
	{
		return StartCoroutine(AnimateHealEffect());
	}

	private IEnumerator AnimateHealEffect()
	{
		healParticles.Play();
		yield return new WaitUntil(() => healParticles.isStopped);
	}
}
