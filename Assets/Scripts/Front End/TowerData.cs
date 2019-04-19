using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class attached to Card Prefab, contains array of properties(all the texts,images,ints with their Elemenet type)
[CreateAssetMenu(menuName = "Data/Tower Data")]
public class TowerData : ScriptableObject
{
    public TowerProperty[] properties;

    public TowerData(TowerProperty[] cardProp)
    {
        this.properties = cardProp;
    }
}
