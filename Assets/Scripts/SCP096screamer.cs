using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class SCP096Screamer : MonoBehaviour
{
    public GameObject screamerUI;     // UI contenant la RawImage/Video
    public VideoPlayer videoPlayer;   // Composant VideoPlayer
    public string gameOverSceneName = "GameOver";
    public float delayBeforeGameOver = 2f;

    private void Start()
    {
        if (screamerUI != null)
            screamerUI.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.playOnAwake = false;
            videoPlayer.Prepare(); // Prépare la vidéo dès le lancement
        }
    }

    public void PlayScreamer()
    {
        if (screamerUI == null || videoPlayer == null) return;

        screamerUI.SetActive(true);

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.prepareCompleted += PlayPreparedVideo;
            videoPlayer.Prepare();
        }
        else
        {
            PlayPreparedVideo(videoPlayer);
        }

        // 🚨 NOUVEAU : Chrono de secours
        StartCoroutine(FailsafeGameOver());
    }

    private IEnumerator FailsafeGameOver()
    {
        // Met la durée de la vidéo du screamer de 096 ici
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(gameOverSceneName);
    }

    private void PlayPreparedVideo(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= PlayPreparedVideo; // On se désabonne
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(GoToGameOver());
    }

    private IEnumerator GoToGameOver()
    {
        yield return new WaitForSeconds(delayBeforeGameOver);
        SceneManager.LoadScene(gameOverSceneName);
    }
}