using UnityEngine;
using System.Collections;

public class GemSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Array to hold all possible spawn points
    public GameObject gemPrefab; // Reference to the Gem prefab
    public GameObject player; // Reference to the player GameObject

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }

        // Randomly select a spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Delay gem instantiation by 1 second
        StartCoroutine(SpawnGemWithDelay(spawnPoint));
    }

    IEnumerator SpawnGemWithDelay(Transform spawnPoint)
    {
        yield return new WaitForSeconds(1.0f);

        // Instantiate the Gem at the selected spawn point
        GameObject spawnedGem = Instantiate(gemPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Gem spawned at position: " + spawnPoint.position);

        // Ensure the player has a ProximityTicking component and set the gem reference
        ProximityTicking proximityTicking = player.GetComponent<ProximityTicking>();
        if (proximityTicking != null)
        {
            proximityTicking.gem = spawnedGem; // Directly assign the gem
            Debug.Log("Gem assigned to ProximityTicking script.");
        }
        else
        {
            Debug.LogError("Player does not have a ProximityTicking component.");
        }
    }
}
