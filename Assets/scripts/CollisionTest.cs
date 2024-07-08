using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
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

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionTest: Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            CheckGameFinish();
        }
    }

    void CheckGameFinish()
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
