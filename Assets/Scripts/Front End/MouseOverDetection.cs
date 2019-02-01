using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Actions/MouseOverDetection")]
public class MouseOverDetection : Action
{
    public override void Execute()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current){position = Input.mousePosition}; 

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData,results);

        //check if we  hit something that has IClickable Interface
        foreach (RaycastResult r in results)
        {
            IClickable c = r.gameObject.GetComponentInParent<IClickable>();
            if(c!= null)
            {
                c.OnHighlight();
                break;
            }
        }
    }
}
