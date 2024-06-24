using UnityEngine;
using System.Collections.Generic;

public class RaycastShoot : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject wallMarkPrefab;
    public GameObject enemyMarkPrefab;
    public GameObject furnitureMarkPrefab;
    public GameObject interactableMarkPrefab;
    public int bulletCount = 10;
    public float range = 100f;
    public float spreadAngle = 10f; // Maximum angle in degrees for bullet spread
    public float fireRate = 0.1f; // Time between shots in seconds
    public int maxMarks = 20; // Maximum number of marks

    private float nextTimeToShoot = 0f;
    private Queue<GameObject> marks = new Queue<GameObject>();

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 shootDirection = GetRandomDirection();
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, shootDirection, out hit, range))
            {
                if (hit.collider.CompareTag("wall") ||
                    hit.collider.CompareTag("enemy") ||
                    hit.collider.CompareTag("furniture") ||
                    hit.collider.CompareTag("interactable"))
                {
                    CreateMark(hit.point, hit.normal, hit.collider.tag);
                }
            }
        }
    }

    Vector3 GetRandomDirection()
    {
        // Randomize both azimuth and elevation angles
        float azimuth = Random.Range(-spreadAngle, spreadAngle);
        float elevation = Random.Range(-spreadAngle, spreadAngle);

        // Convert angles to radians
        float azimuthRad = azimuth * Mathf.Deg2Rad;
        float elevationRad = elevation * Mathf.Deg2Rad;

        // Calculate spread direction
        Vector3 direction = new Vector3(
            Mathf.Sin(azimuthRad) * Mathf.Cos(elevationRad),
            Mathf.Sin(elevationRad),
            Mathf.Cos(azimuthRad) * Mathf.Cos(elevationRad)
        );

        return playerCamera.transform.TransformDirection(direction.normalized);
    }

    void CreateMark(Vector3 position, Vector3 normal, string tag)
    {
        GameObject markPrefab;

        // Select the correct prefab based on the tag
        switch (tag)
        {
            case "wall":
                markPrefab = wallMarkPrefab;
                break;
            case "enemy":
                markPrefab = enemyMarkPrefab;
                break;
            case "furniture":
                markPrefab = furnitureMarkPrefab;
                break;
            case "interactable":
                markPrefab = interactableMarkPrefab;
                break;
            default:
                return; // Exit if the tag doesn't match any specified tags
        }

        // Instantiate the correct prefab
        GameObject mark = Instantiate(markPrefab, position + normal * 0.01f, Quaternion.LookRotation(normal));

        // Add the mark to the queue and remove the oldest if necessary
        marks.Enqueue(mark);
        if (marks.Count > maxMarks)
        {
            GameObject oldMark = marks.Dequeue();
            Destroy(oldMark);
        }

        Destroy(mark, 60f); // Destroy the mark after 60 seconds
    }
}
