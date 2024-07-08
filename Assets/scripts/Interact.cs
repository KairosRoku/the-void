using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public float interactionRange = 3f; // Range within which the player can interact
    public string interactableTag = "Interactable"; // Tag for interactable objects
    public string gemTag = "gem"; // Tag for gem objects
    public KeyCode interactionKey = KeyCode.E; // Key for interaction

    private Camera playerCamera;
    private List<GameObject> inventory = new List<GameObject>();

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            InteractWithObject();
        }
    }

    void InteractWithObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.OnInteract();
                    if (interactable.isPickup)
                    {
                        AddToInventory(interactable.gameObject);
                    }
                }
            }
            else if (hit.collider.CompareTag(gemTag))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.OnInteract();
                    RemoveGem(interactable.gameObject);
                }
            }
        }
    }

    void AddToInventory(GameObject item)
    {
        inventory.Add(item);
        item.SetActive(false); // Hide the item
        Debug.Log("Item added to inventory: " + item.name);
    }

    void RemoveGem(GameObject gem)
    {
        Destroy(gem);
        Debug.Log("Gem removed from scene: " + gem.name);
    }
}
