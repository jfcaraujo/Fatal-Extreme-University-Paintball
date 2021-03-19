using UnityEngine;
using UnityEngine.UI;

public class PowerUpDisplay : MonoBehaviour
{
    public Slider slider;
    private int maxDuration;
    public Transform powerUpSprite;

    public void SetMaxDuration(int duration)
    {
        slider.maxValue = duration;
        maxDuration = duration;
    }

    // Update is called once per frame
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

    public void EndDisplay()
    {
        slider.value = 0;
    }
}