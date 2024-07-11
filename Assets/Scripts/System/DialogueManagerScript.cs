using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string characterName;
        public string chapter;
        public string dialogueSequence;
        public string dialogNumber;
        public string text;
    }

    [System.Serializable]
    public class DialogueList
    {
        public List<Dialogue> dialogues = new List<Dialogue>();
    }

    public Transform player;  // Assign player transform in the inspector
    public TextMeshProUGUI dialogueText;
    public float triggerDistance = 2.0f;  // Distance within which the player can trigger the dialogue
    public float typingSpeed = 0.05f;  // Speed at which the text is displayed letter by letter
    private List<Dialogue> dialogues = new List<Dialogue>();
    private List<Dialogue> currentDialogues = new List<Dialogue>();
    private int currentDialogueIndex = -1; // Initialized to -1 to indicate no dialogue is active
    private Coroutine typingCoroutine;
    public bool isTyping = false;


    public void LoadDialogueData()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        string filePath = Path.Combine(Application.dataPath, "JSONs/dialogue.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            dialogues = JsonUtility.FromJson<DialogueList>(dataAsJson).dialogues;

            Debug.Log($"Loaded {dialogues.Count} dialogues.");
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }



    public void StartDialogue(string characterName, string chapter, string dialogSequence)
    {
        currentDialogues = dialogues.FindAll(d => d.characterName == characterName && d.chapter == chapter && d.dialogueSequence == dialogSequence);
        Debug.Log($"Found {currentDialogues.Count} dialogues for character: {characterName}, chapter: {chapter}, sequence: {dialogSequence}");

        if (currentDialogues.Count > 0)
        {
            currentDialogues.Sort((d1, d2) => int.Parse(d1.dialogNumber).CompareTo(int.Parse(d2.dialogNumber))); // Sort by dialogNumber
            currentDialogueIndex = 0;
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeDialogue(currentDialogues[currentDialogueIndex].text));
        }
        else
        {
            Debug.LogError("Dialogue not found!");
        }
    }

    public void AdvanceDialogue()
    {
        if (currentDialogueIndex != -1)
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < currentDialogues.Count)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeDialogue(currentDialogues[currentDialogueIndex].text));
            }
            else
            {
                // Dialogue sequence complete
                gameObject.SetActive(false);
                currentDialogueIndex = -1;
            }
        }
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void ShowFullText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = currentDialogues[currentDialogueIndex].text;
        isTyping = false;
    }
}