using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Array to hold all possible spawn points
    public GameObject GemPrefab; // Reference to the Gem prefab

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

        // Instantiate the Gem at the selected spawn point
        Instantiate(GemPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
