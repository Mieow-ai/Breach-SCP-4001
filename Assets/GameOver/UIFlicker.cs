using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Graphic))]
public class UIFlicker : MonoBehaviour
{
    public float minInterval = 0.05f;
    public float maxInterval = 0.2f;
    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;
    public bool flickerPosition = true;
    public float positionOffset = 2f;

    private Graphic uiGraphic;
    private RectTransform rectTransform;
    private Vector3 originalPos;

    void Start()
    {
        uiGraphic = GetComponent<Graphic>();
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.localPosition;

        StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        while (true)
        {
            // Flicker alpha
            float randomAlpha = Random.Range(minAlpha, maxAlpha);
            Color c = uiGraphic.color;
            c.a = randomAlpha;
            uiGraphic.color = c;

            // Flicker position
            if (flickerPosition)
            {
                float offsetX = Random.Range(-positionOffset, positionOffset);
                float offsetY = Random.Range(-positionOffset, positionOffset);
                rectTransform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            }

            float randomTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(randomTime);

            // Reset position pour ne pas accumuler le décalage
            rectTransform.localPosition = originalPos;
        }
    }
}
