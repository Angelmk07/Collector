using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public UnityEvent onActive;
    public UnityEvent onDeactive;
    public List<DialogueSettings> dialogueSettings;
    [Min(0f)] public float textSpeed = 50f;

    public GameObject DialoguePanel;
    public Image personaImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    private string currentText;
    private int currentDialogue;
    private int currentCharacter;
    private float timer;
    private bool isActive;
    private bool isTyping;

    void Awake()
    {
        DeactiveDialogue();
    }

    void Update()
    {
        UpdateText();
        UpdateTyping();
    }

    public void ActiveDialogue()
    {
        onActive.Invoke();

        isActive = true;
        currentDialogue = 0;

        DialoguePanel.SetActive(true);

        StartTypingDialogue();
    }

    public void DeactiveDialogue()
    {
        onDeactive.Invoke();

        isActive = false;

        DialoguePanel.SetActive(false);

        ResetTyping();
    }

    public void UpdateText()
    {
        if (isActive && Input.anyKeyDown)
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                currentDialogue++;

                if (dialogueSettings.Count > 0 && currentDialogue < dialogueSettings.Count)
                {
                    StartTypingDialogue();
                }
                else
                {
                    nameText.text = "";
                    dialogueText.text = "";
                    DeactiveDialogue();
                }
            }
        }
    }

    private void UpdateTyping()
    {
        if (isTyping)
        {
            timer += Time.deltaTime;

            int targetCharacter = Mathf.FloorToInt(timer * textSpeed);

            if (targetCharacter > currentCharacter && targetCharacter <= currentText.Length)
            {
                while (currentCharacter < targetCharacter && currentCharacter < currentText.Length)
                {
                    dialogueText.text += currentText[currentCharacter];
                    currentCharacter++;
                }
            }

            if (currentCharacter < currentText.Length)
            {
                string baseText = "> " + currentText.Substring(0, currentCharacter);

                char randomChar = (char)Random.Range(33, 127);

                dialogueText.text = baseText + randomChar;
            }
            else if (currentCharacter >= currentText.Length)
            {
                dialogueText.text = "> " + currentText;
                isTyping = false;
            }
        }
    }

    private void StartTypingDialogue()
    {
        personaImage = dialogueSettings[currentDialogue].persona;
        nameText.text = $"[ {dialogueSettings[currentDialogue].name} ]";

        ResetTyping();

        currentText = dialogueSettings[currentDialogue].dialogue;

        dialogueText.text = "> ";

        isTyping = true;
        timer = 0f;
        currentCharacter = 0;
    }

    private void CompleteTyping()
    {
        dialogueText.text = $"> {currentText}";

        isTyping = false;
        currentCharacter = currentText.Length;
    }

    private void ResetTyping()
    {
        isTyping = false;
        currentText = "";
        timer = 0f;
        currentCharacter = 0;
    }
}