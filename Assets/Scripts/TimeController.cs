using System;
using UnityEngine;

/// <summary>
/// Manages time scale and fixedDeltaTime changes
/// </summary>
public class TimeController : MonoBehaviour
{
    public delegate void DelegateVoid();
    public static event DelegateVoid OnTimeScaleChange;

    private static float defaultTimeScale;
    private static float defaultFixedDeltaTime;

    private static float unpausedTimeScale;
    private static float unpausedTargetTimeScale;

    private static float targetTimeScale;
    private static float targetFixedDeltaTime;

    private float velocityTimeScale;
    private float velocityFixedDeltaTime;

    private float smoothTime = 0.5f;

    private void Awake()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;

        targetTimeScale = defaultTimeScale;
        targetFixedDeltaTime = defaultFixedDeltaTime;

        unpausedTimeScale = defaultTimeScale;
    }

    private void Update()
    {
        if (Math.Abs(Time.timeScale - targetTimeScale) > 0.000001)
        {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref velocityTimeScale, smoothTime);

            OnTimeScaleChange?.Invoke();
        }

        if (Math.Abs(Time.fixedDeltaTime - targetFixedDeltaTime) > 0.000001)
        {
            Time.fixedDeltaTime = Mathf.SmoothDamp(Time.fixedDeltaTime, targetFixedDeltaTime, ref velocityFixedDeltaTime, smoothTime);
        }
    }

    ///<summary>
    /// Changes the target timescale and the target fixedDeltaTime
    ///</summary>
    /// <param name="factor"> Factor to change time scale by </param>
    public static void ChangeTime(float factor)
    {
        targetTimeScale *= factor;
        targetFixedDeltaTime *= factor;
    }

    ///<summary>
    /// Pauses time and saves previous values for unpause
    ///</summary>
    public static void PauseTime()
    {
        if(Time.timeScale == 0)
            return;

        unpausedTimeScale = Time.timeScale;
        unpausedTargetTimeScale = targetTimeScale;

        Time.timeScale = 0f;
        targetTimeScale = 0f;
    }

    ///<summary>
    /// Unpauses time to values before pause
    ///</summary>
    public static void UnpauseTime()
    {
        if(Time.timeScale != 0)
            return;

        Time.timeScale = unpausedTimeScale;
        targetTimeScale = unpausedTargetTimeScale;
    }

    ///<summary>
    /// sets target timescale and the target fixedDeltaTime to default values
    ///</summary>
    public static void ResetTime()
    {
        targetTimeScale = defaultTimeScale;
        targetFixedDeltaTime = defaultFixedDeltaTime;
    }
}
