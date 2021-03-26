using UnityEngine;

/// <summary>
/// Represent a general (short) sound clip.
/// </summary>
[System.Serializable]
public class GeneralSound : Sound
{
    // Used to control the speed at which a sound is triggered to be played
    public float cooldown;
    private float timeLastPlay;

    /// <summary>
    /// Add a audio source to play the clip.
    /// </summary>
    /// <param name="source">Audio source to be added.</param>
    public void AddSource(AudioSource source)
    {
        this.source = source;
    }

    /// <summary>
    /// Plays the clip.
    /// </summary>
    public override void Play()
    {
        if(source == null || Time.time - timeLastPlay < cooldown) return;

        source.PlayOneShot(clip);

        timeLastPlay = Time.time;
    }
}
