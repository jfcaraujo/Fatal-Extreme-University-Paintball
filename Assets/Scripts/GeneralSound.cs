using UnityEngine;

[System.Serializable]
public class GeneralSound : Sound
{
    public float cooldown;
    private float timeLastPlay;

    public void AddSource(AudioSource source)
    {
        this.source = source;
    }
    public override void Play()
    {
        if(source == null || Time.time - timeLastPlay < cooldown) return;

        source.PlayOneShot(clip);

        timeLastPlay = Time.time;
    }
}
