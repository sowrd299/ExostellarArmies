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
	private UnitUI _unitUI;
	private UnitUI unitUI => _unitUI ?? (_unitUI = GetComponent<UnitUI>());

	public void OnPointerEnter(PointerEventData eventData)
	{
		hoverObject = Instantiate(cardPrefab, hoverParent.transform);

		CardUI hoverCardUI = hoverObject.GetComponent<CardUI>();
		hoverCardUI.cardData = card;
		if (unitUI != null)
		{
			// Detect that this is a unit by the presence of unit ui.
			// If so, apply override.
			ApplyUnitOverrides(hoverCardUI, unitUI.unit);
		}

		ZoomHover();

		CanvasGroup hoverGroup = hoverObject.GetComponent<CanvasGroup>();
		hoverGroup.alpha = 0;
		UIManager.instance.LerpTime(
			Mathf.Lerp,
			0f, 1f,
			fadeDuration, alpha => hoverGroup.alpha = alpha
		);

		hoverObject.GetComponent<HoverCardOverlay>().enabled = false;
	}

	private void ApplyUnitOverrides(CardUI cardUI, Unit unit)
	{
		cardUI.RenderCard();
		cardUI.enabled = false;

		ApplyOverrideAspect(unit.HealthPoints, unit.Card.HealthPoints, cardUI.health);
		ApplyOverrideAspect(unit.RangedAttack, unit.Card.RangedAttack, cardUI.rangedDamage);
		ApplyOverrideAspect(unit.MeleeAttack, unit.Card.MeleeAttack, cardUI.meleeDamage);

		cardUI.description.text = unit.MainText;
	}

	private void ApplyOverrideAspect(int unitValue, int cardValue, Text valueText)
	{
		if (unitValue < cardValue)
		{
			valueText.text = unitValue.ToString();
			valueText.color = Color.red;
		}
		else if (unitValue > cardValue)
		{
			valueText.text = unitValue.ToString();
			valueText.color = Color.green;
		}
	}

	private void ZoomHover()
	{
		// Max zoom to help players see better. That's kinda the whole point of this hover thing.
		RectTransform hoverParentTransform = hoverParent.GetComponent<RectTransform>();
		RectTransform hoverCardTransform = hoverObject.GetComponent<RectTransform>();
		hoverCardTransform.position = hoverParentTransform.position;
		hoverCardTransform.localScale = Vector3.one * Mathf.Min(
			hoverParentTransform.rect.width / hoverCardTransform.rect.width,
			hoverParentTransform.rect.height / hoverCardTransform.rect.height
		);
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
			CanvasGroup hoverGroup = hoverObject.GetComponent<CanvasGroup>();
			UIManager.instance.LerpTime(
				Mathf.Lerp,
				1f, 0f,
				fadeDuration, alpha => { if (hoverGroup != null) hoverGroup.alpha = alpha; }
			);

			Destroy(hoverObject, fadeDuration);
		}
	}

	private void OnDestroy()
	{
		RemoveHover();
	}
}
