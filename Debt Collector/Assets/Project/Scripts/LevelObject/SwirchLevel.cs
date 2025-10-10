using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwirchLevel : MonoBehaviour
{
    [SerializeField] private UnityEvent onActive;
    [SerializeField] private UnityEvent onDeactive;
    [SerializeField] private int nextLevelIndex;
    [SerializeField] private GameObject SwitchLevelpanel;
    [SerializeField] private Button accept;
    [SerializeField] private Button cancel;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private DialogueSystem dialogueSystem;

    private void Awake()
    {
        accept.onClick.AddListener(() => Accept());
        cancel.onClick.AddListener(() => Cancel());
        SwitchLevelpanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && dialogueSystem.playerGiveMoney)
        {
            SwitchLevelpanel.SetActive(true);
            onActive.Invoke();
        }
    }

    private void Accept()
    {
        playerStatus.completedLevels++;
        playerStatus.SaveVariables();
        SceneManager.LoadScene(nextLevelIndex);
    }

    private void Cancel()
    {
        SwitchLevelpanel.SetActive(false);
        onDeactive.Invoke();
    }
}