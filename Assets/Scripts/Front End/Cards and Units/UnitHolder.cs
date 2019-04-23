using System.Collections;
using System.Collections.Generic;
using System.Xml;
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

	private PlayUnitCardAction CreatePlayCardAction(UnitCard card)
	{
		return new PlayUnitCardAction(
			card,
			Driver.instance.gameManager.Lanes[laneIndex],
			Client.instance.sideIndex,
			positionIndex
		);
	}

	public void OnCardDrop(DragSource source)
	{
		GameObject unit = Instantiate(unitPrefab, transform);
		CardUI unitUI = unit.GetComponent<CardUI>();
		CardUI cardUI = source.GetComponent<CardUI>();
		unitUI.LoadCard(cardUI.cardData);
		unitUI.cardBackEnd = cardUI.cardBackEnd;

		playAction = CreatePlayCardAction(cardUI.cardBackEnd as UnitCard);
		hand.AddPlayAction(playAction);

		Destroy(source.gameObject);
	}

	public void OnUnitDrop(DragSource source)
	{
		UnitHolder originalHolder = source.originalParent.GetComponent<UnitHolder>();
		if (originalHolder == this) return;

		source.transform.SetParent(transform);
		source.transform.position = transform.position;

		hand.RemovePlayAction(originalHolder.playAction);
		originalHolder.playAction = null;
		playAction = CreatePlayCardAction(source.GetComponent<CardUI>().cardBackEnd as UnitCard);
		hand.AddPlayAction(playAction);
	}
}
