using UnityEngine;

public class AmbienceMusic : MonoBehaviour
{
    public AudioClip ambienceClip;  // The ambient music clip
    public float volume = 0.5f;     // Volume of the ambient music

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ambienceClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        PlayAmbience();
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
