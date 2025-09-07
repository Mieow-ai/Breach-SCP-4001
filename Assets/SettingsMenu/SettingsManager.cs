using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;
    public Slider sensitivitySlider;

    public string mainMenuScene = "MainMenu";

    void Start()
    {
        if (GameSettings.Instance == null)
        {
            GameObject go = new GameObject("GameSettings");
            go.AddComponent<GameSettings>();
        }

        // Initialise les sliders avec les valeurs actuelles de GameSettings
        masterSlider.value = GameSettings.Instance.masterVolume;
        musicSlider.value = GameSettings.Instance.musicVolume;
        sfxSlider.value = GameSettings.Instance.sfxVolume;
        brightnessSlider.value = GameSettings.Instance.brightness;
        sensitivitySlider.value = GameSettings.Instance.mouseSensitivity;
    }

    // ------------------------------
    // Sliders events : appliquer en direct
    // ------------------------------
    public void OnMasterVolumeChanged(float value)
    {
        GameSettings.Instance.masterVolume = value;
        ApplyAudio();
    }

    public void OnMusicVolumeChanged(float value)
    {
        GameSettings.Instance.musicVolume = value;
        ApplyAudio();
    }

    public void OnSFXVolumeChanged(float value)
    {
        GameSettings.Instance.sfxVolume = value;
        ApplyAudio();
    }

    public void OnBrightnessChanged(float value)
    {
        GameSettings.Instance.brightness = value;
        GameSettings.Instance.ApplyBrightness();
    }

    public void OnSensitivityChanged(float value)
    {
        GameSettings.Instance.mouseSensitivity = value;
    }

    // ------------------------------
    // Boutons
    // ------------------------------
    public void ApplySettings()
    {
        GameSettings.Instance.masterVolume = masterSlider.value;
        GameSettings.Instance.musicVolume = musicSlider.value;
        GameSettings.Instance.sfxVolume = sfxSlider.value;
        GameSettings.Instance.brightness = brightnessSlider.value;
        GameSettings.Instance.mouseSensitivity = sensitivitySlider.value;

        GameSettings.Instance.SaveSettings();
        ApplyAudio();
        GameSettings.Instance.ApplyBrightness();

        // Retour au menu
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ResetToDefaults()
    {
        masterSlider.value = 1f;
        musicSlider.value = 0.7f;
        sfxSlider.value = 0.7f;
        brightnessSlider.value = 1f;
        sensitivitySlider.value = 100f;

        ApplySettings();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    // ------------------------------
    // Méthode privée pour appliquer le son en direct
    // ------------------------------
    private void ApplyAudio()
    {
        if (MenuMusicManager.Instance != null)
        {
            // Music et Master appliqués sur le AudioSource MenuMusicManager
            MenuMusicManager.Instance.ApplyVolumes();
        }

        // SFX et Master appliqués sur ton AudioManager (si tu en as un)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ApplyVolumes(
                GameSettings.Instance.masterVolume,
                GameSettings.Instance.musicVolume,
                GameSettings.Instance.sfxVolume
            );
        }

        // Volume global
        AudioListener.volume = GameSettings.Instance.masterVolume;
    }
}


