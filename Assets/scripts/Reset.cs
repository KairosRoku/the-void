using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Reset : MonoBehaviour
{
    public GameObject youDiedText; // Assign in inspector
    public GameObject clickPromptText; // Assign in inspector
    public Image blackScreen; // Assign in inspector
    public AudioSource deathAudioSource; // Assign in inspector
    private bool gamePaused = false;

    void Start()
    {
        youDiedText.SetActive(false);
        clickPromptText.SetActive(false);
        blackScreen.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gamePaused && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("Player hit the reset trigger");
            PauseGame();
            PlayDeathAudio();
            StartCoroutine(ShowDeathUI());
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        gamePaused = true;
    }

    void PlayDeathAudio()
    {
        if (deathAudioSource != null)
        {
            deathAudioSource.Play();
        }
    }

    IEnumerator ShowDeathUI()
    {
        blackScreen.gameObject.SetActive(true);
        youDiedText.SetActive(true);
        clickPromptText.SetActive(true);

        CanvasGroup youDiedCanvasGroup = youDiedText.GetComponent<CanvasGroup>();
        if (youDiedCanvasGroup == null)
        {
            youDiedCanvasGroup = youDiedText.AddComponent<CanvasGroup>();
        }

        CanvasGroup clickPromptCanvasGroup = clickPromptText.GetComponent<CanvasGroup>();
        if (clickPromptCanvasGroup == null)
        {
            clickPromptCanvasGroup = clickPromptText.AddComponent<CanvasGroup>();
        }

        CanvasGroup blackScreenCanvasGroup = blackScreen.GetComponent<CanvasGroup>();
        if (blackScreenCanvasGroup == null)
        {
            blackScreenCanvasGroup = blackScreen.gameObject.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            youDiedCanvasGroup.alpha = alpha;
            clickPromptCanvasGroup.alpha = alpha;
            blackScreenCanvasGroup.alpha = alpha;
            yield return null;
        }

        youDiedCanvasGroup.alpha = 1f; // Ensure fully visible
        clickPromptCanvasGroup.alpha = 1f; // Ensure fully visible
        blackScreenCanvasGroup.alpha = 1f; // Ensure fully visible
    }
}
