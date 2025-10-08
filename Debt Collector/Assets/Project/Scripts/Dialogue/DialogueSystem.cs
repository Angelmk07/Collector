using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public UnityEvent onActive;
    public UnityEvent onDeactive;
    public UnityEvent onBuyWeapon;
    public PlayerStatus playerStatus;
    public List<DialogueSettings> dialogueSettings;
    public List<DialogueSettings> dialogueSettingsNext;
    public List<DialogueSettings> closeDialogueSettings;
    public List<DialogueSettings> buyWeaponSettings;
    public List<DialogueSettings> giveMoneySettings;
    [Min(0f)] public float textSpeed = 50f;
    public int needMoneyForWeapon;
    public int moneyIncreaseAmount = 100;
    public bool useInput = true;
    public bool destroyInNextLevel;

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
    private DialogueChoice currentChoice;

    private enum DialogueChoice
    {
        None,
        Close,
        BuyWeapon,
        GiveMoney
    }

    void Awake()
    {
        isActive = false;
        currentChoice = DialogueChoice.None;

        closeDialogueButton.gameObject.SetActive(false);
        buyGunButton.gameObject.SetActive(false);
        giveMoneyButton.gameObject.SetActive(false);
        DialoguePanel.SetActive(false);

        ResetTyping();

        closeDialogueButton.onClick.AddListener(() => OnCloseDialogueChoice());
        buyGunButton.onClick.AddListener(() => OnBuyWeaponChoice());
        giveMoneyButton.onClick.AddListener(() => OnGiveMoneyChoice());

        if (destroyInNextLevel && playerStatus.completedLevels > 0)
            Destroy(gameObject);
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E) && !isActive && useInput)
            ActiveDialogue();

        UpdateText();
        UpdateTyping();
    }

    public void ActiveDialogue()
    {
        onActive.Invoke();

        isActive = true;
        currentDialogue = 0;
        currentChoice = DialogueChoice.None;

        DialoguePanel.SetActive(true);

        StartTypingDialogue();
    }

    public void DeactiveDialogue()
    {
        onDeactive.Invoke();

        isActive = false;
        currentChoice = DialogueChoice.None;

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

                // ѕровер€ем, есть ли дополнительные диалоги дл€ текущего выбора
                if (currentChoice != DialogueChoice.None)
                {
                    List<DialogueSettings> choiceSettings = GetChoiceDialogueSettings(currentChoice);

                    if (choiceSettings.Count > 0 && currentDialogue < choiceSettings.Count)
                        StartChoiceTypingDialogue(choiceSettings);
                    else
                        DeactiveDialogue();
                }
                else
                {
                    // ќбычный диалог (до выбора)
                    if (playerStatus.completedLevels <= 0)
                    {
                        if (dialogueSettings.Count > 0 && currentDialogue < dialogueSettings.Count)
                            StartTypingDialogue();
                        else
                            ShowChoiceButtons();
                    }
                    else
                    {
                        if (dialogueSettingsNext.Count > 0 && currentDialogue < dialogueSettingsNext.Count)
                            StartTypingDialogueNext();
                        else
                            ShowChoiceButtons();
                    }
                }
            }
        }
    }

    private void ShowChoiceButtons()
    {
        nameText.text = "";
        dialogueText.text = "";
        closeDialogueButton.gameObject.SetActive(true);
        buyGunButton.gameObject.SetActive(playerStatus.money >= needMoneyForWeapon);
        giveMoneyButton.gameObject.SetActive(playerStatus.money >= needMoneyForWeapon);
    }

    private void OnCloseDialogueChoice()
    {
        currentChoice = DialogueChoice.Close;
        StartChoiceDialogue(closeDialogueSettings);
    }

    private void OnBuyWeaponChoice()
    {
        currentChoice = DialogueChoice.BuyWeapon;
        onBuyWeapon.Invoke();
        StartChoiceDialogue(buyWeaponSettings);
    }

    private void OnGiveMoneyChoice()
    {
        currentChoice = DialogueChoice.GiveMoney;
        GiveMoneyAndIncrease();
        StartChoiceDialogue(giveMoneySettings);
    }

    private void StartChoiceDialogue(List<DialogueSettings> settings)
    {
        if (settings == null || settings.Count == 0)
        {
            DeactiveDialogue();
            return;
        }

        closeDialogueButton.gameObject.SetActive(false);
        buyGunButton.gameObject.SetActive(false);
        giveMoneyButton.gameObject.SetActive(false);

        currentDialogue = 0;
        StartChoiceTypingDialogue(settings);
    }

    private void StartChoiceTypingDialogue(List<DialogueSettings> settings)
    {
        personaImage_01.sprite = settings[currentDialogue].persona_01;
        personaImage_02.sprite = settings[currentDialogue].persona_02;
        nameText.text = $"[ {settings[currentDialogue].name} ]";

        ResetTyping();

        currentText = settings[currentDialogue].dialogue;

        dialogueText.text = "> ";

        isTyping = true;
        timer = 0f;
        currentCharacter = 0;
    }

    private List<DialogueSettings> GetChoiceDialogueSettings(DialogueChoice choice)
    {
        return choice switch
        {
            DialogueChoice.Close => closeDialogueSettings,
            DialogueChoice.BuyWeapon => buyWeaponSettings,
            DialogueChoice.GiveMoney => giveMoneySettings,
            _ => new List<DialogueSettings>()
        };
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
        personaImage_01.color = dialogueSettings[currentDialogue].colorPersona_01;
        personaImage_02.sprite = dialogueSettings[currentDialogue].persona_02;
        personaImage_02.color = dialogueSettings[currentDialogue].colorPersona_02;
        nameText.text = $"[ {dialogueSettings[currentDialogue].name} ]";

        ResetTyping();

        currentText = dialogueSettings[currentDialogue].dialogue;

        dialogueText.text = "> ";

        isTyping = true;
        timer = 0f;
        currentCharacter = 0;
    }

    private void StartTypingDialogueNext()
    {
        personaImage_01.sprite = dialogueSettingsNext[currentDialogue].persona_01;
        personaImage_01.color = dialogueSettingsNext[currentDialogue].colorPersona_01;
        personaImage_02.sprite = dialogueSettingsNext[currentDialogue].persona_02;
        personaImage_02.color = dialogueSettingsNext[currentDialogue].colorPersona_02;
        nameText.text = $"[ {dialogueSettingsNext[currentDialogue].name} ]";

        ResetTyping();

        currentText = dialogueSettingsNext[currentDialogue].dialogue;

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
        playerStatus.GiveMoney(needMoneyForWeapon);
        needMoneyForWeapon += moneyIncreaseAmount;
    }

    public bool IsDialogueActive()
    {
        return isActive;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInTrigger = true;

        if (!useInput)
            ActiveDialogue();
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