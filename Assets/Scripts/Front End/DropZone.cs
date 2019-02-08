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
        if (d != null)
        {
            if (this.gameObject.tag == "CardHolder")
            {
                if (this.gameObject.transform.childCount == 0)
                {
                    d.ParentToReturnTo = this.transform;
                    RectTransform parent = this.gameObject.transform.GetComponent<RectTransform>();
                    //GridLayoutGroup grid = this.gameObject.transform.GetComponent<GridLayoutGroup>();
                    //grid.cellSize = new Vector2(parent.rect.width, parent.rect.height);
                    Debug.Log("Dragging:"+d.gameObject.name);
                    RectTransform rt = d.gameObject.transform.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(parent.sizeDelta.x, parent.sizeDelta.x);
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


    public void OnPointerEnter(PointerEventData eventDate)
    {

    }

    public void OnPointerExit(PointerEventData eventDate)
    {

    }
}
