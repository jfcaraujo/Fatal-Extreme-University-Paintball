using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the timer display
/// </summary>
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
            timeDisplay.text = Math.Floor(Time.time - startTime).ToString();//displays seconds since start time
    }

    ///<summary>
    /// Stops updates in the time display. Used when the player dies.
    ///</summary>
    public static void Stop()
    {
        stopTime = true;
    }
}