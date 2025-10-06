using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public UnityEvent onActive;
    public UnityEvent onDeactive;
    public PlayerStatus playerStatus;
    public List<DialogueSettings> dialogueSettings;
    [Min(0f)] public float textSpeed = 50f;
    public int needMoney;
    public int moneyIncreaseAmount = 100;

    public GameObject DialoguePanel;
    public Image personaImage_01;
    public Image personaImage_02;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Button closeDialogueButton;
    public Button buyGunButton;
    public Button giveMoneyButton;

    private string currentText;
    private int currentDialogue;
    private int currentCharacter;
    private float timer;
    private bool isActive;
    private bool isTyping;
    private bool playerInTrigger;

    void Awake()
    {
        DeactiveDialogue();
        closeDialogueButton.onClick.AddListener(() => DeactiveDialogue());

        /*buyGunButton.onClick.AddListener(() => логика траты денег на оружие);*/

        giveMoneyButton.onClick.AddListener(() => GiveMoneyAndIncrease());
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E) && !isActive)
            ActiveDialogue();

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

        closeDialogueButton.gameObject.SetActive(false);
        buyGunButton.gameObject.SetActive(false);
        giveMoneyButton.gameObject.SetActive(false);
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
                    closeDialogueButton.gameObject.SetActive(true);
                    buyGunButton.gameObject.SetActive(true);

                    if (playerStatus.money >= needMoney)
                        giveMoneyButton.gameObject.SetActive(true);
                    else
                        giveMoneyButton.gameObject.SetActive(false);
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
        personaImage_01.sprite = dialogueSettings[currentDialogue].persona_01;
        personaImage_02.sprite = dialogueSettings[currentDialogue].persona_02;
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

    private void GiveMoneyAndIncrease()
    {
        playerStatus.GiveMoney(needMoney);
        needMoney += moneyIncreaseAmount;
        DeactiveDialogue();
    }

    public bool IsDialogueActive()
    {
        return isActive;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInTrigger = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInTrigger = false;
    }

    public bool IsPlayerInTrigger()
    {
        return playerInTrigger;
    }
}