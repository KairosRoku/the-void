using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Array to hold all possible spawn points
    public GameObject playerPrefab; // Reference to the player prefab

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

        // Instantiate the player at the selected spawn point
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
