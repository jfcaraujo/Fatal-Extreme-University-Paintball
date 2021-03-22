using UnityEngine;

public class TimeController : MonoBehaviour
{
    public delegate void DelegateVoid();
    public static event DelegateVoid OnTimeScaleChange;

    private static float defaultTimeScale;
    private static float defaultFixedDeltaTime;

    private static float targetTimeScale;
    private static float targetFixedDeltaTime;

    private float velocityTimeScale;
    private float velocityFixedDeltaTime;

    private float smoothTime = 0.5f;

    private void Awake()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;

        targetTimeScale = Time.timeScale;
        targetFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (Time.timeScale != targetTimeScale)
        {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref velocityTimeScale, smoothTime);

            OnTimeScaleChange?.Invoke();
        }

        if (Time.fixedDeltaTime != targetFixedDeltaTime)
        {
            Time.fixedDeltaTime = Mathf.SmoothDamp(Time.fixedDeltaTime, targetFixedDeltaTime, ref velocityFixedDeltaTime, smoothTime);
        }
    }

    public static void ChangeTime(float factor)
    {
        targetTimeScale = Time.timeScale * factor;
        targetFixedDeltaTime = Time.fixedDeltaTime * factor;
    }

    public static void ResetTime()
    {
        targetTimeScale = defaultTimeScale;
        targetFixedDeltaTime = defaultFixedDeltaTime;
    }
}
