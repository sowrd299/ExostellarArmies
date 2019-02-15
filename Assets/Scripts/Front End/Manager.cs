using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public State currentState;

    [SerializeField]
    private GameObject cardPrefab;
    public List<GameObject> cards = new List<GameObject>();

    [SerializeField]
    private GameObject placeHolder;
    [SerializeField]
    private GameObject handPlaceHolder;


    [SerializeField]
    private Text handCapacity;


    private void FixedUpdate()
    {
        handCapacity.text = "Hand capacity\n" + handPlaceHolder.gameObject.transform.childCount.ToString()+"/3";
    }

    public void spawnCards()
    {
        while (handPlaceHolder.gameObject.transform.childCount + cards.Count < 3)
        {
            GameObject tempCard = Instantiate(cardPrefab, placeHolder.transform);
            cards.Add(tempCard);
        }   
        StartCoroutine(moveToHand());
    }

    IEnumerator moveToHand()
    {
        while(cards.Count>0)
        {
            float timeOfTravel = 0.5f;
            float elapsedTime = 0f;
            Vector3 startingPosition = cards[0].transform.position;
            while (elapsedTime < timeOfTravel)
            {
                cards[0].transform.position = Vector3.Lerp(startingPosition, handPlaceHolder.transform.position, (elapsedTime / timeOfTravel));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cards[0].gameObject.transform.SetParent(handPlaceHolder.transform);
            cards.RemoveAt(0);
            //HorizontalLayoutGroup h = handPlaceHolder.gameObject.GetComponent<HorizontalLayoutGroup>();
            //h.enabled = false;
            //h.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    private void Update()
    {
        currentState.startActions();
    }
}
