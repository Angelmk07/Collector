using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public bool canMelee;
    [SerializeField] private Animator animator;
 
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Vector3 attackPosition;
    [SerializeField] private float attackRadius;
    [SerializeField] private int damage = 1;

    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float damageBeginning = 0.2f;

    [SerializeField] private Color gizmoIdleColor = Color.white;
    [SerializeField] private Color gizmoAttackColor = Color.red;
    [SerializeField] private bool showGizmos = true;

    private float cooldownTimer;
    private float attackTimer;
    private float damageBeginningTimer;
    private int hitCount;
    private bool isAttacking;
    private bool damageTriggered;

    void Update()
    {
        UpdateTimers();
        HandleAttack();
        UpdateAttackState();
    }

    void UpdateTimers()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (damageBeginningTimer > 0)
        {
            damageBeginningTimer -= Time.deltaTime;

            if (damageBeginningTimer <= 0 && !damageTriggered)
                StartAttack();
        }
    }

    void HandleAttack()
    {
        if (!canMelee) return;
        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0)
            DamageBeginning();
    }

    void DamageBeginning()
    {
        animator.SetTrigger("isAttack");
        damageBeginningTimer = damageBeginning;
        cooldownTimer = attackCooldown;
        damageTriggered = false;
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        damageTriggered = true;

        PerformAttack();
    }

    void UpdateAttackState()
    {
        if (isAttacking && attackTimer <= 0)
        {
            isAttacking = false;
            damageBeginningTimer = 0;
        }
    }

    void PerformAttack()
    {
        Vector2 circleCenter = transform.position + attackPosition;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(circleCenter, attackRadius, enemyLayers);
        hitCount = hitColliders.Length;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            DealDamage(hitColliders[i].gameObject);
        }
    }

    void DealDamage(GameObject target)
    {
        target.TryGetComponent(out HpController hp);
        hp.TakeDamage(damage);
    }

    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Vector3 circleCenter = transform.position + attackPosition;

            if (isAttacking)
                Gizmos.color = gizmoAttackColor;
            else
                Gizmos.color = gizmoIdleColor;

            Gizmos.DrawWireSphere(circleCenter, attackRadius);
        }
    }
}