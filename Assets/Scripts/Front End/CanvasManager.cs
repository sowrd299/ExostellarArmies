using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;

public class CanvasManager : MonoBehaviour
{

    [SerializeField]
    private List<Button> imageList;
    [SerializeField]
    private List<Image> handPlaced;

    private List<string> cardNames;
    

    private List<RectTransform> cardPos = new List<RectTransform>();
    private float speed;

    private List<bool> frontLineFull;
    private List<bool> backLineFull;

    private int cardChosen = -1;
    // Start is called before the first frame update
    void Start()
    {
        speed = 10f;
        for(int i = 0; i< imageList.Count; i++)
        {
            cardPos[i] = imageList[i].gameObject.GetComponent<RectTransform>();
        }
        //read from Hand
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void chooseCard(int i)
    {
        cardChosen=i;
    }

    public void makeMove(int i)
    {
        if(cardChosen != -1 && !frontLineFull[i] && !backLineFull[i])
            StartCoroutine(startMoving(i));
    }

    IEnumerator startMoving(int i)
    {
        float startTime = Time.time;
		while(Time.time < startTime + 0.5f && imageList[i].transform.position.y < handPlaced[i].transform.position.y)
		{
			imageList[i].transform.Translate(0f,speed,0f);
			yield return null;
        }
        yield return null;
    }
    
}