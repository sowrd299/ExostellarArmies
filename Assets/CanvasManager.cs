using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    private List<Image> imageList;
    private Image i;

    private List<string> cardNames;

    private List<RectTransform> cardPos = new List<RectTransform>();
    private Vector3 startingPos;
    private float speed;
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

    public void makeMove(int i)
    {
        imageList[i].transform.Translate(0f,speed,0f);
    }

    IEnumerator startMoving(int i)
    {
        while(true)
        {
            
        }
    }
    
}