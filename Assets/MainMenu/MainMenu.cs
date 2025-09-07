using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string settingScene = "SettingScene";
    public string playScene = "Breach";

    public void Play()
    {
        SceneManager.LoadScene(playScene);
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene(settingScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}


