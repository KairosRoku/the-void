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
                Debug.Log("Raycast hit: " + hit.collider.tag);

                if (hit.collider.CompareTag("wall") ||
                    hit.collider.CompareTag("enemy") ||
                    hit.collider.CompareTag("furniture") ||
                    hit.collider.CompareTag("interactable"))
                {
                    CreateMark(hit.point, hit.normal, hit.collider.tag);
                }

                if (hit.collider.CompareTag("enemy"))
                {
                    Debug.Log("Hit enemy: " + hit.collider.name);
                    EnemyAI enemyAI = hit.collider.GetComponentInParent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.HitByBullet();
                    }
                    else
                    {
                        Debug.LogWarning("EnemyAI component not found on hit collider's parent.");
                    }
                }
            }
        }
    }

    Vector3 GetRandomDirection()
    {
        float azimuth = Random.Range(-spreadAngle, spreadAngle);
        float elevation = Random.Range(-spreadAngle, spreadAngle);
        float azimuthRad = azimuth * Mathf.Deg2Rad;
        float elevationRad = elevation * Mathf.Deg2Rad;
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
                return;
        }

        GameObject mark = Instantiate(markPrefab, position + normal * 0.01f, Quaternion.LookRotation(normal));
        marks.Enqueue(mark);
        if (marks.Count > maxMarks)
        {
            GameObject oldMark = marks.Dequeue();
            Destroy(oldMark);
        }

        Destroy(mark, 60f);
    }
}
