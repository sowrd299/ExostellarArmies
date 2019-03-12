using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUI : MonoBehaviour
{
    public TowerFrontEnd tower;
    public TowerUIProperties[] properties;
    private int hp;
    // Start is called before the first frame update
    private void Start()
    {
        if (tower != null)
            LoadTower(tower);
    }

    // Update is called once per frame
    void Update()
    {
        int.TryParse(properties[0].text.text, out hp);
        if (hp <= 0)
            StartCoroutine(DestroyAndRespawn());
    }

    IEnumerator DestroyAndRespawn()
    {
        AudioManager.instance.Play("Tower Explosion 1");
        float timeOfTravel = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < timeOfTravel)
        {
            this.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (elapsedTime / timeOfTravel));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < timeOfTravel)
        {
            this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (elapsedTime / timeOfTravel));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
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
