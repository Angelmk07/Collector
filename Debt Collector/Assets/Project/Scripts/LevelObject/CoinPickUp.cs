using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] private int giveMoney;

    private PlayerStatus playerStatus;

    void Awake()
    {
        playerStatus = FindAnyObjectByType<PlayerStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerStatus.GetMoney(giveMoney);
            Destroy(gameObject);
        }
    }
}