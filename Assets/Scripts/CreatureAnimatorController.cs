using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CreatureAnimatorController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Tooltip("Vitesse minimale à partir de laquelle on considère que la créature est en mouvement.")]
    public float movementThreshold = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        bool isMoving = speed > movementThreshold;
        animator.SetBool("IsMoving", isMoving);
    }
}
