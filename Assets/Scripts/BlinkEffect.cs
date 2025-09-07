using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkEffect : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("L'image noire/transparente pour les paupières")]
    public Image blinkOverlay;
    [Tooltip("Le Canvas à afficher pendant le clignement")]
    public Canvas targetCanvas;

    [Header("Paramètres du Clignement")]
    [Range(0.1f, 10f)] public float blinkInterval = 3f;
    [Range(0.1f, 2f)] public float blinkDuration = 0.4f;
    [Range(0.01f, 0.5f)] public float canvasShowTime = 0.1f;

    [Header("Effet de Fade")]
    [Tooltip("Courbe pour adoucir l'ouverture/fermeture")]
    public AnimationCurve blinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float _timer;

    void Start()
    {
        InitBlinkSystem();
    }

    void InitBlinkSystem()
    {
        if (blinkOverlay != null)
        {
            SetAlpha(0f); // Commence transparent (yeux ouverts)
            blinkOverlay.raycastTarget = false; // Désactive les clics UI
        }

        if (targetCanvas != null)
            targetCanvas.enabled = false;
    }

    void Update()
    {
        HandleAutomaticBlink();
        HandleManualBlink();
    }

    void HandleAutomaticBlink()
    {
        _timer += Time.deltaTime;
        if (_timer >= blinkInterval)
        {
            _timer = 0f;
            StartCoroutine(PerformBlink());
        }
    }

    void HandleManualBlink()
    {
        if (Input.GetKeyDown(KeyCode.B))
            StartCoroutine(PerformBlink());
    }

    IEnumerator PerformBlink()
    {
        // Fermeture progressive avec la courbe
        yield return StartCoroutine(AnimateBlink(0f, 1f, blinkDuration / 2));

        // Active le Canvas au moment où les yeux sont fermés
        if (targetCanvas != null)
        {
            targetCanvas.enabled = true;
            yield return new WaitForSeconds(canvasShowTime);
            targetCanvas.enabled = false;
        }

        // Réouverture progressive
        yield return StartCoroutine(AnimateBlink(1f, 0f, blinkDuration / 2));
    }

    IEnumerator AnimateBlink(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = blinkCurve.Evaluate(t);
            SetAlpha(Mathf.Lerp(startAlpha, endAlpha, curveValue));
            yield return null;
        }
        SetAlpha(endAlpha); // Assure la valeur finale exacte
    }

    void SetAlpha(float alpha)
    {
        if (blinkOverlay == null) return;

        Color color = blinkOverlay.color;
        color.a = alpha;
        blinkOverlay.color = color;

        // Optionnel : Change aussi la couleur pour un effet plus doux
        if (alpha > 0.5f)
            color = Color.Lerp(Color.clear, Color.black, (alpha - 0.5f) * 2f);

        blinkOverlay.color = color;
    }
}
