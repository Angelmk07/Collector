using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    public int health;
    public int money;
    public int completedLevels;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text completedLevelsText;

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

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    public void Die()
    {
        // Здесь добавить логику смерти
    }

    public void SaveVariables()
    {
        PlayerPrefs.SetInt("MoneyVariables", money);
        PlayerPrefs.SetInt("CompletedLevelsVariables", completedLevels);

        PlayerPrefs.Save();
    }

    public void LoadVariables()
    {
        money = PlayerPrefs.GetInt("MoneyVariables", money);
        completedLevels = PlayerPrefs.GetInt("CompletedLevelsVariables", completedLevels);
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("MoneyVariables");
        PlayerPrefs.DeleteKey("CompletedLevelsVariables");
        PlayerPrefs.Save();

        money = 0;
        completedLevels = 0;
    }

    public void UpdateText()
    {
        moneyText.text = money.ToString();
        completedLevelsText.text = completedLevels.ToString();
    }
}