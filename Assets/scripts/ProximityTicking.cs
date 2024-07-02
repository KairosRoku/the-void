using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ProximityTicking : MonoBehaviour
{
    public GameObject player;       // Reference to the player
    public AudioClip tickSound;     // Tick sound clip
    public float maxTickInterval = 6.0f; // Maximum interval between ticks (farthest distance)
    public float minTickInterval = 0.5f; // Minimum interval between ticks (nearest distance)
    public float detectionRange = 10.0f; // Range within which the ticking occurs

    private AudioSource audioSource;
    private Coroutine tickingCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (player != null && tickSound != null)
        {
            // Ensure the audio source settings are correct
            audioSource.loop = false;
            audioSource.playOnAwake = false;

            // Start the coroutine to handle the ticking sound
            tickingCoroutine = StartCoroutine(HandleTickingSound());
        }
    }

    IEnumerator HandleTickingSound()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= detectionRange)
            {
                // Normalize distance to a 0-1 range
                float t = distance / detectionRange;
                // Interpolate tick interval based on distance
                float tickInterval = Mathf.Lerp(minTickInterval, maxTickInterval, t);

                // Play the tick sound
                audioSource.PlayOneShot(tickSound);

                // Wait for the next tick
                yield return new WaitForSeconds(tickInterval);
            }
            else
            {
                // If the player is out of range, wait for a short period before checking again
                yield return new WaitForSeconds(0.5f); // Check again after 0.5 seconds
            }
        }
    }
}
