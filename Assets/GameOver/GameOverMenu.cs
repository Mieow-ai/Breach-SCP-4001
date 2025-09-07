using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    public string gameScene = "Breach";

    void Start()
    {
        // Affiche et libère le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Replay()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
