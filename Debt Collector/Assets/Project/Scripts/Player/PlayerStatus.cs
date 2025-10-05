using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private int money;

    public int completedLevels;

    void Awake()
    {
        LoadVariables();
    }

    public void GiveMoney(int giveMoney)
    {
        money += giveMoney;
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
}