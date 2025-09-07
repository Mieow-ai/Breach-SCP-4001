using UnityEngine;
using UnityEngine.AI;

public class RandomWander : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval)
        {
            Vector3 newPos = GetRandomNavMeshPosition(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            randomPos.y = center.y; // Garde le niveau de hauteur constant

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
                return hit.position;
        }

        return center; // fallback si aucun point trouvé
    }
}
