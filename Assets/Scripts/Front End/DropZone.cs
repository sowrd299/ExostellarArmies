using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SFB.Game;


public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        //Lane[] l = Driver.Instance.lanes;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        Debug.Log(d == null);
        if (d != null)
        {
            if (this.gameObject.tag == "CardHolder")
            {
                if (this.gameObject.transform.childCount <= 1)
                {
                    d.ParentToReturnTo = this.transform;
                    RectTransform parent = this.gameObject.transform.GetComponent<RectTransform>();
                    RectTransform rt = d.gameObject.transform.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(parent.rect.width, parent.rect.height);
                }
            }
            else if(this.gameObject.tag == "EnemyCardHolder" && d.gameObject.tag == "MyCards")
            {
                return;
            }
            else
            {
                d.ParentToReturnTo = this.transform;
            }

            
        }

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            Debug.Log("d==null: " + d == null);
            d.placeHolderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeHolderParent == this.transform)
        {
            d.placeHolderParent = d.ParentToReturnTo;
        }
    }
}
