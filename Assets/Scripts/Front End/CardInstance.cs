﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance : MonoBehaviour, IClickable
{
    public void OnClick()
    {

    }

    public void OnHighlight()
    {
        Vector3 v = Vector3.one * .60f;
        this.transform.parent.localScale = v;
    }
}


