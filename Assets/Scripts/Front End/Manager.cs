﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public State currentState;
    private void Update()
    {
        currentState.startActions();
    }
}
