using UnityEngine;

[System.Serializable]
public class IndividualSound : Sound
{
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    public bool loop;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    public AudioRolloffMode audioRolloffMode;

    public float minDistance = 1f;
    public float maxDistance = 500f;

    public void CreateSource(GameObject obj)
    {
        source = obj.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;

        source.spatialBlend = spatialBlend;
        source.rolloffMode = audioRolloffMode;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
    }

    public override void Play()
    {
        if (source == null) return;

        source.Play();
    }

    public void Stop()
    {
        if (source == null) return;

        source.Stop();
    }

    public bool IsPlaying()
    {
        if (source == null) return false;

        return source.isPlaying;
    }
}
