using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;
    public Transform ParentToReturnTo
    {
        get
        {
            return parentToReturnTo;
        }
        set
        {
            parentToReturnTo = value;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("On Drag");
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        this.transform.SetParent(parentToReturnTo);
    }
}
