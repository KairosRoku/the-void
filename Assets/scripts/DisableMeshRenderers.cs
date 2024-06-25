using UnityEngine;

public class DisableMeshRenderers : MonoBehaviour
{
    // Array of tags to disable MeshRenderer for
    private string[] tagsToDisable = { "wall", "interactable", "furniture", "enemy" };

    void Start()
    {
        foreach (string tag in tagsToDisable)
        {
            // Find all game objects with the specified tag
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

            // Loop through each object and disable its MeshRenderer
            foreach (GameObject obj in objectsWithTag)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
        }
    }
}
