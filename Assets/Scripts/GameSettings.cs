using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    [Header("Volumes")]
    public float masterVolume = 1f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 0.7f;

    [Header("Autres")]
    public float brightness = 1f;
    public float mouseSensitivity = 100f;
    public bool introDone = false; // Flag persistant

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1f;
        musicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 0.7f;
        sfxVolume = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : 0.7f;
        brightness = PlayerPrefs.HasKey("Brightness") ? PlayerPrefs.GetFloat("Brightness") : 1f;
        mouseSensitivity = PlayerPrefs.HasKey("MouseSensitivity") ? PlayerPrefs.GetFloat("MouseSensitivity") : 100f;

        // sécurité : si une valeur est en dehors des bornes, on remet les défauts
        if (masterVolume <= 0f) masterVolume = 1f;
        if (musicVolume < 0f) musicVolume = 0.7f;
        if (sfxVolume < 0f) sfxVolume = 0.7f;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("Brightness", brightness);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();
    }

    public void ApplyAudio()
    {
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.ApplyVolumes();
        }
    }

    public void ApplyBrightness()
    {
        RenderSettings.ambientLight = Color.white * brightness;
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }
}
