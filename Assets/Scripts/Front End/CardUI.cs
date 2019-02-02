using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;

public class CardUI : MonoBehaviour
{
    //public Card cardBackEnd;
    public CardFrontEnd card;
    public CardUIProperties[] properties;

    private void Start()
    {

        LoadCard(card);
    }

    public void LoadCard(CardFrontEnd c)
    {
        if (c == null)
         return;
        card = c;

        for(int i=0; i<c.properties.Length; i++)
        {
            CardProperties cp = c.properties[i];
            CardUIProperties p = GetProperty(cp.element);
            if(p==null)
                continue;
            
            if(cp.element is ElementInt)
            {
                p.text.text = cp.intValue.ToString();
            }
            else if(cp.element is ElementText)
            {
                p.text.text = cp.stringValue;
            }
            else if(cp.element is ElementImage)
            {
                p.image.sprite = cp.sprite;
            }
        }
        
    }

    //searches untill Element type matches
    public CardUIProperties GetProperty(Element e)
    {
        CardUIProperties res = null;
        for(int i=0; i<properties.Length; i++)
        {
            if(properties[i].element == e)//
            {
                res = properties[i];
                break;
            }
        }
        return res;
    }
}
