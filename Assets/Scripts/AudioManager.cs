using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private readonly List<AudioSource> allAudioSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterAudioSource(AudioSource source)
    {
        if (source != null && !allAudioSources.Contains(source))
            allAudioSources.Add(source);
    }

    public void UnregisterAudioSource(AudioSource source)
    {
        if (allAudioSources.Contains(source))
            allAudioSources.Remove(source);
    }

    public void ApplyVolumes(float masterVolume, float musicVolume, float sfxVolume)
    {
        foreach (var src in allAudioSources)
        {
            if (src == null) continue;

            if (src.CompareTag("Music"))
                src.volume = musicVolume * masterVolume;
            else
                src.volume = sfxVolume * masterVolume;
        }
    }
}