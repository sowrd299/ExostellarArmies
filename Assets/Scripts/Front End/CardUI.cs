using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Text name;
    public Text detail;
    public Text flavor;
    public Text type;

    public CardFrontEnd card;

    private void Start()
    {
        LoadCard(card);
    }

    public void LoadCard(CardFrontEnd c)
    {
        card = c;
        name.text = c.cardName;
        detail.text = c.cardDetail;
        flavor.text = c.cardFlavor;
        type.text = c.cardType;
    }
}
