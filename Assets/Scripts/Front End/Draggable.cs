using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public GameObject myHand=null;

    void Start()
    {
        myHand = GameObject.FindWithTag("MyHand");
    }
    Transform parentToReturnTo = null;
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
    public Transform placeHolderParent = null;

    private GameObject placeHolder = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        myHand.GetComponent<Image>().raycastTarget = true;
        //Debug.Log("Begin Drag");
        placeHolder = new GameObject();
        placeHolder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeHolder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeHolder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
        parentToReturnTo = this.transform.parent;
        placeHolderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        this.transform.position = eventData.position;
        if(placeHolder.transform.parent != placeHolderParent)
        {
            placeHolder.transform.SetParent(placeHolderParent);
        }
        int newSiblingIndex = placeHolderParent.childCount;
        for (int i = 0; i < placeHolderParent.childCount; i++)
        {
            if(this.transform.position.x < placeHolderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (placeHolder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }
        placeHolder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        myHand.GetComponent<Image>().raycastTarget = false;
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0f);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeHolder);
    }
}
