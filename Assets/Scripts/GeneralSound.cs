using UnityEngine;

[System.Serializable]
public class GeneralSound : Sound
{
    public void AddSource(AudioSource source)
    {
        this.source = source;
    }
    public override void Play()
    {
        if(source == null) return;

        source.PlayOneShot(clip);
    }
}
