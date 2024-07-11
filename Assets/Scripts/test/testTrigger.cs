using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTrigger : MonoBehaviour
{
    public GameObject dialogueManagerPrefab;
    GameObject dialogueManager;
    public Transform target; // Assign this in the inspector with the object to interact with
    public float interactionDistance = 1.5f;
    private bool chatBoxOpen = false;

    public string characterName;
    public string chapter;
    public string sequence;

    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite upSprite;
    public Sprite downSprite;

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        dialogueManager = Instantiate(dialogueManagerPrefab);
        dialogueManager.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateSprite();
        UpdateSortingOrder();
        // Check if the player is within the interaction distance
        if (Vector3.Distance(transform.position, target.position) <= interactionDistance)
        {
            // Check if the 'E' key is pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!chatBoxOpen)
                {
                    dialogueManager.SetActive(true);
                    dialogueManager.GetComponent<DialogueManager>().LoadDialogueData();
                    dialogueManager.GetComponent<DialogueManager>().StartDialogue(characterName, chapter, sequence);
                    chatBoxOpen = true;
                }
                else
                {
                    if (chatBoxOpen)
                    {
                        if (dialogueManager.GetComponent<DialogueManager>().isTyping == true)
                        {
                            dialogueManager.GetComponent<DialogueManager>().ShowFullText();
                        }
                        else
                        {
                            dialogueManager.GetComponent<DialogueManager>().AdvanceDialogue();
                        }
                    }
                }
                if (!dialogueManager.activeSelf) chatBoxOpen = false;
            }
        }
        else dialogueManager.SetActive(false);
    }

    void UpdateSprite()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle >= -45 && angle < 45)
        {
            spriteRenderer.sprite = rightSprite;  // Right
        }
        else if (angle >= 45 && angle < 135)
        {
            spriteRenderer.sprite = upSprite;     // Up
        }
        else if (angle >= -135 && angle < -45)
        {
            spriteRenderer.sprite = downSprite;   // Down
        }
        else
        {
            spriteRenderer.sprite = leftSprite;   // Left
        }
    }

    void UpdateSortingOrder()
    {
        if (target.position.y > transform.position.y)
        {
            spriteRenderer.sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1; // In front of target
        }
        else
        {
            spriteRenderer.sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder - 1; // Behind target
        }
    }
}