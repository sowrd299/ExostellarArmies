using System.Collections;
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
		get { return _unit ?? (_unit = new Unit(cardData, Driver.instance.gameManager.GameState)); }
		set { _unit = value; }
	}

	[Header("Asset References")]
	public FactionRegistry factions;
	public GameObject damageTextPrefab;

	[Header("UI References")]
	public SVGImage background;
	public Text unitCost;
	public Text rangedDamage;
	public Text meleeDamage;
	public Text health;

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
		background.sprite = factions[unit.Card.Faction].unitFrame;
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
		Vector3 attackMove = direction * attackMoveMagnitude;

		Vector3 originalPosition = transform.position;
		float startTime = Time.time;
		while (Time.time - startTime < attackDuration)
		{
			transform.position = originalPosition + attackMove * attackMoveCurve.Evaluate((Time.time - startTime) / attackDuration);
			yield return null;
		}

		transform.position = originalPosition;
	}

	public Coroutine TakeDamage(int amount)
	{
		return StartCoroutine(AnimateTakeDamage(amount));
	}

	private IEnumerator AnimateTakeDamage(int amount)
	{
		GameObject damageTextObject = Instantiate(damageTextPrefab, transform.parent);
		Text damageText = damageTextObject.GetComponent<Text>();
		damageText.text = $"-{amount}";
		damageText.CrossFadeAlpha(0.1f, damageTextDuration, false);

		RenderUnit();

		Vector3 startPostion = transform.position;
		Vector3 endPosition = startPostion + Vector3.up * damageTextMoveMagnitude;

		float startTime = Time.time;
		while (Time.time - startTime < damageTextDuration)
		{
			float deltaTime = Time.time - startTime;
			damageTextObject.transform.position = Vector3.Lerp(startPostion, endPosition, deltaTime / damageTextDuration);
			yield return null;
		}

		Destroy(damageTextObject);
	}
}
