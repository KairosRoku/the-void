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
    public GameObject gem; // Make gem public

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
        StartCoroutine(FindGem());
    }

    IEnumerator FindGem()
    {
        while (true)
        {
            gem = GameObject.FindGameObjectWithTag("gem");
            if (gem == null)
            {
                if (isInRange)
                {
                    isInRange = false;
                    if (tickingCoroutine != null)
                    {
                        StopCoroutine(tickingCoroutine);
                        tickingCoroutine = null;
                    }
                }
                yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
            }
            else
            {
                if (tickingCoroutine == null && isInRange)
                {
                    tickingCoroutine = StartCoroutine(HandleTickingSound());
                }
                yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
            }
        }
    }

    void Update()
    {
        if (gem == null)
        {
            // If no gem is found, the ticking follows the player
            float distance = 0f;
            HandleTicking(distance);
            return;
        }

        float gemDistance = Vector3.Distance(transform.position, gem.transform.position);
        HandleTicking(gemDistance);
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
        while (isInRange)
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
