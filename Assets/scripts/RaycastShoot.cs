using UnityEngine;
using System.Collections.Generic;

public class RaycastShoot : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject wallMarkPrefab;
    public GameObject enemyMarkPrefab;
    public GameObject furnitureMarkPrefab;
    public GameObject interactableMarkPrefab;
    public GameObject gemMarkPrefab;
    public GameObject returnMarkPrefab; // Prefab for the "return" tag
    public int bulletCount = 10;
    public float range = 100f;
    public float spreadAngle = 10f; // Maximum angle in degrees for bullet spread
    public float fireRate = 0.1f; // Time between shots in seconds
    public int maxMarks = 20; // Maximum number of marks
    public int maxIterations = 10; // Maximum number of iterations for the loop

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
            Vector3 currentPosition = playerCamera.transform.position;
            int iterations = 0;

            while (iterations < maxIterations)
            {
                RaycastHit hit;
                if (Physics.Raycast(currentPosition, shootDirection, out hit, range))
                {
                    if (hit.collider.CompareTag("ignore"))
                    {
                        // Continue raycasting from the hit point onwards
                        currentPosition = hit.point + shootDirection * 0.01f;
                        iterations++;
                        continue;
                    }

                    if (hit.collider.CompareTag("wall") ||
                        hit.collider.CompareTag("enemy") ||
                        hit.collider.CompareTag("furniture") ||
                        hit.collider.CompareTag("interactable") ||
                        hit.collider.CompareTag("gem") ||
                        hit.collider.CompareTag("return"))
                    {
                        CreateMark(hit.point, hit.normal, hit.collider.tag);

                        if (hit.collider.CompareTag("enemy"))
                        {
                            EnemyAI enemyAI = hit.collider.GetComponentInParent<EnemyAI>();
                            if (enemyAI != null)
                            {
                                enemyAI.HitByBullet();
                            }
                        }
                        break; // Exit loop after marking the first valid hit
                    }
                }
                break; // Exit loop if no valid hit
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
            case "gem":
                markPrefab = gemMarkPrefab;
                break;
            case "return":
                markPrefab = returnMarkPrefab;
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
