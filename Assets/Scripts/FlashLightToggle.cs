using UnityEngine;
using System.Collections;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;
    public AudioSource audioSource;
    public AudioClip toggleOnSound;
    public AudioClip toggleOffSound;

    [Header("UI Controller")]
    public FlashlightUIController UIControllerLight;

    [Header("Battery System")]
    public CamcorderUIManager camcorderUI;

    private bool isOn = false;           // Lampe allumée ou non
    private bool isLocked = false;       // Verrouillage batterie vide
    private bool emptyBatterySoundPlayed = false;

    public float messageDisplayTime = 3f;

    public bool IsFlashlightOnAndActive => isOn && !isLocked && flashlight.enabled;

    void Update()
    {
        // Si verrouillée, ne fait rien sauf recharge
        if (isLocked)
            return;

        // Activation / désactivation avec F
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (camcorderUI != null && camcorderUI.batteryLife <= 0f)
            {
                if (!emptyBatterySoundPlayed)
                {
                    PlaySound(toggleOffSound);
                    emptyBatterySoundPlayed = true;
                }
                return;
            }

            ToggleFlashlight();
        }
    }

    public void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlight.enabled = isOn;
        PlaySound(isOn ? toggleOnSound : toggleOffSound);
    }

    public void ForceTurnOffAndLock()
    {
        if (!isOn && emptyBatterySoundPlayed) return; // Evite boucle

        isOn = false;
        flashlight.enabled = false;
        isLocked = true;

        PlaySound(toggleOffSound);
        emptyBatterySoundPlayed = true;
    }

    public void Unlock()
    {
        isLocked = false;
        emptyBatterySoundPlayed = false; // permet de rejouer le son si batterie vide plus tard
        if (UIControllerLight != null)
            StartCoroutine(ShowUIFeedback());
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private IEnumerator ShowUIFeedback()
    {
        UIControllerLight.ShowAvailableUI();
        yield return new WaitForSeconds(messageDisplayTime);
        UIControllerLight.HideUI();
    }
}





