using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float defaultTimeScale;
    private float defaultFixedDeltaTime;

    void Start()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void SlowDownTime(float factor)
    {
        Time.timeScale *= factor;
        Time.fixedDeltaTime *= factor;
    }

    public void ResetTime()
    {
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
}
