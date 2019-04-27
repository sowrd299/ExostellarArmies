using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SFB.Game;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler
{
	public GameObject cardPrefab;
	public float fadeDuration;
	[HideInInspector]
	public GameObject hoverParent;

	private GameObject hoverObject;
	private Graphic[] _cardGraphics;
	private Graphic[] cardGraphics => _cardGraphics ?? (_cardGraphics = GetComponentsInChildren<Graphic>());

	public void OnPointerEnter(PointerEventData eventData)
	{
		hoverObject = Instantiate(cardPrefab, hoverParent.transform);

		CardUI hoverCardUI = hoverObject.GetComponent<CardUI>();
		hoverCardUI.LoadCard(GetComponent<CardUI>().cardData);
		hoverCardUI.LoadHp(GetComponent<CardUI>().GetHP());

		RectTransform hoverParentTransform = hoverParent.GetComponent<RectTransform>();
		RectTransform hoverCardTransform = hoverObject.GetComponent<RectTransform>();
		hoverCardTransform.localScale = Vector3.one * Mathf.Min(
			hoverParentTransform.rect.width / hoverCardTransform.rect.width,
			hoverParentTransform.rect.height / hoverCardTransform.rect.height
		);

		Debug.Log($"There are {hoverObject.GetComponent<CardHover>().cardGraphics.Length} card graphic objects");

		foreach (Graphic cardGraphic in hoverObject.GetComponent<CardHover>().cardGraphics)
		{
			cardGraphic.CrossFadeAlpha(0, 0, true);
			cardGraphic.CrossFadeAlpha(1, fadeDuration, true);
		}
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
			foreach (Graphic cardGraphic in hoverObject.GetComponent<CardHover>().cardGraphics)
			{
				cardGraphic.CrossFadeAlpha(0, fadeDuration, true);
			}

			Destroy(hoverObject, fadeDuration);
		}
	}
}
