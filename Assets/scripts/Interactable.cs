using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isPickup = false; // Set to true if this object is a pickup item
    public bool isDoor = false; // Set to true if this object is a door

    private Animator doorAnimator;
    private bool isDoorClosed = true; // Track the door's closed/open state

    void Start()
    {
        if (isDoor)
        {
            doorAnimator = GetComponent<Animator>();
            doorAnimator.SetBool("isClosed", true); // Ensure the door starts closed
        }
    }

    public void OnInteract()
    {
        if (isDoor)
        {
            ToggleDoor();
        }
        else if (isPickup)
        {
            Pickup();
        }
    }

    void ToggleDoor()
    {
        if (doorAnimator != null)
        {
            isDoorClosed = !isDoorClosed;
            doorAnimator.SetBool("isClosed", isDoorClosed);
            Debug.Log("Door " + (isDoorClosed ? "closed" : "opened") + ": " + gameObject.name);
        }
    }

    void Pickup()
    {
        // Add pickup logic if needed
        Debug.Log("Picked up: " + gameObject.name);
    }
}
