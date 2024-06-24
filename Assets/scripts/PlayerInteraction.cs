using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Camera playerCamera;
    public Text interactionPrompt;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false); // Ensure the prompt is inactive at the start
        }
    }

    void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform.CompareTag("interactable"))
            {
                interactionPrompt.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenInteractable(hit.transform);
                }
            }
            else
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }
        else
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void OpenInteractable(Transform interactableTransform)
    {
        Interactable interactable = interactableTransform.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.Open();
        }
    }
}

public class Interactable : MonoBehaviour
{
    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, 0, 1.5f); // Adjust as necessary for how you want the interactable object to open
    }

    public void Open()
    {
        if (!isOpen)
        {
            transform.position = openPosition;
            isOpen = true;
        }
    }
}
