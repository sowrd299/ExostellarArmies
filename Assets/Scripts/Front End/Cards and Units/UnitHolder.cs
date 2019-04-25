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

	public PlayUnitCardAction playAction;

	private PlayUnitCardAction CreatePlayCardAction(UnitCard card)
	{
		return new PlayUnitCardAction(
			card,
			Driver.instance.gameManager.Lanes[laneIndex],
			Client.instance.sideIndex,
			positionIndex
		);
	}

	public GameObject InstantiateUnit(Card cardBackEnd, CardPropertyMap cardData)
	{
		GameObject unit = Instantiate(unitPrefab, transform);
		CardUI unitUI = unit.GetComponent<CardUI>();

		unitUI.LoadCard(cardData);
		unitUI.cardBackEnd = cardBackEnd;

		return unit;
	}

	public void OnCardDrop(DragSource source)
	{
		CardUI sourceCardUI = source.GetComponent<CardUI>();
		InstantiateUnit(sourceCardUI.cardBackEnd, sourceCardUI.cardData);

		playAction = CreatePlayCardAction(sourceCardUI.cardBackEnd as UnitCard);
		hand.AddPlayAction(playAction);

		Destroy(source.gameObject);
		Manager.instance.ValidateDropCost();
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
