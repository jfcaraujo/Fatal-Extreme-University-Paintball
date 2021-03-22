using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource mainAudioSource;

    // General sounds play in the main audio source
    public GeneralSound[] generalSounds;

    // Individual sounds play on their own audio source
    public IndividualSound[] individualSounds;

    private List<Sound> sounds;

    private void Awake()
    {
        if (mainAudioSource == null)
        {
            Debug.LogWarning("No main audio source specified, general sounds won't be played.");
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
}
