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

	private Hand hand;
	private Queue<IEnumerator> drawAnimationQueue = new Queue<IEnumerator>();

	private HashSet<PlayUnitCardAction> playActions = new HashSet<PlayUnitCardAction>();

	public int deploymentCost => playActions.Sum(action => action.card.DeployCost);


	private void OnEnable()
	{
		if (hand != null)
		{
			hand.afterInsert += OnInsertCard;
		}
	}

	public void TrackHand(Hand hand)
	{
		if (this.hand != null)
		{
			throw new System.Exception($"HandManager already tracking a hand (id: {this.hand.ID}), cannot track another hand (id: {hand.ID})");
		}

		this.hand = hand;
		OnEnable();
	}

	private void OnInsertCard(int index, Card newCard)
	{
		drawAnimationQueue.Enqueue(AnimateMoveToHand(newCard));
	}

	private IEnumerator AnimateMoveToHand(Card newCard)
	{
		GameObject cardObject = CreateCardDisplay(newCard);

		Vector3 startPosition = drawOrigin.position;
		Vector3 targetPosition = cardObject.transform.parent.position;
		cardObject.transform.position = startPosition;

		float startTime = Time.time;
		while (Time.time - startTime < travelTime)
		{
			cardObject.transform.position = Vector3.Lerp(
				startPosition,
				targetPosition,
				travelCurve.Evaluate((Time.time - startTime) / travelTime)
			);
			yield return null;
		}
		cardObject.transform.position = targetPosition;

		yield return new WaitForSeconds(interval);
	}

	public Coroutine DrawCards()
	{
		return StartCoroutine(AnimateDrawCards());
	}

	public Coroutine DrawUnknownCards()
	{
		foreach (Transform child in transform)
		{
			if (child.childCount == 0)
			{
				drawAnimationQueue.Enqueue(AnimateMoveToHand(new UnknownCard()));
			}
		}
		return StartCoroutine(AnimateDrawCards());
	}

	private IEnumerator AnimateDrawCards()
	{
		while (drawAnimationQueue.Count > 0)
		{
			yield return StartCoroutine(drawAnimationQueue.Dequeue());
		}
	}

	private void OnDisable()
	{
		if (hand != null)
		{
			hand.afterInsert -= OnInsertCard;
		}
	}

	private GameObject CreateCardDisplay(Card cardData)
	{
		Transform cardHolder = GetNextAvailableCardHolder();
		GameObject cardObject = Instantiate(cardPrefab, cardHolder);

		CardUI cardUI = cardObject.GetComponent<CardUI>();
		cardUI.cardData = cardData;

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
