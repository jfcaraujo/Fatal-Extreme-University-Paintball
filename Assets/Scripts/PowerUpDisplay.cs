using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the power-up display
/// </summary>
public class PowerUpDisplay : MonoBehaviour
{
    public Slider slider;
    private int maxDuration;
    public Transform powerUpSprite;

    ///<summary>
    /// Sets the maximum duration for the power-ups
    ///</summary>
    public void SetMaxDuration(int duration)
    {
        slider.maxValue = duration;
        maxDuration = duration;
    }

    void Update()
    {
        if (slider.value > 0)
        {
            if (!PauseMenu.gameIsPaused)
                slider.value -= Time.unscaledDeltaTime;
        }

        if (slider.value <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    ///<summary>
    /// Activates the power-up display
    ///</summary>
    public void StartNewPowerUp(int powerUp)
    {
        slider.value = maxDuration;
        gameObject.SetActive(true);
        int i = 0;
        foreach (Transform powerup in powerUpSprite)
        {
            powerup.gameObject.SetActive(i == powerUp);
            i++;
        }
    }

    ///<summary>
    /// Deactivates the power-up display
    ///</summary>
    public void EndDisplay()
    {
        slider.value = 0;
    }
}