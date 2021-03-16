using UnityEngine;

public class PaperTowel : Item
{
    protected override bool ConsumeItem()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        HealthController healthController = player.GetComponent<HealthController>();

        return healthController.AddPaperTowels(1);
    }
}
