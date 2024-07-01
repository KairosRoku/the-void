using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10.0f;
    public float viewAngle = 45.0f;
    public LayerMask playerLayer;
    public float stunDuration = 20.0f;
    public float stunCooldown = 2.0f;
    public float wanderRadius = 20.0f;
    public float wanderTimer = 5.0f;

    private NavMeshAgent agent;
    private Transform player;
    private bool playerFound;
    private bool isStunned;
    private bool canBeStunned = true;
    private bool isWandering = true;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerFound = false;

        // Delay the search for the player by 1 second
        Invoke("FindPlayer", 1.0f);

        // Start wandering
        timer = wanderTimer;
        StartCoroutine(Wander());
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
        if (isStunned || !playerFound)
        {
            return;
        }

        if (isWandering)
        {
            return;
        }

        if (IsPlayerInSight())
        {
            Debug.Log("Player in sight, chasing...");
            FollowPlayer();
        }
        else
        {
            agent.isStopped = true;
            Debug.Log("Player not in sight, stopping...");
        }
    }

    bool IsPlayerInSight()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is null.");
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= viewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Player detected by raycast.");
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Raycast hit something else: " + hit.collider.name);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Player is outside of view angle.");
            }
        }
        else
        {
            Debug.LogWarning("Player is out of detection range.");
        }

        return false;
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            Debug.Log("Chasing player to position: " + player.position);
        }
    }

    public void HitByBullet()
    {
        Debug.Log("Enemy hit by bullet.");
        if (isWandering)
        {
            isWandering = false;
            Debug.Log("Enemy stops wandering and starts chasing the player.");
        }
        if (canBeStunned)
        {
            StartCoroutine(StunEnemy());
        }
    }

    IEnumerator Wander()
    {
        float wanderDuration = 60.0f;
        float startTime = Time.time;

        while (Time.time - startTime < wanderDuration)
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                Debug.Log("Wandering to position: " + newPos);
                timer = 0;
            }

            yield return null;
        }

        isWandering = false;
        Debug.Log("Wander duration ended. Enemy starts chasing the player if in sight.");
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
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
