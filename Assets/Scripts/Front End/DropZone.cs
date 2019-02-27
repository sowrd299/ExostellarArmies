using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;


public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Lane l=null;
    private PlayUnitCardAction action;
    private Player p;

    void Start()
    {
        if (this.gameObject.tag == "CardHolder" )
        {
            Debug.Log("Sibl" + (this.transform.GetSiblingIndex())+(this.transform.gameObject.name));

            l = Driver.instance.gameManager.Lanes[this.transform.GetSiblingIndex()];

        }


        //if (Driver.instance.gameManager.Players[0]!=null)
        //    p = Driver.instance.gameManager.Players[0];
        //action = new PlayUnitCardAction(UnitCard c, Lane l, int play, int pos);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            Debug.Log("L==null" + (l == null));
            CardUI c = d.gameObject.GetComponent<CardUI>();
            p = Driver.instance.gameManager.Players[0];
            UnitCard uc = p.Hand[c.handIndex] as UnitCard;
            if (this.gameObject.tag == "CardHolder" && this.gameObject.transform.childCount <= 1)
            {
                if (this.transform.parent.name.Contains("Front"))
                    action = new PlayUnitCardAction(uc, l, 0, 0);
                else
                    action = new PlayUnitCardAction(uc, l, 0, 1);
                if (Driver.instance.gameManager.IsLegalAction(p, action))
                {
                    d.ParentToReturnTo = this.transform;
                    foreach (Delta del in action.GetDeltas(Driver.instance.gameManager.Players[0]))
                        del.Apply();
                }

                //RectTransform parent = this.gameObject.transform.GetComponent<RectTransform>();
                //RectTransform rt = d.gameObject.transform.GetComponent<RectTransform>();
                //rt.sizeDelta = new Vector2(parent.rect.width, parent.rect.height);
            }
            else if(this.gameObject.tag == "MyHand")
            {
                //Driver.instance.dropCostSum -= cardCost;
                d.ParentToReturnTo = this.transform;
            }
            else if(this.gameObject.tag == "EnemyCardHolder" && d.gameObject.tag == "MyCards")
            {
                return;
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
