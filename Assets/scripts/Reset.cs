using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // If you're using Text, otherwise use TMPro if using TextMeshPro

public class Reset : MonoBehaviour
{
    public GameObject youDiedText; // Assign in inspector
    public GameObject clickPromptText; // Assign in inspector
    private bool gamePaused = false;

    void Start()
    {
        youDiedText.SetActive(false);
        clickPromptText.SetActive(false);
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
            StartCoroutine(ShowYouDiedMessage());
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        gamePaused = true;
    }

    IEnumerator ShowYouDiedMessage()
    {
        youDiedText.SetActive(true);
        clickPromptText.SetActive(true);

        CanvasGroup canvasGroup = youDiedText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = youDiedText.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // Ensure fully visible

        clickPromptText.SetActive(true);
    }
}
