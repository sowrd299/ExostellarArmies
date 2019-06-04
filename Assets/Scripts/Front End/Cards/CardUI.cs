using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;

public class CardUI : MonoBehaviour, IHasCard
{
	[HideInInspector]
	public Card cardData;
	public Card card => cardData;

	[Header("Asset References")]
	public FactionRegistry factions;


	[Header("Card UI References")]
	public SVGImage background;
	public Text cardName;
	public Text description;
	public Text unitType;
	public Text flavourText;
	public Text cardCost;
	public GameObject cover;

	[Header("Unit Card UI References")]
	public Text rangedDamage;
	public Text meleeDamage;
	public Text health;

	[Header("Config")]
	public float revealDuration;
	public AnimationCurve revealCurve;

	private void Start()
	{
		RenderCard();
	}

	public void RenderCard()
	{
		background.sprite = factions[cardData.Faction].cardBackground;
		cardName.text = cardData.Name;
		string mainText = (cardData as UnitCard)?.MainText;
		description.text = (mainText == null ? cardData.MainText : mainText);
		cardCost.text = cardData.DeployCost.ToString();
		if (cardData.FlavorText != "")
		{
			flavourText.text = cardData.FlavorText;
		}
		else
		{
			flavourText.gameObject.SetActive(false);
		}

		if (cardData is UnitCard)
		{
			UnitCard unitCardData = cardData as UnitCard;

			unitType.text = unitCardData.UnitType;
			rangedDamage.text = unitCardData.RangedAttack.ToString();
			meleeDamage.text = unitCardData.MeleeAttack.ToString();
			health.text = unitCardData.HealthPoints.ToString();
		}
	}

	public Coroutine RevealCard(Card card)
	{
		cardData = card;

		return StartCoroutine(AnimateRevealCard());
	}

	private IEnumerator AnimateRevealCard()
	{
		Transform t = transform;
		Vector3 originalScale = t.localScale;

		yield return UIManager.instance.LerpTime(
			Vector3.Lerp, originalScale, Vector3.up,
			revealDuration / 2, revealCurve.Evaluate, scale => t.localScale = scale
		);

		cover.SetActive(false);
		RenderCard();

		yield return UIManager.instance.LerpTime(
			Vector3.Lerp, Vector3.up, originalScale,
			revealDuration / 2, time => 1 - revealCurve.Evaluate(1 - time), scale => t.localScale = scale
		);
	}
}
