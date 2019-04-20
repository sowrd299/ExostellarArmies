using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game;
using SFB.Game.Management;
using SFB.Net.Client;

public class UnitHolder : MonoBehaviour
{
	public int laneIndex;
	public int positionIndex;

	public GameObject unitPrefab;

	public HandManager hand;

	private PlayUnitCardAction playAction;

	public void OnCardDrop(DragSource source)
	{
		GameObject unit = Instantiate(unitPrefab, transform);
		unit.GetComponent<CardUI>().LoadCard(source.GetComponent<CardUI>().cardData);

		playAction = new PlayUnitCardAction(
			unit.GetComponent<CardUI>().cardBackEnd as UnitCard,
			Driver.instance.gameManager.Lanes[laneIndex],
			Client.Instance.SideIndex,
			positionIndex
		);
		hand.AddPlayAction(playAction);

		Destroy(source.gameObject);
	}

	public void OnUnitDrop(DragSource source)
	{

	}
}
