using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageAnimationController : MonoBehaviour
{
    private List<GameObject> lanes = new List<GameObject>();
    private List<GameObject> l1Texts = new List<GameObject>();
    private List<GameObject> l2Texts = new List<GameObject>();
    private List<GameObject> l3Texts = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        initializelists();
        //StartCoroutine(startAnim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void initializelists()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject lane = this.transform.GetChild(i).gameObject;
            lanes.Add(lane);
            for (int m = 0; m < lane.transform.childCount; m++)
            {
                if (i == 0)
                    l1Texts.Add(lane.transform.GetChild(m).gameObject);
                else if(i==1)
                    l2Texts.Add(lane.transform.GetChild(m).gameObject);
                else
                    l3Texts.Add(lane.transform.GetChild(m).gameObject);
            }
        }
    }

    private bool checkIfHasDamage(List<GameObject> texts)
    {
        bool ans = false;
        for (int i = 0; i < texts.Count; i++)
        {
            int cardCost;
            int.TryParse(texts[i].GetComponent<Text>().text, out cardCost);
            if (cardCost > 0)
            {
                ans = true;
                texts[i].gameObject.SetActive(true);
            }
        }
        return ans;
    }

    public IEnumerator startAnim()
    {
        checkIfHasDamage(l1Texts);
        checkIfHasDamage(l2Texts);
        checkIfHasDamage(l3Texts);
        yield return new WaitForSeconds(1f);
        StartCoroutine(changeSpots(l1Texts));
        StartCoroutine(changeSpots(l2Texts));
        StartCoroutine(changeSpots(l3Texts));
    }

    IEnumerator changeSpots(List<GameObject> texts)
    {
        float timeOfTravel = 0.5f;
        float elapsedTime = 0f;
        Vector3 startingPos1 = texts[0].transform.position;
        Vector3 startingPos2 = texts[1].transform.position;
        while (elapsedTime < timeOfTravel)
        {
            texts[0].transform.position = Vector3.Lerp(startingPos1, startingPos2, (elapsedTime / timeOfTravel));
            texts[1].transform.position = Vector3.Lerp(startingPos2, startingPos1, (elapsedTime / timeOfTravel));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        texts[0].GetComponent<Text>().color = Color.red;
        string s = "-"+texts[0].GetComponent<Text>().text.ToString();
        texts[0].GetComponent<Text>().text = s; 
        StartCoroutine(textShake(texts[0]));

        texts[1].GetComponent<Text>().color = Color.red;
        string s2 = "-" + texts[1].GetComponent<Text>().text.ToString();
        texts[1].GetComponent<Text>().text = s2;
        StartCoroutine(textShake(texts[1]));

        Driver.instance.updateCardsOntable();
        Driver.instance.updateTowerUI();
        yield return new WaitForSeconds(1f);
        turnOfAllTexts();

    }

    void turnOfAllTexts()
    {
        for (int i = 0; i < l1Texts.Count; i++)
        {
            l1Texts[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < l2Texts.Count; i++)
        {
            l2Texts[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < l2Texts.Count; i++)
        {
            l3Texts[i].gameObject.SetActive(false);
        }
    }

    IEnumerator textShake(GameObject g)
    {
        float elapsed = 0.0f;
        float timeOfTravel = 0.5f;
        Quaternion originalRotation = g.transform.rotation;
        while (elapsed < timeOfTravel)
        {
            float z = Random.value * 10 - (10 / 2);
            g.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z + z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        g.transform.rotation = originalRotation;
    }
}
