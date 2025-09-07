using UnityEngine;
using UnityEngine.Playables;

public class Screamer3199 : MonoBehaviour
{
    public PlayableDirector Timeline3199;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Lance la cinématique
            if (Timeline3199 != null)
            {
                Timeline3199.stopped += OnTimelineFinished;
                Timeline3199.Play();
            }
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        Timeline3199.stopped -= OnTimelineFinished;
        Destroy(gameObject);
    }
}
