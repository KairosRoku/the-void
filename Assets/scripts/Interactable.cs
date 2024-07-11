using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isPickup = false; // Set to true if this object is a pickup item
    public bool isDoor = false; // Set to true if this object is a door
    public AudioClip interactionSound; // Audio clip to play on interaction
    public AudioClip doorOpenSound; // Audio clip for door opening
    public AudioClip doorCloseSound; // Audio clip for door closing

    private AudioSource audioSource;
    private Animator doorAnimator;
    private bool isDoorClosed = true; // Track the door's closed/open state

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (isDoor)
        {
            doorAnimator = GetComponent<Animator>();
            doorAnimator.SetBool("isClosed", true); // Ensure the door starts closed
        }
    }

    public void OnInteract()
    {
        if (isDoor)
        {
            ToggleDoor();
        }
        else if (isPickup)
        {
            Pickup();
        }
        else
        {
            PlayInteractionSound();
        }
    }

    void ToggleDoor()
    {
        if (doorAnimator != null)
        {
            isDoorClosed = !isDoorClosed;
            doorAnimator.SetBool("isClosed", isDoorClosed);
            PlayDoorSound();
            Debug.Log("Door " + (isDoorClosed ? "closed" : "opened") + ": " + gameObject.name);
        }
    }

    void Pickup()
    {
        // Add pickup logic if needed
        Debug.Log("Picked up: " + gameObject.name);
        PlayInteractionSound();
    }

    void PlayDoorSound()
    {
        if (audioSource != null)
        {
            if (isDoorClosed && doorCloseSound != null)
            {
                audioSource.PlayOneShot(doorCloseSound);
            }
            else if (!isDoorClosed && doorOpenSound != null)
            {
                audioSource.PlayOneShot(doorOpenSound);
            }
        }
    }

    void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
    }
}
