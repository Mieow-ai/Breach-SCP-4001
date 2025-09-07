using UnityEngine;

public class Screamer087 : MonoBehaviour
{
    public Animator cubeAnimator;
    private string triggerName = "PlayScreamer";
    public bool destroyAfterTrigger = false;
    public AudioSource ScreamerAudioManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(cubeAnimator != null)
            {
                cubeAnimator.SetTrigger(triggerName);
                ScreamerAudioManager.Play();
            }

            if (destroyAfterTrigger)
            {
                Destroy(gameObject);
            }
        }

    }
}
