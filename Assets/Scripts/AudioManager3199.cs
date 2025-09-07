using UnityEngine;

public class MonsterAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audioAmbiance;
    public AudioSource audioSFX;

    [Header("Clips")]
    public AudioClip clipAmbiance;
    public AudioClip clipPasLents;
    public AudioClip clipPasRapides;
    public AudioClip clipCriAttaque;

    public void PlayAmbiance()
    {
        if (!audioAmbiance.isPlaying)
        {
            audioAmbiance.clip = clipAmbiance;
            audioAmbiance.loop = true;
            audioAmbiance.Play();
        }
    }

    public void PlayPasLents()
    {
        if (audioSFX.clip != clipPasLents || !audioSFX.isPlaying)
        {
            audioSFX.clip = clipPasLents;
            audioSFX.loop = true;
            audioSFX.Play();
        }
    }

    public void PlayPasRapides()
    {
        if (audioSFX.clip != clipPasRapides || !audioSFX.isPlaying)
        {
            audioSFX.clip = clipPasRapides;
            audioSFX.loop = true;
            audioSFX.Play();
        }
    }

    public void StopPas()
    {
        if (audioSFX.clip == clipPasLents || audioSFX.clip == clipPasRapides)
        {
            audioSFX.Stop();
        }
    }

    public void PlayCriAttaque()
    {
        audioSFX.loop = false;
        audioSFX.clip = clipCriAttaque;
        audioSFX.Play();
    }

}



