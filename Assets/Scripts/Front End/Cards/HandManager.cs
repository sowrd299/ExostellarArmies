using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Game;

public class HandManager : MonoBehaviour
{
	[Header("Object References")]
	public GameObject cardPrefab;

	public GameObject hoverParent;

	[Header("Draw Animation")]
	public Transform drawOrigin;
	public float travelTime;
	public AnimationCurve travelCurve;
	public float interval;

	private HashSet<PlayUnitCardAction> playActions = new HashSet<PlayUnitCardAction>();

	public int deploymentCost => playActions.Sum(action => action.card.DeployCost);

	public bool cardDraggable { get; private set; }


	public Coroutine DrawCard(Card card)
	{
		return StartCoroutine(AnimateDrawCard(card));
	}

	private IEnumerator AnimateDrawCard(Card newCard)
	{
		GameObject cardObject = CreateCardDisplay(newCard);

		Vector3 startPosition = drawOrigin.position;
		Vector3 targetPosition = cardObject.transform.parent.position;
		cardObject.transform.position = startPosition;

		yield return UIManager.instance.LerpTime(
			Vector3.Lerp,
			startPosition,
			targetPosition,
			travelTime,
			travelCurve.Evaluate,
			position => cardObject.transform.position = position
		);

		yield return new WaitForSeconds(interval);
	}

	public Coroutine DrawUnknownCards()
	{
		return StartCoroutine(AnimateDrawUnknownCards());
	}

	private IEnumerator AnimateDrawUnknownCards()
	{
		foreach (Transform child in transform)
		{
			if (child.childCount == 0)
			{
				yield return StartCoroutine(AnimateDrawCard(new UnknownCard()));
			}
		}
	}

	public void SetCardDraggable(bool draggable)
	{
		cardDraggable = draggable;

		foreach (Transform child in transform)
		{
			if (child.childCount > 0)
			{
				child.GetChild(0).GetComponent<DragSource>().enabled = draggable;
			}
		}
	}

	private GameObject CreateCardDisplay(Card cardData)
	{
		Transform cardHolder = GetNextAvailableCardHolder();
		GameObject cardObject = Instantiate(cardPrefab, cardHolder);

		CardUI cardUI = cardObject.GetComponent<CardUI>();
		cardUI.cardData = cardData;

		DragSource dragSource = cardObject.GetComponent<DragSource>();
		if (dragSource != null)
		{
			dragSource.enabled = cardDraggable;
		}

		HoverCardOverlay cardHover = cardObject.GetComponent<HoverCardOverlay>();
		if (cardHover != null)
		{
			cardHover.hoverParent = hoverParent;
		}

		return cardObject;
	}

	public void OnUnitDrop(DragSource source)
	{
		UnitHolder originalHolder = source.originalParent.GetComponent<UnitHolder>();

		RemovePlayAction(originalHolder.playAction);
		originalHolder.playAction = null;

		GameObject cardObject = CreateCardDisplay(source.GetComponent<UnitUI>().cardData);
		cardObject.transform.position = cardObject.transform.parent.position;

		foreach (DropTarget target in originalHolder.GetComponents<DropTarget>())
		{
			target.enabled = true;
		}

		Destroy(source.gameObject);
		UIManager.instance.ValidateDropCost();
	}

	public int GetCardCount()
	{
		int count = 0;

		foreach (Transform child in transform)
		{
			if (child.childCount > 0) count++;
		}

		return count;
	}

	private Transform GetNextAvailableCardHolder()
	{
		foreach (Transform child in transform)
		{
			if (child.childCount == 0) return child;
		}
		return null;
	}

	// TODO: Should probably fork the play action related functions into a separate manager.
	public void AddPlayAction(PlayUnitCardAction action)
	{
		playActions.Add(action);
	}

	public void RemovePlayAction(PlayUnitCardAction action)
	{
		playActions.Remove(action);
	}

	public PlayUnitCardAction[] ExportActions()
	{
		Debug.Log($"Exporting {playActions.Count} play actions");
		PlayUnitCardAction[] exports = new PlayUnitCardAction[playActions.Count];
		playActions.CopyTo(exports);
		playActions.Clear();
		return exports;
	}
}
