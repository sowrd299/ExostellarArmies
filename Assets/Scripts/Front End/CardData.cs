using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class attached to Card Prefab, contains array of properties(all the texts,images,ints with their Element type)
[CreateAssetMenu(menuName = "Data/Card Data")]
public class CardData : ScriptableObject
{
	public CardProperty[] properties;

	private Dictionary<Element, CardProperty> _propertyMap;
	private Dictionary<Element, CardProperty> propertyMap =>
		_propertyMap ?? (_propertyMap = properties.ToDictionary(property => property.element, property => property));

	public CardData(CardProperty[] cardProp)
	{
		this.properties = cardProp;
	}

	public CardProperty GetProperty(Element e)
	{
		return propertyMap.TryGetValue(e, out CardProperty result) ? result : null;
	}
}
