using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance { get; private set; }
    private AudioSource musicSource;

    // Les scènes où la musique doit continuer
    private string[] menuScenes = { "MainMenu", "SettingMenu" };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;

        if (!musicSource.isPlaying)
            musicSource.Play();

        ApplyVolumes();

        // Écoute du changement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool shouldKeepMusic = false;

        foreach (string menuScene in menuScenes)
        {
            if (scene.name == menuScene)
            {
                shouldKeepMusic = true;
                break;
            }
        }

        if (!shouldKeepMusic)
        {
            musicSource.Stop();
            Destroy(gameObject); // Détruire ce singleton
        }
    }

    public void ApplyVolumes()
    {
        if (GameSettings.Instance == null) return;

        AudioListener.volume = GameSettings.Instance.masterVolume;
        musicSource.volume = GameSettings.Instance.musicVolume;
    }
}




