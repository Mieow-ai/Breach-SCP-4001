using UnityEngine;
using UnityEngine.Playables; // pour la Timeline
using UnityEngine.SceneManagement;

public class ZoneFin : MonoBehaviour
{
    public PlayableDirector timelineCinematique;
    private PlayerMovementWithFreelook playerMovement;
    private NightVisionManager NVision;
    private FlashlightUIController FlashLight;
    public string sceneToLoad = "Fin";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.TousDossiersRecuperes())
            {
                Debug.Log("Tous les dossiers récupérés ! Lancement de la cinématique.");

                // Récupère le script PlayerMovementWithFreelook sur le joueur
                playerMovement = other.GetComponent<PlayerMovementWithFreelook>();
                if (playerMovement != null)
                    playerMovement.enabled = false; // 🔒 bloque tout (souris + déplacements)

                NVision = other.GetComponent<NightVisionManager>();
                if (NVision != null)
                    NVision.enabled = false; 

                FlashLight = other.GetComponent<FlashlightUIController>();
                if (FlashLight != null)
                    FlashLight.enabled = false; 

                // Lance la cinématique
                if (timelineCinematique != null)
                {
                    timelineCinematique.stopped += OnTimelineFinished;
                    timelineCinematique.Play();
                }
            }
            else
            {
                Debug.Log("Il manque encore des dossiers !");
            }
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        timelineCinematique.stopped -= OnTimelineFinished;
        SceneManager.LoadScene(sceneToLoad);
    }
}
