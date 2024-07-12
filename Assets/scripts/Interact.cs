using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interact : MonoBehaviour
{
    public float interactionRange = 3f; // Range within which the player can interact
    public string interactableTag = "Interactable"; // Tag for interactable objects
    public string gemTag = "gem"; // Tag for gem objects
    public KeyCode interactionKey = KeyCode.E; // Key for interaction

    private List<GameObject> inventory = new List<GameObject>();
    private Transform playerTransform;

    public TextMeshProUGUI interactPrompt; // Reference to the TextMeshProUGUI for the interaction prompt
    public TextMeshProUGUI returnPrompt; // Reference to the TextMeshProUGUI for the return prompt
    public TextMeshProUGUI startPrompt; // Reference to the TextMeshProUGUI for the start prompt
    private bool isNearInteractable = false; // Track if the player is near an interactable object

    void Start()
    {
        playerTransform = GetComponent<Transform>();
        if (interactPrompt != null)
        {
            SetPromptAlpha(interactPrompt, 0); // Ensure the prompt is invisible at the start
        }
        if (returnPrompt != null)
        {
            SetPromptAlpha(returnPrompt, 0); // Ensure the return prompt is invisible at the start
        }
        if (startPrompt != null)
        {
            SetPromptAlpha(startPrompt, 0); // Ensure the start prompt is invisible at the start
            StartCoroutine(DisplayStartPrompt());
        }
    }

    void Update()
    {
        CheckForNearbyObjects();

        if (isNearInteractable && Input.GetKeyDown(interactionKey))
        {
            InteractWithNearbyObjects();
        }
    }

    void CheckForNearbyObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, interactionRange);
        isNearInteractable = false;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(interactableTag) || hitCollider.CompareTag(gemTag))
            {
                isNearInteractable = true;
                break;
            }
        }

        // Fade in/out the interact prompt
        if (isNearInteractable)
        {
            FadeInPrompt(interactPrompt);
        }
        else
        {
            FadeOutPrompt(interactPrompt);
        }
    }

    void InteractWithNearbyObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, interactionRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(interactableTag))
            {
                Interactable interactable = hitCollider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                    if (interactable.isPickup)
                    {
                        AddToInventory(interactable.gameObject);
                    }
                }
            }
            else if (hitCollider.CompareTag(gemTag))
            {
                Interactable interactable = hitCollider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                    StartCoroutine(RemoveGemAfterSound(interactable.gameObject));
                    StartCoroutine(DisplayReturnPrompt());
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

    IEnumerator RemoveGemAfterSound(GameObject gem)
    {
        Collider gemCollider = gem.GetComponent<Collider>();
        if (gemCollider != null)
        {
            gemCollider.enabled = false; // Disable the collider to prevent further interaction
        }

        AudioSource audioSource = gem.GetComponent<AudioSource>();
        Interactable interactable = gem.GetComponent<Interactable>();
        if (audioSource != null && interactable != null && interactable.interactionSound != null)
        {
            audioSource.PlayOneShot(interactable.interactionSound);
            yield return new WaitForSeconds(interactable.interactionSound.length);
        }
        Destroy(gem);
        Debug.Log("Gem removed from scene: " + gem.name);
    }

    IEnumerator DisplayReturnPrompt()
    {
        if (returnPrompt != null)
        {
            // Fade in
            float duration = 1f;
            float targetAlpha = 1f;
            for (float t = 0.01f; t < duration; t += Time.deltaTime)
            {
                SetPromptAlpha(returnPrompt, Mathf.Lerp(0, targetAlpha, Mathf.Min(1, t / duration)));
                yield return null;
            }
            SetPromptAlpha(returnPrompt, targetAlpha);

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Fade out
            for (float t = 0.01f; t < duration; t += Time.deltaTime)
            {
                SetPromptAlpha(returnPrompt, Mathf.Lerp(targetAlpha, 0, Mathf.Min(1, t / duration)));
                yield return null;
            }
            SetPromptAlpha(returnPrompt, 0);
        }
    }

    IEnumerator DisplayStartPrompt()
    {
        if (startPrompt != null)
        {
            // Fade in
            float duration = 1f;
            float targetAlpha = 1f;
            for (float t = 0.01f; t < duration; t += Time.deltaTime)
            {
                SetPromptAlpha(startPrompt, Mathf.Lerp(0, targetAlpha, Mathf.Min(1, t / duration)));
                yield return null;
            }
            SetPromptAlpha(startPrompt, targetAlpha);

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Fade out
            for (float t = 0.01f; t < duration; t += Time.deltaTime)
            {
                SetPromptAlpha(startPrompt, Mathf.Lerp(targetAlpha, 0, Mathf.Min(1, t / duration)));
                yield return null;
            }
            SetPromptAlpha(startPrompt, 0);
        }
    }

    void FadeInPrompt(TextMeshProUGUI prompt)
    {
        if (prompt != null)
        {
            SetPromptAlpha(prompt, Mathf.Lerp(prompt.color.a, 1, Time.deltaTime * 2)); // Adjust the fade speed as needed
        }
    }

    void FadeOutPrompt(TextMeshProUGUI prompt)
    {
        if (prompt != null)
        {
            SetPromptAlpha(prompt, Mathf.Lerp(prompt.color.a, 0, Time.deltaTime * 2)); // Adjust the fade speed as needed
        }
    }

    void SetPromptAlpha(TextMeshProUGUI prompt, float alpha)
    {
        if (prompt != null)
        {
            Color color = prompt.color;
            color.a = alpha;
            prompt.color = color;
        }
    }
}
