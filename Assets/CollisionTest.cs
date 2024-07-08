using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionTest: Collision detected with: " + collision.gameObject.name);
    }
}
