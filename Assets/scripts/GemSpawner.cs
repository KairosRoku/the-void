using UnityEngine;
using System.Collections;

public class GemSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Array to hold all possible spawn points
    public GameObject gemPrefab; // Reference to the Gem prefab

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }

        // Randomly select a spawn point for the gem
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Delay gem instantiation by 1 second
        StartCoroutine(SpawnGemWithDelay(spawnPoint));
    }

    IEnumerator SpawnGemWithDelay(Transform gemSpawnPoint)
    {
        yield return new WaitForSeconds(1.0f);

        // Instantiate the Gem at the selected spawn point
        GameObject spawnedGem = Instantiate(gemPrefab, gemSpawnPoint.position, gemSpawnPoint.rotation);
        Debug.Log("Gem spawned at position: " + gemSpawnPoint.position);

        // Ensure the player has a ProximityTicking component and set the gem reference
        ProximityTicking proximityTicking = FindObjectOfType<ProximityTicking>();
        if (proximityTicking != null)
        {
            proximityTicking.SetGem(spawnedGem); // Use the setter method to assign the gem
            Debug.Log("Gem assigned to ProximityTicking script.");
        }
        else
        {
            Debug.LogError("No ProximityTicking component found in the scene.");
        }
    }
}
