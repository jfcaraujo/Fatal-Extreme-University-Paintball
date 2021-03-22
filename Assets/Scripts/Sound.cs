using UnityEngine;

public abstract class Sound
{
    public string name;

    public AudioClip clip;

    protected AudioSource source;

    public abstract void Play();
}
