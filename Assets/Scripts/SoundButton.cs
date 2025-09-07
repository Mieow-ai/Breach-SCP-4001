using UnityEngine;

public class UIButtonSFX : MonoBehaviour
{
    [Header("Audio à jouer")]
    public AudioClip clickSound;

    [Header("AudioSource à utiliser")]
    public AudioSource audioSource;

    public void PlayClickSFX()
    {
        if (clickSound == null)
        {
            Debug.LogWarning("Aucun clip assigné pour le bouton !");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("Aucun AudioSource assigné pour le bouton !");
            return;
        }

        audioSource.PlayOneShot(clickSound);
    }
}



