using UnityEngine;
using UnityEngine.UI;

public class SimpleRegister : MonoBehaviour
{
    [SerializeField] private InputField nameInput;
    [SerializeField] private GameObject registerPanel;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            registerPanel.SetActive(false);
        }
    }

    public void OnRegisterClick()
    {
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName)) return;

        PlayerPrefs.SetString("PlayerName", playerName);
        registerPanel.SetActive(false);
    }

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName", "Unknown");
    }
}
