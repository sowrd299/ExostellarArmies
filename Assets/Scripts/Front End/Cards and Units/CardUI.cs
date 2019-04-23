using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Net.Client;

public class CardUI : MonoBehaviour
{
	//public Card cardBackEnd;
	public CardPropertyMap cardData;
	public CardUIProperty[] propertyDisplays;
	public Element hpElement;

    public Card cardBackEnd;
	private Dictionary<Element, CardUIProperty> _propertyDisplayMap;
	private Dictionary<Element, CardUIProperty> propertyDisplayMap =>
		_propertyDisplayMap ?? (_propertyDisplayMap = propertyDisplays.ToDictionary(property => property.element, property => property));

	private List<CardPropertyMap> playerFrontEnds = new List<CardPropertyMap>();
	private List<CardPropertyMap> enemyFrontEnds = new List<CardPropertyMap>();

	public int handIndex
	{
		get
		{
			if (!this.gameObject.name.Contains("Enemy"))
				return Driver.instance.gameManager.Players[Client.instance.sideIndex].Hand.IndexOf(cardBackEnd);
			else
				return Driver.instance.gameManager.Players[Mathf.Abs(Client.instance.sideIndex - 1)].Hand.IndexOf(cardBackEnd);
		}
	}
	public bool Old = false;
	private int hp;

	private void Start()
	{
		if (cardData != null)
			LoadCard(cardData);
	}
   
	//Updates all of the UI properties to the values in c
	public void LoadCard(CardPropertyMap card)
	{
		if (card == null) return;

		cardData = card;

		for (int i = 0; i < card.properties.Length; i++)
		{
			CardProperty cardProperty = card.properties[i];
			CardUIProperty uiProperty = GetProperty(cardProperty.element);
			if (uiProperty == null) continue;

			if (cardProperty.element is ElementInt)
			{
				uiProperty.text.text = cardProperty.intValue.ToString();
			}
			else if (cardProperty.element is ElementText)
			{
				uiProperty.text.text = cardProperty.stringValue;
			}
			else if (cardProperty.element is ElementImage)
			{
				uiProperty.image.sprite = cardProperty.sprite;
			}
		}
	}

	public void loadHp(CardPropertyMap card)
	{
		if (card == null)
			return;
		for (int i = 0; i < card.properties.Length; i++)
		{
			CardProperty cardProperty = card.properties[i];
			CardUIProperty uiProperty = GetProperty(cardProperty.element);
			if (cardProperty.element is ElementInt)
			{
				//Debug.Log(p.GetType());
				uiProperty.text.text = cardProperty.intValue.ToString();
			}
		}
	}

	//searches untill Element type matches
	public CardUIProperty GetProperty(Element e)
	{
		return propertyDisplayMap.TryGetValue(e, out CardUIProperty result) ? result : null;
	}
}
