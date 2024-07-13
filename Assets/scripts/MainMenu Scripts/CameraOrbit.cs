using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // The target object to orbit around
    public float distance = 10.0f; // Distance from the target
    public float orbitSpeed = 10.0f; // Speed of orbit
    public float height = 5.0f; // Height above the target
    public Vector3 rotationOffset = new Vector3(20.0f, 0.0f, 0.0f); // Rotation offset (X, Y, Z)

    private float currentAngle = 0.0f; // Current angle of orbit

    void Update()
    {
        if (target != null)
        {
            // Calculate the current angle
            currentAngle += orbitSpeed * Time.deltaTime;
            currentAngle = currentAngle % 360.0f;

            // Convert angle to radians
            float angleInRadians = currentAngle * Mathf.Deg2Rad;

            // Calculate the new position
            Vector3 offset = new Vector3(Mathf.Sin(angleInRadians) * distance, height, Mathf.Cos(angleInRadians) * distance);
            transform.position = target.position + offset;

            // Look at the target
            transform.LookAt(target);

            // Apply the rotation offset
            transform.rotation *= Quaternion.Euler(rotationOffset);
        }
    }
}
