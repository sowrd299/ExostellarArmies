using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DragSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[Header("DnD Config")]
	public string dragLayerTag;
	public DragDropType type;

	[Header("Animation Config")]
	public float returnTime;
	public AnimationCurve returnCurve;

	[Header("Events")]
	public UnityEvent onBeginDrag;

	[HideInInspector]
	public bool dropSuccess;
	private Transform dragLayer;
	public Transform originalParent { get; private set; }
	private Vector3 originalPosition;
	private Vector2 startPointerPosition;

	private CanvasGroup _canvasGroup;
	private CanvasGroup canvasGroup => _canvasGroup == null ? _canvasGroup = GetComponent<CanvasGroup>() : _canvasGroup;

	private Camera _mainCamera;
	private Camera mainCamera => _mainCamera ?? (_mainCamera = Camera.main);

	private void Start()
	{
		dragLayer = GameObject.FindGameObjectWithTag(dragLayerTag).transform;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		originalParent = transform.parent;
		originalPosition = transform.position;
		transform.SetParent(dragLayer);
		startPointerPosition = mainCamera.ScreenToWorldPoint(eventData.position);

		canvasGroup.blocksRaycasts = false;

		DropTarget.ShowDropHints(type);

		onBeginDrag.Invoke();
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = originalPosition + mainCamera.ScreenToWorldPoint(eventData.position) - (Vector3)startPointerPosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!dropSuccess)
		{
			StartCoroutine(ReturnToSender());
		}

		dropSuccess = false;
		canvasGroup.blocksRaycasts = true;

		DropTarget.HideDropHints(type);
	}

	private IEnumerator ReturnToSender()
	{
		yield return UIManager.instance.LerpTime(
			Vector3.Lerp,
			transform.position,
			originalPosition,
			returnTime,
			returnCurve.Evaluate,
			position => transform.position = position
		);

		transform.SetParent(originalParent);
	}
}
