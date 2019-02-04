using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
            d.ParentToReturnTo = this.transform;
    }

    public void OnPointerEnter(PointerEventData eventDate)
    {

    }

    public void OnPointerExit(PointerEventData eventDate)
    {

    }
}
