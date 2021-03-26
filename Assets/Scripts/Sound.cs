using UnityEngine;

/// <summary>
/// Abstract class to represent a sound clip.
/// </summary>
public abstract class Sound
{
    public string name;

    public AudioClip clip;

    protected AudioSource source;

    public abstract void Play();

    public void ChangePitch(float value)
    {
        if (source)
        {
            source.pitch = value;
        }
    }
}
