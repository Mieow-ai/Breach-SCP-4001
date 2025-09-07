using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class NightVisionManager : MonoBehaviour
{
    [Header("Effets visuels")]
    public Volume nightVisionVolume;
    public GameObject nightVisionOverlay;

    [Header("Sons")]
    public AudioSource audioSourceOn;
    public AudioSource audioSourceOff;

    private bool isNightVisionOn = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleNightVision();
        }
    }

    void ToggleNightVision()
    {
        isNightVisionOn = !isNightVisionOn;

        // Active/Désactive le volume post-processing
        if (nightVisionVolume != null)
            nightVisionVolume.enabled = isNightVisionOn;

        if (isNightVisionOn)
        {
            // Active l'overlay et joue le son ON
            if (nightVisionOverlay != null)
                nightVisionOverlay.SetActive(true);

            if (audioSourceOn != null)
                audioSourceOn.Play();

            Debug.Log("Night Vision ON – son joué.");
        }
        else
        {
            // Joue le son OFF et désactive l'overlay après un petit délai
            if (audioSourceOff != null)
                audioSourceOff.Play();

            StartCoroutine(DisableOverlayAfterSound());

            Debug.Log("Night Vision OFF – son joué.");
        }
    }

    IEnumerator DisableOverlayAfterSound()
    {
        yield return new WaitForSeconds(0.3f); // Ajuste en fonction de ton son
        if (nightVisionOverlay != null)
            nightVisionOverlay.SetActive(false);
    }
}
