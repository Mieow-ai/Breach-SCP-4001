using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource calmAudioSource;       // Pour l’ambiance calme
    public AudioSource rageAmbianceAudio;     // Pour l’ambiance de rage
    public AudioSource screamAudioSource;     // Pour le cri (one shot)

    [Header("Audio Clips")]
    public AudioClip calmAmbience;
    public AudioClip rageAmbience;
    public AudioClip screamAmbience;

    void Start()
    {
        PlayCalmAmbience();
    }

    public void PlayCalmAmbience()
    {
        if (calmAudioSource != null && calmAmbience != null)
        {
            calmAudioSource.clip = calmAmbience;
            calmAudioSource.loop = true;
            calmAudioSource.Play();
        }
    }

    public void PlayRageAmbience()
    {
        // Stop calm ambience if playing
        if (calmAudioSource != null && calmAudioSource.isPlaying)
        {
            calmAudioSource.Stop();
        }

        // Play scream once
        if (screamAudioSource != null && screamAmbience != null)
        {
            screamAudioSource.PlayOneShot(screamAmbience);
        }

        // Play looping rage ambience
        if (rageAmbianceAudio != null && rageAmbience != null)
        {
            rageAmbianceAudio.clip = rageAmbience;
            rageAmbianceAudio.Play();
        }
    }

    public void StopAll()
    {
        calmAudioSource?.Stop();
        rageAmbianceAudio?.Stop();
        screamAudioSource?.Stop();
    }

    public void StopRageSound()
    {
        if (screamAudioSource != null && screamAudioSource.isPlaying)
            screamAudioSource.Stop();

        if (rageAmbianceAudio != null && rageAmbianceAudio.isPlaying)
            rageAmbianceAudio.Stop();
    }
}