using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the audio for an object.
/// Needs a main AudioSource that will be used to play any short sound effects (General Sounds).
/// Creates individual AudioSources for longer sounds that need to be stopped and played (Individual Sounds).
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Main audio source used to play general sounds
    public AudioSource mainAudioSource;

    // General sounds play in the main audio source
    public GeneralSound[] generalSounds;

    // Individual sounds play on their own audio source
    public IndividualSound[] individualSounds;

    // Full list with all sounds
    private List<Sound> sounds;

    private void Awake()
    {
        // Subscribe to TimeScaleChange event, to change the sound's pitch according to the game speed
        TimeController.OnTimeScaleChange += AdjustPitchToTime;

        if (mainAudioSource == null)
        {
            Debug.LogWarning("No main audio source specified, general sounds won't be played.");
        }
        else
        {
            mainAudioSource.pitch = Time.timeScale;
        }

        sounds = new List<Sound>();

        foreach (GeneralSound sound in generalSounds)
        {
            if (sounds.Exists(x => x.name == sound.name))
            {
                Debug.LogWarning("Duplicate sound name, only first will be considered.");
                continue;
            }

            sound.AddSource(mainAudioSource);

            sounds.Add(sound);
        }

        foreach (IndividualSound sound in individualSounds)
        {
            if (sounds.Exists(x => x.name == sound.name))
            {
                Debug.LogWarning("Duplicate sound name, only first will be considered.");
                continue;
            }

            sound.CreateSource(gameObject);

            sounds.Add(sound);
        }
    }

    /// <summary>
    /// Plays a sound previously added to the AudioManager.
    /// </summary>
    /// <param name="soundName">Name of the sound to be played.</param>
    public void PlaySound(string soundName)
    {
        Sound s = sounds.Find(x => x.name == soundName);

        if (s == null)
        {
            Debug.LogWarning(soundName + " sound not found.");
            return;
        }

        s.Play();
    }

    /// <summary>
    /// Stops a sound if it is currently playing.
    /// </summary>
    /// <param name="soundName">Name of the sound to be stopped.</param>
    public void StopSound(string soundName)
    {
        Sound s = sounds.Find(x => x.name == soundName);

        IndividualSound inds = s as IndividualSound;

        if (inds == null)
        {
            Debug.LogWarning(soundName + " individual sound not found.");
            return;
        }

        inds.Stop();
    }

    /// <summary>
    /// Checks if a sound is currently playing.
    /// </summary>
    /// <param name="soundName">Name of the sound to be checked.</param>
    /// <returns>If the specified sound is currently playing.</returns>
    public bool IsSoundPlaying(string soundName)
    {
        Sound s = sounds.Find(x => x.name == soundName);

        IndividualSound inds = s as IndividualSound;

        if (inds == null)
        {
            Debug.LogWarning(soundName + " individual sound not found.");
            return false;
        }

        return inds.IsPlaying();
    }

    /// <summary>
    /// Changes the pitch of all of the sounds in the AudioManager.
    /// </summary>
    /// <param name="newPitch">New pitch value.</param>
    public void ChangePitchAll(float newPitch)
    {
        mainAudioSource.pitch = newPitch;

        foreach (IndividualSound item in individualSounds)
        {
            item.ChangePitch(newPitch);
        }
    }

    /// <summary>
    /// Receives events to change pitch of all sounds.
    /// </summary>
    private void AdjustPitchToTime()
    {
        ChangePitchAll(Time.timeScale);
    }

    private void OnDestroy()
    {
        TimeController.OnTimeScaleChange -= AdjustPitchToTime;
    }
}
