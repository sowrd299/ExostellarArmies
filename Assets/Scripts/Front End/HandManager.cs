using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game.Management;

public class HandManager : MonoBehaviour
{
	[Header("Draw Animation")]
	public Transform drawOrigin;
	public float travelTime;
	public AnimationCurve travelCurve;
	public float interval;

	private HashSet<PlayUnitCardAction> playActions = new HashSet<PlayUnitCardAction>();

	public Coroutine MoveToHand(List<GameObject> cards)
	{
		return StartCoroutine(AnimateMoveToHand(cards));
	}

	private IEnumerator AnimateMoveToHand(List<GameObject> cards)
	{
		Debug.Log($"Adding {cards.Count} card to hand display");
		foreach (GameObject card in cards)
		{
			Transform cardHolder = GetNextAvailableCardHolder();

			Vector3 startPosition = drawOrigin.position;
			Vector3 targetPosition = cardHolder.transform.position;
			card.transform.SetParent(cardHolder);
			card.transform.position = startPosition;

			float startTime = Time.time;
			while (Time.time - startTime < travelTime)
			{
				card.transform.position = Vector3.Lerp(
					startPosition,
					targetPosition,
					travelCurve.Evaluate((Time.time - startTime) / travelTime)
				);
				yield return null;
			}
			card.transform.position = targetPosition;

			yield return new WaitForSeconds(interval);
		}
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

	public void AddPlayAction(PlayUnitCardAction action)
	{
		playActions.Add(action);
	}

	public void RemovePlayAction(PlayUnitCardAction action)
	{
		playActions.Remove(action);
	}

	public IEnumerable<PlayUnitCardAction> ExportActions()
	{
		PlayUnitCardAction[] exports = new PlayUnitCardAction[playActions.Count];
		playActions.CopyTo(exports);
		playActions.Clear();
		return exports;
	}
}
