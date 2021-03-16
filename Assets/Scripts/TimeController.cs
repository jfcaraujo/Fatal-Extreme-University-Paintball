using UnityEngine;

public class TimeController : MonoBehaviour
{
    private static float defaultTimeScale;
    private static float defaultFixedDeltaTime;

    private void Start()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public static void SlowDownTime(float factor)
    {
        Time.timeScale *= factor;
        Time.fixedDeltaTime *= factor;
    }

    public static void ResetTime()
    {
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
}
