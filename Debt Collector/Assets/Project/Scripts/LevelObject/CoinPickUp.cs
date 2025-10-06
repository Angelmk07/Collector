using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] private int giveMoney;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;

    private PlayerStatus playerStatus;
    private Vector2 moveDirection;

    void Awake()
    {
        playerStatus = FindAnyObjectByType<PlayerStatus>();
    }

    void Start()
    {
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.velocity = moveDirection * moveSpeed;
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