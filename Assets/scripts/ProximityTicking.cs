using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ProximityTicking : MonoBehaviour
{
    public AudioClip tickSound; // Tick sound clip
    public float maxTickInterval = 6.0f; // Maximum interval between ticks (farthest distance)
    public float minTickInterval = 0.5f; // Minimum interval between ticks (nearest distance)
    public float detectionRange = 10.0f; // Range within which the ticking occurs

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

        StartCoroutine(FindGem());
    }

    IEnumerator FindGem()
    {
        while (gem == null)
        {
            gem = GameObject.FindGameObjectWithTag("gem");
            if (gem == null)
            {
                Debug.Log("Gem not found. Retrying...");
                yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
            }
            else
            {
                Debug.Log("Gem found!");
            }
        }
    }

    void Update()
    {
        if (gem == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, gem.transform.position);

        if (distance <= detectionRange)
        {
            if (!isInRange)
            {
                // Start the coroutine if the player enters the detection range
                isInRange = true;
                if (tickingCoroutine == null)
                {
                    tickingCoroutine = StartCoroutine(HandleTickingSound());
                    Debug.Log("Player entered detection range.");
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
                    Debug.Log("Player exited detection range.");
                }
            }
        }
    }

    IEnumerator HandleTickingSound()
    {
        while (isInRange)
        {
            float distance = Vector3.Distance(transform.position, gem.transform.position);
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
