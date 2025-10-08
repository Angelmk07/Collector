using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerSprite;

    private Vector2 movement;

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.magnitude > 1f)
            movement.Normalize();

        rb.velocity = movement * moveSpeed;

        animator.SetBool("isWalk", movement.x != 0 || movement.y != 0);

        if (movement.x > 0)
            playerSprite.localScale = new Vector3(1f, 1f, 1f);
        else if (movement.x < 0)
            playerSprite.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}