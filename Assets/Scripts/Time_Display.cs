using System;
using UnityEngine;
using UnityEngine.UI;

public class Time_Display : MonoBehaviour
{
    public Text timeDisplay;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }
    void FixedUpdate()
    {
        timeDisplay.text=Math.Floor(Time.time-startTime).ToString();

    }
}
