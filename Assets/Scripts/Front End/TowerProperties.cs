﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//each CardProperties class will have either stringValue,Sprite or int depending on Element type
[System.Serializable]
public class TowerProperties
{
    public int intValue;
    public Sprite sprite;
    public Element element;
}