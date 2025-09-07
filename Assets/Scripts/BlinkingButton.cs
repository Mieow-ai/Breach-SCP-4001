using UnityEngine;
using UnityEngine.UI; // N'oublie pas ce using pour RawImage
using System.Collections;

public class BlinkingRawImage : MonoBehaviour
{
    [Header("Paramètres du clignotement")]
    [Tooltip("Couleur lorsque 'allumé'")]
    public Color blinkColor = new Color(1, 0, 0, 1); // Rouge vif par défaut (format RGBA)

    [Tooltip("Vitesse du clignotement en secondes")]
    public float blinkSpeed = 0.5f;

    [Tooltip("Si coché, alterne entre visible/invisible au lieu de changer de couleur")]
    public bool toggleVisibility = false;

    private RawImage rawImageComponent;
    private Color originalColor;
    private bool isBlinking = true;

    void Start()
    {
        // Récupère le composant RawImage
        rawImageComponent = GetComponent<RawImage>();

        if (rawImageComponent == null)
        {
            Debug.LogError("Pas de composant RawImage trouvé sur cet objet !", gameObject);
            return;
        }

        originalColor = rawImageComponent.color;
        StartCoroutine(BlinkEffect());
    }

    IEnumerator BlinkEffect()
    {
        while (isBlinking)
        {
            if (toggleVisibility)
            {
                // Mode visible/invisible
                rawImageComponent.enabled = !rawImageComponent.enabled;
            }
            else
            {
                // Mode changement de couleur
                rawImageComponent.color = (rawImageComponent.color == originalColor) ? blinkColor : originalColor;
            }

            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    // Méthode pour arrêter le clignotement
    public void StopBlinking()
    {
        isBlinking = false;
        rawImageComponent.color = originalColor;
        rawImageComponent.enabled = true;
    }

    // Méthode pour redémarrer le clignotement
    public void RestartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkEffect());
        }
    }
}