using UnityEngine;

public abstract class Sound
{
    public string name;

    public AudioClip clip;

    protected AudioSource source;

    public abstract void Play();

    public void ChangePitch(float value)
    {
        if (source != null)
        {
            source.pitch = value;
        }
    }
}
