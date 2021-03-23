﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class Time_Display : MonoBehaviour
{
    public Text timeDisplay;
    private float startTime;
    private static bool stopTime;

    void Start()
    {
        startTime = Time.time;
        stopTime = false;
    }

    void FixedUpdate()
    {
        if (!stopTime)
            timeDisplay.text = Math.Floor(Time.time - startTime).ToString();
    }

    public static void Stop()
    {
        stopTime = true;
    }
}