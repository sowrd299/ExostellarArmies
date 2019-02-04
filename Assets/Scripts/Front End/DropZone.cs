using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private void Update()
    {
       
    }
    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            if (this.gameObject.tag == "CardHolder")
            {
                if (this.gameObject.transform.childCount == 0)
                    d.ParentToReturnTo = this.transform;
            }
            else
            {
                d.ParentToReturnTo = this.transform;
            }
            
        }

    }

    public void OnPointerEnter(PointerEventData eventDate)
    {

    }

    public void OnPointerExit(PointerEventData eventDate)
    {

    }
}
