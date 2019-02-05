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
        listofCards = new List<GameObject>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current){position = Input.mousePosition}; 

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData,results);

        //check if we  hit something that has IClickable Interface
        foreach (RaycastResult r in results)
        {
            IClickable c = r.gameObject.GetComponentInParent<IClickable>();
            Debug.Log(r.gameObject.name);
            if (c!= null && r.gameObject.transform.parent.tag == "MyCards")
            {
                c.OnHighlight();
                listofCards.Add(r.gameObject.transform.parent.gameObject);
            }
            else
            {
                if (listofCards.Count > 0)
                {
                    for (int i = 0; i < listofCards.Count; i++)
                    {
                        Vector3 v = Vector3.one * .50f;
                        listofCards[i].gameObject.transform.localScale = v;
                    }
                }
            }
        }
    }
}
