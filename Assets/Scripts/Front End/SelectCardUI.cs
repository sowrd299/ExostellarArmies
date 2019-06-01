using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Content;

public class SelectCardUI : MonoBehaviour
{
	public static SelectCardUI instance;

	[Header("Asset References")]
	public GameObject cardHolderPrefab;
	public GameObject cardPrefab;

	[Header("UI References")]
	public CanvasGroup background;
	public Text title;
	public GameObject cardGrid;
	public DropTarget dropTarget;

	[Header("Config")]
	public float fadeDuration;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	public void DEBUG_ChooseCard()
	{
		StartCoroutine(DEBUG_AnimateChooseCard());
	}

	private IEnumerator DEBUG_AnimateChooseCard()
	{
		CardLoader cardLoader = new CardLoader();
		StringBuilder output = new StringBuilder();

		yield return ChooseCard(new[] {
			cardLoader.GetByID("Commercial Coms Relay"),
			cardLoader.GetByID("Ancillary Medical Officer"),
			cardLoader.GetByID("Cannoneer Drone"),
			cardLoader.GetByID("Paladin-Class XS Marines")
		}, output);

		Debug.Log(output.ToString());
	}

	public Coroutine ChooseCard(Card[] cards, StringBuilder output)
	{
		return StartCoroutine(AnimateChooseCard(cards, output));
	}

	private IEnumerator AnimateChooseCard(Card[] cards, StringBuilder output)
	{
		DragDropType dragType = dropTarget.type;
		List<Transform> cardHolders = new List<Transform>();
		foreach (Card card in cards)
		{
			GameObject cardHolder = Instantiate(cardHolderPrefab, cardGrid.transform);
			GameObject cardObject = Instantiate(cardPrefab, cardHolder.transform);

			cardObject.GetComponent<DragSource>().type = dragType;
			cardObject.GetComponent<CardUI>().cardData = card;
			cardObject.GetComponent<HoverCardOverlay>().enabled = false;

			cardHolders.Add(cardHolder.transform);
		}

		background.interactable = true;
		background.blocksRaycasts = true;
		yield return UIManager.instance.LerpTime(
			Mathf.Lerp,
			0f, 1f, fadeDuration,
			alpha => background.alpha = alpha
		);

		UnityAction<DragSource> dropListener = null;
		bool dropped = false;
		dropListener = dragSource =>
		{
			output.Clear();
			output.Append(dragSource.GetComponent<CardUI>().card.ID);
			dropped = true;
			Destroy(dragSource.gameObject);
			dropTarget.onDrop.RemoveListener(dropListener);
		};
		dropTarget.onDrop.AddListener(dropListener);

		yield return new WaitUntil(() => dropped);

		yield return UIManager.instance.LerpTime(
			Mathf.Lerp,
			1f, 0f, fadeDuration,
			alpha => background.alpha = alpha
		);

		foreach (Transform cardHolder in cardHolders)
		{
			Destroy(cardHolder.gameObject);
		}
		background.interactable = false;
		background.blocksRaycasts = false;
	}
}
