using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class attached to Card Prefab, maps the card element to its value
[CreateAssetMenu(menuName = "Data/Card Property Map")]
public class CardPropertyMap : ScriptableObject
{
	public CardProperty[] properties;

	private Dictionary<Element, CardProperty> _propertyMap;
	private Dictionary<Element, CardProperty> propertyMap =>
		_propertyMap ?? (_propertyMap = properties.ToDictionary(property => property.element, property => property));

	public CardPropertyMap(CardProperty[] cardProp)
	{
		this.properties = cardProp;
	}

	public CardProperty GetProperty(Element e)
	{
		return propertyMap.TryGetValue(e, out CardProperty result) ? result : null;
	}
}
