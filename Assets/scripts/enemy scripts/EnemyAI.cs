using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public LayerMask playerLayer;
    public float stunDuration = 20.0f;
    public float stunCooldown = 2.0f;
    public float wanderRadius = 20.0f;
    public float wanderTimer = 5.0f;
    public AudioClip walkingSound;
    public AudioClip[] audioCues; // Array of audio clips
    public int hitsRequiredForAudio = 10; // Number of hits required to play audio

    private NavMeshAgent agent;
    private Transform player;
    private AudioSource audioSource;
    private bool playerFound;
    private bool isStunned;
    private bool canBeStunned = true;
    private bool isWandering = true;
    private float timer;
    private int hitCount = 0; // Hit counter

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
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
            StopWalkingSound();
            return;
        }

        if (isWandering)
        {
            return;
        }

        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            PlayWalkingSound();
            Debug.Log("Chasing player to position: " + player.position);
        }
        else
        {
            StopWalkingSound();
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

        // Increment the hit count
        hitCount++;

        // Check if the enemy has reached the required number of hits to play the audio cue
        if (hitCount >= hitsRequiredForAudio)
        {
            PlayRandomAudioCue();
            hitCount = 0; // Reset the hit count after playing the audio cue
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
                PlayWalkingSound();
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
        StopWalkingSound();
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

    void PlayWalkingSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = walkingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopWalkingSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void PlayRandomAudioCue()
    {
        if (audioCues.Length > 0)
        {
            AudioClip randomClip = audioCues[Random.Range(0, audioCues.Length)];
            audioSource.PlayOneShot(randomClip);
            Debug.Log("Playing random audio cue.");
        }
    }
}
