﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_Display : MonoBehaviour
{
    public Text timeDisplay;
    
    void FixedUpdate()
    {
        timeDisplay.text=Math.Floor(Time.time).ToString();

    }
}
