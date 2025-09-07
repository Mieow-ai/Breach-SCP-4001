using UnityEngine;

public class Animate : MonoBehaviour
{
    Animator PlayerAnimator;
    private bool isDead = false;

    void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
    }

    public void SetDead(bool dead)
    {
        isDead = dead;
    }

    void Update()
    {
        if (isDead) return;

        PlayerAnimator.SetFloat("walk", Input.GetAxis("Vertical"));
        PlayerAnimator.SetFloat("run", Input.GetAxis("Fire3"));
    }
}
