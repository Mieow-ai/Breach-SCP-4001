using TMPro;
using UnityEngine;
using System.Collections;

public class RadioTextIntro : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI textUI;
    public CanvasGroup canvasGroup;

    [Header("Text Settings")]
    [TextArea]
    public string fullText = "Site-82 → Breach de confinement\nOuest de la Floride";
    public float letterDelay = 0.05f;
    public float displayDuration = 5f;
    public float fadeDuration = 1f;

    [Header("Optional Sound")]
    public AudioSource typingSound;

    void Start()
    {
        canvasGroup.alpha = 1f;
        StartCoroutine(ShowTextSequence());
    }

    IEnumerator ShowTextSequence()
    {
        textUI.text = "";

        // Écriture lettre par lettre
        foreach (char c in fullText)
        {
            textUI.text += c;

            if (typingSound && !char.IsWhiteSpace(c))
                typingSound.PlayOneShot(typingSound.clip);

            yield return new WaitForSeconds(letterDelay);
        }

        // Attendre un peu avant de fade-out
        yield return new WaitForSeconds(displayDuration);

        // Fade-out progressif
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        // Optionnel : désactiver complètement l'objet
        textUI.gameObject.SetActive(false);
    }
}
