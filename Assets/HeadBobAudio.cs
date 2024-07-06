using UnityEngine;

public class HeadBobAudio : MonoBehaviour
{
    public FirstPersonController firstPersonController; // Reference to the FirstPersonController script
    public AudioSource audioSource; // AudioSource component to play the audio clip

    private float previousY; // To store the previous Y position of the head

    void Start()
    {
        // Initialize previousY with the initial Y position of the head
        previousY = firstPersonController.Head.localPosition.y;
    }

    void Update()
    {
        // Get the current Y position of the head
        float currentY = firstPersonController.Head.localPosition.y;

        // Check if the head is at the peak (when it changes direction from going up to down)
        if (currentY < previousY)
        {
            // Play the audio clip at the peak
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        // Update previousY for the next frame
        previousY = currentY;
    }
}
