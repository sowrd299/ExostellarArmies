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
            CardUI c = d.gameObject.GetComponent<CardUI>();
            p = Driver.instance.gameManager.Players[0];

            if (this.gameObject.tag == "CardHolder" && this.gameObject.transform.childCount <= 1)
            {
                //UnitCard uc = c.cardBackEnd as UnitCard;
                //if (this.transform.parent.name.Contains("Front"))
                //    action = new PlayUnitCardAction(uc, l, 0, 0);
                //else
                //    action = new PlayUnitCardAction(uc, l, 0, 1);
                //if (Driver.instance.gameManager.IsLegalAction(p, action))
                //{
                //    d.ParentToReturnTo = this.transform;
                //    Delta[] del = action.GetDeltas(Driver.instance.gameManager.Players[0]);
                //    for (int i = 1; i < del.Length; i++)
                //    {
                //        del[i].Apply();
                //    }
                //    //foreach (Delta del in action.GetDeltas(Driver.instance.gameManager.Players[0]))
                //        //del.Apply();
                //}

                d.ParentToReturnTo = this.transform;

            }
            else if(this.gameObject.tag == "MyHand")
            {
                //Driver.instance.dropCostSum -= cardCost;
                //Driver.instance.gameManager.Players[0].Hand.Add(c.cardBackEnd);
                //Driver.instance.myMana.Add(c.cardBackEnd.DeployCost);
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
