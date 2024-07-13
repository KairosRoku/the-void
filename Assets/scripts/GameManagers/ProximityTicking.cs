using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ProximityTicking : MonoBehaviour
{
    public AudioClip tickSound; // Tick sound clip
    public float maxTickInterval = 6.0f; // Maximum interval between ticks (farthest distance)
    public float minTickInterval = 0.5f; // Minimum interval between ticks (nearest distance)
    public float detectionRange = 10.0f; // Range within which the ticking occurs
    public float initialDelay = 1.0f; // Initial delay before ticking starts

    private AudioSource audioSource;
    private Coroutine tickingCoroutine;
    private bool isInRange = false;
    private bool gemAssigned = false;
    private GameObject player;
    public GameObject gem { get; private set; } // Make gem public with private set

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (tickSound != null)
        {
            audioSource.clip = tickSound; // Set the tick sound
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(initialDelay);
        StartCoroutine(FindPlayerAndGem());
    }

    IEnumerator FindPlayerAndGem()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
        }

        while (!gemAssigned)
        {
            gem = GameObject.FindGameObjectWithTag("gem");
            if (gem != null)
            {
                gemAssigned = true;
                StartTicking();
            }
            yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
        }
    }

    public void SetGem(GameObject newGem)
    {
        gem = newGem;
        gemAssigned = true;
        StartTicking();
    }

    void StartTicking()
    {
        if (gameObject.activeInHierarchy && tickingCoroutine == null)
        {
            tickingCoroutine = StartCoroutine(HandleTickingSound());
        }
    }

    void Update()
    {
        if (!gemAssigned)
            return;

        if (gem != null)
        {
            float gemDistance = Vector3.Distance(transform.position, gem.transform.position);
            HandleTicking(gemDistance);
        }
        else
        {
            HandleTicking(0f); // Gem is destroyed, follow the player with max ticking
        }
    }

    void HandleTicking(float distance)
    {
        if (distance <= detectionRange)
        {
            if (!isInRange)
            {
                // Start the coroutine if the player enters the detection range
                isInRange = true;
                if (tickingCoroutine == null)
                {
                    tickingCoroutine = StartCoroutine(HandleTickingSound());
                }
            }
        }
        else
        {
            if (isInRange)
            {
                // Stop the coroutine if the player exits the detection range
                isInRange = false;
                if (tickingCoroutine != null)
                {
                    StopCoroutine(tickingCoroutine);
                    tickingCoroutine = null;
                }
            }
        }
    }

    IEnumerator HandleTickingSound()
    {
        while (true)
        {
            float distance = gem != null ? Vector3.Distance(transform.position, gem.transform.position) : 0f;
            float t = Mathf.Clamp01(distance / detectionRange);
            float tickInterval = Mathf.Lerp(minTickInterval, maxTickInterval, Mathf.Log10(t + 1) / Mathf.Log10(2));

            audioSource.PlayOneShot(tickSound);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    void OnDrawGizmos()
    {
        if (gem != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gem.transform.position, detectionRange);
        }
    }
}
