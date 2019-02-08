using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public State currentState;

    [SerializeField]
    private List<GameObject> cardPrefabs;
    public List<GameObject> cards = new List<GameObject>();

    [SerializeField]
    private GameObject placeHolder;
    [SerializeField]
    private GameObject handPlaceHolder;


    public void spawnCards()
    {
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject tempCard = Instantiate(cardPrefabs[i], placeHolder.transform);
            cards.Add(tempCard);
        }
        StartCoroutine(moveToHand());
    }

    IEnumerator moveToHand()
    {
        while(cards.Count>0)
        {
            cards[0].gameObject.transform.SetParent(handPlaceHolder.transform.parent);
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
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    private void Update()
    {
        currentState.startActions();
    }
}
