using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text winText;
    public Text restartPrompt;
    private bool gameIsPaused = false;

    void Start()
    {
        if (winText != null) winText.gameObject.SetActive(false);
        if (restartPrompt != null) restartPrompt.gameObject.SetActive(false);
    }

    public void CheckGameFinish()
    {
        // This method should be called by the CollisionTest script
        if (GameObject.FindGameObjectsWithTag("gem").Length == 0)
        {
            GameFinished();
        }
    }

    void GameFinished()
    {
        gameIsPaused = true;
        Time.timeScale = 0f; // Pause the game

        if (winText != null) winText.gameObject.SetActive(true);
        if (restartPrompt != null) restartPrompt.gameObject.SetActive(true);

        Debug.Log("Game Finished!");
    }

    void Update()
    {
        if (gameIsPaused && Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        Time.timeScale = 1f; // Unpause the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
