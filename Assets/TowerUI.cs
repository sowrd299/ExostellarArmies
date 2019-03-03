using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUI : MonoBehaviour
{
    public TowerFrontEnd tower;
    public TowerUIProperties[] properties;
    // Start is called before the first frame update
    private void Start()
    {
        if (tower != null)
            LoadTower(tower);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTower(TowerFrontEnd t)
    {
        if (t == null)
            return;
        this.tower = t;
        for (int i = 0; i < t.properties.Length; i++)
        {
            TowerProperties tp = t.properties[i];
            TowerUIProperties p = GetProperty(tp.element);
            if (p == null)
                continue;

            if (tp.element is ElementInt)
            {
                p.text.text = tp.intValue.ToString();
            }
            else if (tp.element is ElementImage)
            {
                p.image.sprite = tp.sprite;
            }
        }
    }

    public TowerUIProperties GetProperty(Element e)
    {
        TowerUIProperties res = null;
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].element == e)//
            {
                res = properties[i];
                break;
            }
        }
        return res;
    }
}
