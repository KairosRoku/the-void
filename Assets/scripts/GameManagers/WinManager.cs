using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WinManager: MonoBehaviour
{
    public TMP_Text winText; // Assign in inspector
    public TMP_Text restartPrompt; // Assign in inspector
    public Image blackScreen; // Assign in inspector
    private bool gamePaused = false;
    private bool canTrigger = false;

    void Start()
    {
        StartCoroutine(AllowTriggerAfterDelay(3f));
        InitializeUI();
    }

    IEnumerator AllowTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTrigger = true;
    }

    void InitializeUI()
    {
        winText.gameObject.SetActive(false);
        restartPrompt.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gamePaused && Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1f; // Unpause the game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("CollisionTest: Collision detected with: " + collision.gameObject.name);
            CheckGameFinish();
        }
    }

    public void CheckGameFinish()
    {
        if (!canTrigger)
            return;

        if (GameObject.FindGameObjectsWithTag("gem").Length == 0)
        {
            GameFinished();
        }
    }

    void GameFinished()
    {
        gamePaused = true;
        Time.timeScale = 0f; // Pause the game

        StartCoroutine(ShowWinMessage());
    }

    IEnumerator ShowWinMessage()
    {
        blackScreen.gameObject.SetActive(true);
        winText.gameObject.SetActive(true);
        restartPrompt.gameObject.SetActive(true);

        CanvasGroup winCanvasGroup = winText.GetComponent<CanvasGroup>();
        if (winCanvasGroup == null)
        {
            winCanvasGroup = winText.gameObject.AddComponent<CanvasGroup>();
        }

        CanvasGroup blackScreenCanvasGroup = blackScreen.GetComponent<CanvasGroup>();
        if (blackScreenCanvasGroup == null)
        {
            blackScreenCanvasGroup = blackScreen.gameObject.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        winCanvasGroup.alpha = 0f;
        blackScreenCanvasGroup.alpha = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            winCanvasGroup.alpha = alpha;
            blackScreenCanvasGroup.alpha = alpha;
            yield return null;
        }

        winCanvasGroup.alpha = 1f; // Ensure fully visible
        blackScreenCanvasGroup.alpha = 1f; // Ensure fully visible

        restartPrompt.gameObject.SetActive(true);
    }
}
