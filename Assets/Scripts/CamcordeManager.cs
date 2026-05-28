using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CamcorderUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI recText;
    public TextMeshProUGUI clockText;
    public Image batteryBar;
    public TextMeshProUGUI playText;
    public TextMeshProUGUI refillPromptText;

    [Header("Battery Settings")]
    public float batteryLife = 100f;
    public float drainRate = 5f;

    [Header("Battery Sprites")]
    public Sprite batteryFull;
    public Sprite battery50;
    public Sprite battery25;
    public Sprite batteryEmpty;

    [Header("Flashlight")]
    public FlashlightToggle flashlightScript;

    [Header("Inventory")]
    public BatteryInventory batteryInventory;

    private bool isRecording = true;

    void Start()
    {
        InvokeRepeating(nameof(UpdateClock), 0f, 1f);
        StartCoroutine(BlinkREC());

        if (refillPromptText != null)
            refillPromptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (flashlightScript != null && flashlightScript.IsFlashlightOnAndActive)
        {
            batteryLife -= drainRate * Time.deltaTime;
            batteryLife = Mathf.Clamp(batteryLife, 0f, 100f);

            if (batteryLife <= 0f)
            {
                flashlightScript.ForceTurnOffAndLock();
                isRecording = false;
                recText.enabled = false;
                playText.gameObject.SetActive(true);
            }
        }

        UpdateBatteryIcon();
        batteryBar.fillAmount = batteryLife / 100f;

        if (refillPromptText != null && batteryLife <= 0f && batteryInventory != null && batteryInventory.batteryCount > 0)
        {
            refillPromptText.gameObject.SetActive(true);
        }
        else if (refillPromptText != null)
        {
            refillPromptText.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.R))
            ToggleRecording();
    }

    void UpdateClock()
    {
        clockText.text = DateTime.Now.ToString("HH:mm:ss");
    }

    System.Collections.IEnumerator BlinkREC()
    {
        while (true)
        {
            if(recText != null)
                recText.enabled = !recText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ToggleRecording()
    {
        isRecording = !isRecording;
        playText.gameObject.SetActive(!isRecording);
    }

    void UpdateBatteryIcon()
    {
        if (batteryLife > 75) batteryBar.sprite = batteryFull;
        else if (batteryLife > 25) batteryBar.sprite = battery50;
        else if (batteryLife > 5) batteryBar.sprite = battery25;
        else batteryBar.sprite = batteryEmpty;
    }

    public void RechargeBattery(float amount)
    {
        batteryLife = Mathf.Clamp(batteryLife + amount, 0f, 100f);
        UpdateBatteryIcon();
        batteryBar.fillAmount = batteryLife / 100f;
    }
}




