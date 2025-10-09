using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrownDoor2D : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private Vector2 throwDirection = Vector2.right;
    [SerializeField] private float throwForce = 8f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private bool useGravity = false;
    [SerializeField] private HpController hpController;
    [SerializeField] private bool Used;
    [SerializeField] private Sprite spriteDorKick;
    private Rigidbody2D rb;
    private float elapsedTime;

    private void Start()
    {
        hpController.OnDead.AddListener(OnKick);
    }
    private void OnKick()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = useGravity ? 1f : 0f;

        Vector2 direction = throwDirection.normalized;
        rb.velocity = direction * throwForce;

        elapsedTime = 0f;
        Used = true;
        if(gameObject.TryGetComponent(out SpriteRenderer sr)&&spriteDorKick!=null)
            {
            sr.sprite = spriteDorKick;
        }
    }

    private void Update()
    {
        if (!Used) return;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetTrajectory(Vector2 direction, float force)
    {
        throwDirection = direction;
        throwForce = force;

        if (rb != null)
        {
            Vector2 dir = throwDirection.normalized;
            rb.velocity = dir * throwForce;
        }
    }
}