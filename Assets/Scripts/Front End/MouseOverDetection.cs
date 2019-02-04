using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Actions/MouseOverDetection")]
public class MouseOverDetection : Action
{
    private List<GameObject> listofCards;

    public override void Execute()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current){position = Input.mousePosition}; 

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData,results);

        //check if we  hit something that has IClickable Interface
        foreach (RaycastResult r in results)
        {
            IClickable c = r.gameObject.GetComponentInParent<IClickable>();
            if(c!= null && r.gameObject.transform.parent.tag == "Card")
            {
                c.OnHighlight();
                listofCards.Add(r.gameObject.transform.parent.gameObject);
                Debug.Log(r.gameObject.name);
                break;
            }
            else
            {
                for(int i=0; i<listofCards.Count;i++)
                {
                    Vector3 v = Vector3.one * .50f;
                    listofCards[i].gameObject.transform.localScale = v;
                }
            }
        }
    }
}
