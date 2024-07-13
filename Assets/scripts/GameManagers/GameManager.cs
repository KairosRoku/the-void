using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Array of tags to disable MeshRenderer for
    private string[] tagsToDisable = { "wall", "interactable", "furniture", "enemy" };

    // Ambient music variables
    public AudioClip ambienceClip;  // The ambient music clip
    public float volume = 0.5f;     // Volume of the ambient music

    private AudioSource audioSource;

    void Start()
    {
        // Disable MeshRenderers
        foreach (string tag in tagsToDisable)
        {
            // Find all game objects with the specified tag
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

            // Loop through each object and disable its MeshRenderer
            foreach (GameObject obj in objectsWithTag)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
        }

        // Setup and play ambient music
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ambienceClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        PlayAmbience();
    }

    void Update()
    {
        // Check if the player is holding the 'R' key
        if (Input.GetKey(KeyCode.R))
        {
            // Reset the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Add any other logic that was in the original Update method here
        // For example:
        // YourExistingUpdateLogic();
    }

    public void PlayAmbience()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopAmbience()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }
}
