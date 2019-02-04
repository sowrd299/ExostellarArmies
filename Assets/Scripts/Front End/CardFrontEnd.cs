using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class attached to Card Prefab, contains array of properties(all the texts,images,ints with their Elemenet type)
[CreateAssetMenu(menuName = "CardFrontEnd")]
public class CardFrontEnd : ScriptableObject
{
    public CardProperties[] properties;

    public CardFrontEnd (CardProperties[] cardProp)
    {
        this.properties = cardProp;
    }
}
