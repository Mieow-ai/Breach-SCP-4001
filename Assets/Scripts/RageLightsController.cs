using UnityEngine;

public class RageLightsController : MonoBehaviour
{
    public Light[] ceilingLights;
    public float blinkInterval = 0.1f;

    private bool isRage = false;
    private float timer = 0f;
    private bool lightsOn = false;

    void Update()
    {
        if (!isRage)
        {
            SetLights(false);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            lightsOn = !lightsOn;
            SetLights(lightsOn);
        }
    }

    void SetLights(bool state)
    {
        foreach (var light in ceilingLights)
        {
            light.enabled = state;
        }
    }

    public void SetRageState(bool rage)
    {
        isRage = rage;
        if (!rage)
        {
            SetLights(false);
        }
    }

    public bool IsRaging()
    {
        return isRage;
    }
}
