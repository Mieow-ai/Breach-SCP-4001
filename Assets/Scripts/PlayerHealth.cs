using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Animator animator;
    public float deathDelay = 3f;
    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die"); // <-- joue ton anim de mort
            GetComponent<Animate>()?.SetDead(true);
        }

        // Recharge la scène après un délai
        //Invoke(nameof(RestartScene), deathDelay);
    }

    /*
    private void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    */
}
