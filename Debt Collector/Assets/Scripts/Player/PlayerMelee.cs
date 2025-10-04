using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Vector3 attackPosition;
    [SerializeField] private Vector3 attackSize;
    [SerializeField] private int damage = 1;

    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackDuration = 0.2f;

    [SerializeField] private Color gizmoIdleColor = Color.white;
    [SerializeField] private Color gizmoAttackColor = Color.red;
    [SerializeField] private bool showGizmos = true;

    private float cooldownTimer;
    private float attackTimer;
    private int hitCount;
    private bool isAttacking;

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
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0)
            StartAttack();
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        cooldownTimer = attackCooldown;

        PerformAttack();
    }

    void UpdateAttackState()
    {
        if (isAttacking && attackTimer <= 0)
            isAttacking = false;
    }

    void PerformAttack()
    {
        Vector3 boxCenter = transform.position + attackPosition;
        Vector3 boxSize = attackSize;

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize * 0.5f, transform.rotation, enemyLayers);
        hitCount = hitColliders.Length;

        for (int i = 0; i < hitColliders.Length; i++)
            DealDamage(hitColliders[i].gameObject);
    }

    void DealDamage(GameObject target)
    {
        // Здесь добавить логику нанесения урона

        Debug.Log($"Hit: {target.name} with {damage} damage");
    }

    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Vector3 boxCenter = transform.position + attackPosition;
            Vector3 boxSize = attackSize;

            if (isAttacking)
                Gizmos.color = gizmoAttackColor;
            else
                Gizmos.color = gizmoIdleColor;

            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}