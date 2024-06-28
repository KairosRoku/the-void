using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10.0f;
    public float retreatDistance = 5.0f;
    public float viewAngle = 45.0f;
    public LayerMask playerLayer;

    private NavMeshAgent agent;
    private Transform player;
    private bool playerInSight;
    private bool playerFound;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerFound = false;

        // Delay the search for the player by 1 second
        Invoke("FindPlayer", 1.0f);
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerFound = true;
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the player is tagged as 'Player'.");
        }
    }

    void Update()
    {
        if (!playerFound)
        {
            return; // Exit Update if player has not been found yet
        }

        playerInSight = false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= viewAngle / 2)
            {
                if (!Physics.Linecast(transform.position, player.position, playerLayer))
                {
                    playerInSight = true;
                }
            }
        }

        if (playerInSight)
        {
            Retreat();
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void Retreat()
    {
        if (player != null)
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
            Vector3 retreatPosition = transform.position + directionAwayFromPlayer * retreatDistance;
            agent.SetDestination(retreatPosition);
        }
    }
}
