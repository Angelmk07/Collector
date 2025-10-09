using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    public int health;
    public int money;
    public int completedLevels;
    public int needMoney;
    [SerializeField] private GameObject pomer;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text completedLevelsText;
    [SerializeField] private UnityEvent Ondead;
    [SerializeField] private LeaderboardSaver leaderboard;

    void Awake()
    {
        LoadVariables();
        UpdateText();
    }

    public void GetMoney(int giveMoney)
    {
        money += giveMoney;
        UpdateText();
    }

    public void GiveMoney(int giveMoney)
    {
        money -= giveMoney;
        UpdateText();
    }

    public void UpdateNeedMoney(int giveNeedMoney)
    {
        needMoney += giveNeedMoney;
    }

    [System.Obsolete]
    public void Die()
    {
        pomer.SetActive(true);
        StartCoroutine(Restart());
    }

    public void SaveVariables()
    {
        PlayerPrefs.SetInt("MoneyVariables", money);
        PlayerPrefs.SetInt("CompletedLevelsVariables", completedLevels);
        PlayerPrefs.SetInt("NeedMoney", needMoney);

        PlayerPrefs.Save();
    }

    public void LoadVariables()
    {
        money = PlayerPrefs.GetInt("MoneyVariables", money);
        completedLevels = PlayerPrefs.GetInt("CompletedLevelsVariables", completedLevels);
        needMoney = PlayerPrefs.GetInt("NeedMoney", needMoney);
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("MoneyVariables");
        PlayerPrefs.DeleteKey("CompletedLevelsVariables");
        PlayerPrefs.DeleteKey("NeedMoney");
        PlayerPrefs.Save();

        money = 0;
        completedLevels = 0;
        needMoney = 0;
    }

    public void UpdateText()
    {
        moneyText.text = money.ToString();
        completedLevelsText.text = completedLevels.ToString();
    }

    IEnumerator Restart()
    {
        leaderboard.SendScore(completedLevels);
        Ondead?.Invoke();
        ResetSave();
        SceneManager.LoadScene(0);

        yield return new WaitForSeconds(5f);
    }
}