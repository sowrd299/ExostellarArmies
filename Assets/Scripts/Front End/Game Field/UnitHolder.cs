using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using SFB.Game;
using SFB.Game.Management;
using SFB.Net.Client;

public class UnitHolder : MonoBehaviour
{
	private GameManager gameManager => Driver.instance.gameManager;

	[Header("Data")]
	public int laneIndex;
	public int positionIndex;

	[Header("Object References")]
	public GameObject unitPrefab;
	public UnitManager unitManager;
	public HandManager handManager;
	public GameObject hoverParent;

	[HideInInspector]
	public PlayUnitCardAction playAction;

	private void Start()
	{
		unitManager.unitHolders[laneIndex, positionIndex] = this;
	}

	private PlayUnitCardAction CreatePlayCardAction(UnitCard card)
	{
		return new PlayUnitCardAction(
			card,
			Driver.instance.gameManager.Lanes[laneIndex],
			Driver.instance.sideIndex,
			positionIndex
		);
	}

	private GameObject InstantiateUnit(Unit unit)
	{
		GameObject unitObject = InstantiateUnit(unit.Card);
		unitObject.GetComponent<UnitUI>().unit = unit;
		return unitObject;
	}

	public GameObject InstantiateUnit(UnitCard cardData)
	{
		GameObject unitObject = Instantiate(unitPrefab, transform);

		UnitUI unitUI = unitObject.GetComponent<UnitUI>();
		unitUI.cardData = cardData;

		HoverCardOverlay cardHover = unitObject.GetComponent<HoverCardOverlay>();
		cardHover.hoverParent = hoverParent;

		return unitObject;
	}

	public void OnCardDrop(DragSource source)
	{
		CardUI sourceCardUI = source.GetComponent<CardUI>();
		InstantiateUnit(sourceCardUI.cardData as UnitCard);

		playAction = CreatePlayCardAction(sourceCardUI.cardData as UnitCard);
		handManager.AddPlayAction(playAction);

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

		handManager.RemovePlayAction(originalHolder.playAction);
		originalHolder.playAction = null;
		playAction = CreatePlayCardAction(source.GetComponent<UnitUI>().cardData);
		handManager.AddPlayAction(playAction);

		foreach (DropTarget target in GetComponents<DropTarget>())
		{
			target.enabled = false;
		}
		foreach (DropTarget target in originalHolder.GetComponents<DropTarget>())
		{
			target.enabled = true;
		}
	}

	public void RenderUnit()
	{
		Unit unit = gameManager.Lanes[laneIndex].Units[unitManager.sideIndex, positionIndex];

		if (unit == null)
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
			foreach (DropTarget target in GetComponents<DropTarget>())
			{
				target.enabled = true;
			}
		}
		else
		{
			Transform transform = base.transform;
			if (transform.childCount == 0)
			{
				InstantiateUnit(unit);
			}
			else
			{
				GameObject unitObject = transform.GetChild(0).gameObject;
				unitObject.GetComponent<UnitUI>().RenderUnit();
			}
		}
	}

	public void LockUnit()
	{
		Transform transform = base.transform;
		if (transform.childCount > 0)
		{
			GameObject childObject = transform.GetChild(0).gameObject;
			foreach (DragSource source in childObject.GetComponents<DragSource>())
			{
				source.enabled = false;
			}

			Unit unit = gameManager.Lanes[laneIndex].Units[unitManager.sideIndex, positionIndex];
			childObject.GetComponent<UnitUI>().unit = unit;
		}
	}
}
