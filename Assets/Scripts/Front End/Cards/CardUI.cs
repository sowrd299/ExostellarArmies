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
	public Text flavourText;
	public Text cardCost;

	[Header("Unit Card UI References")]
	public Text rangedDamage;
	public Text meleeDamage;
	public Text health;

	private void Start()
	{
		RenderCard();
	}

	public void RenderCard()
	{
		background.sprite = factions[cardData.Faction].cardBackground;
		cardName.text = cardData.Name;
		description.text = cardData.MainText;
		flavourText.text = cardData.FlavorText;
		cardCost.text = cardData.DeployCost.ToString();

		if (cardData is UnitCard)
		{
			UnitCard unitCardData = cardData as UnitCard;

			rangedDamage.text = unitCardData.RangedAttack.ToString();
			meleeDamage.text = unitCardData.MeleeAttack.ToString();
			health.text = unitCardData.HealthPoints.ToString();
		}
	}
}
