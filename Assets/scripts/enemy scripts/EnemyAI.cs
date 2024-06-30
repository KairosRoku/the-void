using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10.0f;
    public float retreatDistance = 5.0f;
    public float viewAngle = 45.0f;
    public LayerMask playerLayer;
    public float stunDuration = 20.0f;
    public float stunCooldown = 2.0f;

    private NavMeshAgent agent;
    private Transform player;
    private bool playerInSight;
    private bool playerFound;
    private bool isStunned;
    private bool canBeStunned = true;

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
            Debug.Log("Player found.");
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the player is tagged as 'Player'.");
        }
    }

    void Update()
    {
        if (!playerFound || isStunned)
        {
            return;
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

    public void HitByBullet()
    {
        Debug.Log("Enemy hit by bullet.");
        if (canBeStunned)
        {
            StartCoroutine(StunEnemy());
        }
    }

    IEnumerator StunEnemy()
    {
        isStunned = true;
        canBeStunned = false;
        agent.isStopped = true;
        Debug.Log("Enemy stunned.");

        // Wait for the stun duration
        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        agent.isStopped = false;
        Debug.Log("Enemy stun duration ended.");

        // Wait for the cooldown before the enemy can be stunned again
        yield return new WaitForSeconds(stunCooldown);

        canBeStunned = true;
        Debug.Log("Enemy can be stunned again.");
    }
}
