using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Net.Client;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Lane l=null;
    private PlayUnitCardAction action;
    private Player p;

    void Update()
    {
        if (this.gameObject.tag == "CardHolder" )
        {
			if(l == null)
				l = Driver.instance?.gameManager?.Lanes[this.transform.GetSiblingIndex()];
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            CardUI c = d.gameObject.GetComponent<CardUI>();
            p = Driver.instance.gameManager.Players[0];
            if (this.gameObject.tag == "CardHolder" && this.gameObject.transform.childCount <= 1)
                d.ParentToReturnTo = this.transform;
            else if(this.gameObject.tag == "MyHand")
                d.ParentToReturnTo = this.transform;
            else if(this.gameObject.tag == "EnemyCardHolder" && d.gameObject.tag == "MyCards")
                return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
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
