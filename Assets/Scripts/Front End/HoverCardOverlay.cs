using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SFB.Game;

public class HoverCardOverlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler
{
	[Header("Asset References")]
	public GameObject cardPrefab;

	[Header("Animation Config")]
	public float fadeDuration;
	[HideInInspector]
	public GameObject hoverParent;

	private GameObject hoverObject;
	private Graphic[] _cardGraphics;
	private Graphic[] cardGraphics => _cardGraphics ?? (_cardGraphics = GetComponentsInChildren<Graphic>());
	private Card _card;
	private Card card => _card ?? (_card = GetComponent<IHasCard>().card);

	public void OnPointerEnter(PointerEventData eventData)
	{
		hoverObject = Instantiate(cardPrefab, hoverParent.transform);

		CardUI hoverCardUI = hoverObject.GetComponent<CardUI>();
		hoverCardUI.cardData = card;

		// Max zoom to help players see better. That's kinda the whole point of this hover thing.
		RectTransform hoverParentTransform = hoverParent.GetComponent<RectTransform>();
		RectTransform hoverCardTransform = hoverObject.GetComponent<RectTransform>();
		hoverCardTransform.position = hoverParentTransform.position;
		hoverCardTransform.localScale = Vector3.one * Mathf.Min(
			hoverParentTransform.rect.width / hoverCardTransform.rect.width,
			hoverParentTransform.rect.height / hoverCardTransform.rect.height
		);

		foreach (Graphic cardGraphic in hoverObject.GetComponent<HoverCardOverlay>().cardGraphics)
		{
			cardGraphic.CrossFadeAlpha(0, 0, true);
			cardGraphic.CrossFadeAlpha(1, fadeDuration, true);
		}

		hoverObject.GetComponent<HoverCardOverlay>().enabled = false;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		RemoveHover();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		RemoveHover();
	}

	public void RemoveHover()
	{
		if (hoverObject != null)
		{
			foreach (Graphic cardGraphic in hoverObject.GetComponent<HoverCardOverlay>().cardGraphics)
			{
				cardGraphic.CrossFadeAlpha(0, fadeDuration, true);
			}

			Destroy(hoverObject, fadeDuration);
		}
	}
}
