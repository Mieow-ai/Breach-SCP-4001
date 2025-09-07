using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public List<Light> lights;
    public float blinkDuration = 0.1f;
    public float blinkInterval = 0.2f;
    private Coroutine blinkCoroutine;

    void OnEnable()
    {
        StartBlinking();
    }

    void OnDisable()
    {
        StopBlinking();
    }

    public void StartBlinking()
    {
        if (blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkLights());
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        // Allumer les lumières à la fin si nécessaire
        foreach (Light l in lights)
        {
            if (l != null)
                l.enabled = true;
        }
    }

    IEnumerator BlinkLights()
    {
        while (true)
        {
            foreach (Light l in lights)
                if (l != null)
                    l.enabled = !l.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
