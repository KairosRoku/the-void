using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameFinished = false;
    private bool canTrigger = false;

    void Start()
    {
        StartCoroutine(AllowTriggerAfterDelay(3f));
    }

    IEnumerator AllowTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTrigger = true;
    }

    public void CheckGameFinish()
    {
        if (!canTrigger)
            return;

        if (GameObject.FindGameObjectsWithTag("gem").Length == 0)
        {
            gameFinished = true;
            Debug.Log("Game Finished!");
        }
    }
}
