using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class DropEvent : UnityEvent<DragSource> { }

public class DropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	private static Dictionary<DragDropType, HashSet<DropTarget>> dropTargets = new Dictionary<DragDropType, HashSet<DropTarget>>();

	public DragDropType type;

	public DropEvent onDrop;

	private Outline _outline;
	private Outline outline => _outline == null ? _outline = GetComponent<Outline>() : _outline;

	public Color hintColor;
	public Color hoverColor;

	public static void ShowDropHints(DragDropType type)
	{
		if (dropTargets.ContainsKey(type))
		{
			foreach (DropTarget target in dropTargets[type])
			{
				if (!target.enabled) continue;

				target.outline.enabled = true;
				target.outline.effectColor = target.hintColor;
			}
		}
	}

	public static void HideDropHints(DragDropType type)
	{
		if (dropTargets.ContainsKey(type))
		{
			foreach (DropTarget target in dropTargets[type])
			{
				target.outline.enabled = false;
			}
		}
	}

	private bool CanAccept(GameObject drag, out DragSource source)
	{
		source = null;
		if (!enabled) return false;
		if (drag == null) return false;

		source = drag.GetComponent<DragSource>();
		if (source == null) return false;

		return source.type == type;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (CanAccept(eventData.pointerDrag, out DragSource source))
		{
			source.dropSuccess = true;
			HideDropHints(type);
			onDrop.Invoke(source);
		}
	}

	private void OnEnable()
	{
		if (!dropTargets.ContainsKey(type))
		{
			dropTargets.Add(type, new HashSet<DropTarget>());
		}

		dropTargets[type].Add(this);
	}

	private void Start()
	{
		outline.effectColor = hintColor;
		outline.enabled = false;
	}

	private void OnDisable()
	{
		dropTargets[type].Remove(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!enabled) return;

		if (CanAccept(eventData.pointerDrag, out DragSource source))
		{
			outline.effectColor = hoverColor;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!enabled) return;

		if (CanAccept(eventData.pointerDrag, out DragSource source))
		{
			outline.effectColor = hintColor;
		}
	}
}
