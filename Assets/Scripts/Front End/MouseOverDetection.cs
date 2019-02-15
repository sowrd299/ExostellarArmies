using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Actions/MouseOverDetection")]
public class MouseOverDetection : Action
{
    private List<GameObject> listofCards = new List<GameObject>();

    public override void Execute()
    {
        //listofCards = new List<GameObject>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current){position = Input.mousePosition}; 

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData,results);

        if(results.Count == 0)
        {
            int i = 0;
            while (i < listofCards.Count)
            {
                Vector3 v = Vector3.one * .4f;
                listofCards[i].gameObject.transform.localScale = v;
                i++;
            }
            listofCards.Clear();
        }
        else
        {
            //check if we  hit something that has IClickable Interface
            foreach (RaycastResult r in results)
            {
                IClickable c = r.gameObject.GetComponentInParent<IClickable>();
                Debug.Log(r.gameObject.transform.parent.tag);
                if (c != null && r.gameObject.transform.parent.tag == "MyCards")
                {
                    c.OnHighlight();
                    if (!listofCards.Contains(r.gameObject.transform.parent.gameObject))
                    {
                        listofCards.Add(r.gameObject.transform.parent.gameObject);
                    }
                }
                else if(c== null || r.gameObject.transform.tag == "Card")
                {
                    int i = 0;
                    while (i < listofCards.Count)
                    {
                        Vector3 v = Vector3.one * .4f;
                        listofCards[i].gameObject.transform.localScale = v;
                        i++;
                    }
                    listofCards.Clear();
                }
            }
        }
       
    }
}
