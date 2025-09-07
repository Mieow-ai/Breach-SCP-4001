using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string sceneToLoad = "NomDeTaSceneJeu"; // ou index dans Build Settings
    public float delayBeforeLoad = 2f; // Petit délai

    public CanvasGroup loadingScreen; // Pour fade-in écran noir (optionnel)

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        // Fade écran noir si tu veux
        if (loadingScreen != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                loadingScreen.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(delayBeforeLoad);

        // Charger la scène
        SceneManager.LoadScene(sceneToLoad);
    }
}
