using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }
}