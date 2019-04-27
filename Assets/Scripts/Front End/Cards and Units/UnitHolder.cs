using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using SFB.Game;
using SFB.Game.Management;
using SFB.Net.Client;

public class UnitHolder : MonoBehaviour
{
	[Header("Indices")]
	public int laneIndex;
	public int positionIndex;

	[Header("Object references")]
	public GameObject unitPrefab;
	public HandManager hand;
	public GameObject hoverParent;

	[HideInInspector]
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

		CardHover cardHover = unit.GetComponent<CardHover>();
		cardHover.hoverParent = hoverParent;

		return unit;
	}

	public void OnCardDrop(DragSource source)
	{
		CardUI sourceCardUI = source.GetComponent<CardUI>();
		InstantiateUnit(sourceCardUI.cardBackEnd, sourceCardUI.cardData);

		playAction = CreatePlayCardAction(sourceCardUI.cardBackEnd as UnitCard);
		hand.AddPlayAction(playAction);

		Destroy(source.gameObject);
		UIManager.instance.ValidateDropCost();

		foreach (DropTarget target in GetComponents<DropTarget>())
		{
			target.enabled = false;
		}
	}

	public void OnUnitDrop(DragSource source)
	{
		UnitHolder originalHolder = source.originalParent.GetComponent<UnitHolder>();

		source.transform.SetParent(transform);
		source.transform.position = transform.position;

		hand.RemovePlayAction(originalHolder.playAction);
		originalHolder.playAction = null;
		playAction = CreatePlayCardAction(source.GetComponent<CardUI>().cardBackEnd as UnitCard);
		hand.AddPlayAction(playAction);

		foreach (DropTarget target in GetComponents<DropTarget>())
		{
			target.enabled = false;
		}
		foreach (DropTarget target in originalHolder.GetComponents<DropTarget>())
		{
			target.enabled = true;
		}
	}
}
