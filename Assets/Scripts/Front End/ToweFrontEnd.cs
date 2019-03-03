using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class attached to Card Prefab, contains array of properties(all the texts,images,ints with their Elemenet type)
[CreateAssetMenu(menuName = "TowerFrontEnd")]
public class TowerFrontEnd : ScriptableObject
{
    public TowerProperties[] properties;

    public TowerFrontEnd(TowerProperties[] cardProp)
    {
        this.properties = cardProp;
    }
}
