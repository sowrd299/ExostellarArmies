using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;

public class CardUI : MonoBehaviour
{
    //public Card cardBackEnd;
    public CardFrontEnd card;
    public CardUIProperties[] properties;

    private List<CardFrontEnd> playerFrontEnds = new List<CardFrontEnd>();
    private List<CardFrontEnd> enemyFrontEnds = new List<CardFrontEnd>();
    public Card cardBackEnd;

    public int handIndex
    {
        get { 
            if(!this.gameObject.name.Contains("Enemy"))
                return Driver.instance.gameManager.Players[0].Hand.IndexOf(cardBackEnd); 
            else
                return Driver.instance.gameManager.Players[1].Hand.IndexOf(cardBackEnd);
        }
    }
    private bool old = false;
    public bool Old
    {
        get
        {
            return old;
        }
        set
        {
            old = value;
        }
    }

    private void Start()
    {
        if (card != null)
            LoadCard(card);
        //handIndex = this.transform.GetSiblingIndex();

        if (!this.gameObject.name.Contains("Enemy"))
            cardBackEnd = Driver.instance.gameManager.Players[0].Hand[this.transform.GetSiblingIndex()];
        else
            cardBackEnd = Driver.instance.gameManager.Players[1].Hand[this.transform.GetSiblingIndex()];

    }

    private void Update()
    {
//        Debug.Log("Index: " + this.transform.GetSiblingIndex());
        if(playerFrontEnds != null && playerFrontEnds.Count==0)
            playerFrontEnds = Driver.instance.loadFrontEnd(Driver.instance.gameManager.Players[0]);
        if (enemyFrontEnds != null && enemyFrontEnds.Count == 0)
            enemyFrontEnds = Driver.instance.loadFrontEnd(Driver.instance.gameManager.Players[1]);

        if (card==null)
        {
            if(!this.gameObject.name.Contains("Enemy"))
            {
                LoadCard(playerFrontEnds[this.gameObject.transform.GetSiblingIndex()]);
                playerFrontEnds.RemoveAt(this.gameObject.transform.GetSiblingIndex());
            }
            else
            {
                LoadCard(enemyFrontEnds[this.gameObject.transform.GetSiblingIndex()]);
                enemyFrontEnds.RemoveAt(this.gameObject.transform.GetSiblingIndex());
            }
        }
        //Debug.Log("Name ="+this.gameObject.transform.GetSiblingIndex().ToString()+this.properties[0].text.text);
    }

    //Updates all of the UI properties to the values in c
    public void LoadCard(CardFrontEnd c)
    {
        if (c == null)
         return;
        this.card = c;
        for (int i=0; i<c.properties.Length; i++)
        {
            CardProperties cp = c.properties[i];
            CardUIProperties p = GetProperty(cp.element);
            if(p==null)
                continue;
            
            if(cp.element is ElementInt)
            {
                p.text.text = cp.intValue.ToString();
            }
            else if(cp.element is ElementText)
            {
                p.text.text = cp.stringValue;
            }
            else if(cp.element is ElementImage)
            {
                p.image.sprite = cp.sprite;
            }
        }
    }

    //searches untill Element type matches
    public CardUIProperties GetProperty(Element e)
    {
        CardUIProperties res = null;
        for(int i=0; i<properties.Length; i++)
        {
            if(properties[i].element == e)//
            {
                res = properties[i];
                break;
            }
        }
        return res;
    }
}
