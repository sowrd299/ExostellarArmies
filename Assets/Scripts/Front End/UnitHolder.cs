using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHolder : MonoBehaviour
{
	public int laneIndex;

	public GameObject unitPrefab;
	
	public void OnCardDrop(DragSource source)
	{
		GameObject unit = Instantiate(unitPrefab, transform);
		unit.GetComponent<CardUI>().LoadCard(source.GetComponent<CardUI>().cardData);
		Destroy(source.gameObject);
	}

	public void OnUnitDrop(DragSource source)
	{

	}
}
