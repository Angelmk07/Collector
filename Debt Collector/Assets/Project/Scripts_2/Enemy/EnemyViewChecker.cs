using UnityEngine;
using System.Collections;

public class EnemyViewChecker : MonoBehaviour
{
    public bool PlayerInSight { get; private set; }

    [Header("Type")]
    [SerializeField] private bool Melee;
    [SerializeField] private bool Range;

    [Header("Move")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float chaseStopDistance = 1.5f;

    [Header("View Settings")]
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private Transform viewOrigin;
    [SerializeField] private LayerMask playerMask;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool canAttack = true;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color viewColor = Color.yellow;
    [SerializeField] private Color detectedColor = Color.red;

    [Header("Melee")]
    [SerializeField] private float distance = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private int countRays = 3;
    [SerializeField] private RaycastHit2D[] hits;

    [Header("Fire settings")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private LayerMask hitMask;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool isDead = false;

    private Transform target;
    private Vector3 lastKnownPosition;
    private bool isChasing;
    private bool isAttacking;
    private float nextFireTime = 0f;

    // === Главное свойство: направление взгляда ===
    private Vector2 ViewDirection => viewOrigin != null ? viewOrigin.right : transform.right;

    private void Awake()
    {
        hits = new RaycastHit2D[countRays];
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isDead)
        {
            DetectPlayer();

            if (isChasing && !isAttacking)
                ChasePlayer();
            else
                SetMoveAnimation(false);

            if (PlayerInSight && canAttack && IsPlayerInAttackRange())
            {
                StartAttack();
            }
        }
    }

    private bool IsPlayerInAttackRange()
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position) <= attackDistance;
    }

    private void StartAttack()
    {
        if (!canAttack) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canAttack = false;

        StopMovement();
        SetMoveAnimation(false);

        PerformAttack();

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        isAttacking = false;
    }

    private void StopMovement()
    {
        // можно добавить сброс скорости или pathfinding если нужно
    }

    public void dead()
    {
        isDead = true;
        animator.SetBool("IsDead", isDead);
    }

    private void PerformAttack()
    {
        if (Melee || Range)
            StartCoroutine(MeleeAttackWithDelay(0.15f));
    }

    private IEnumerator MeleeAttackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Melee)
            TryMelee();
        else if (Range)
            TryRange();
    }

    void TryMelee()
    {
        if (!Melee) return;
        if (countRays <= 0) return;
        SetAttackAnimation();

        hits[0] = Physics2D.Raycast(viewOrigin.position, ViewDirection, distance, hitMask);

        for (int i = 1; i < countRays; i++)
        {
            Vector3 lastray = viewOrigin.up * (range * i / countRays);
            hits[i] = Physics2D.Raycast(viewOrigin.position, (ViewDirection + (Vector2)lastray).normalized, distance, hitMask);

            if (i + 1 < countRays)
            {
                i++;
                hits[i] = Physics2D.Raycast(viewOrigin.position, (ViewDirection - (Vector2)lastray).normalized, distance, hitMask);
            }
        }

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            if (((1 << hit.collider.gameObject.layer) & hitMask) != 0)
            {
                if (hit.collider.TryGetComponent(out HpController hp))
                    hp.TakeDamage(damage);
            }
        }
    }

    void TryRange()
    {
        if (!Range) return;
        if (Time.time < nextFireTime) return;
        SetAttackAnimation();

        GameObject trowObj = Instantiate(Prefab, shootPoint.transform.position, Quaternion.identity);
        if (trowObj.TryGetComponent(out Rigidbody2D rb))
            rb.AddForce(ViewDirection * bulletSpeed, ForceMode2D.Impulse);

        nextFireTime = Time.time + fireRate;
    }

    private void DetectPlayer()
    {
        Transform origin = viewOrigin != null ? viewOrigin : transform;

        RaycastHit2D hit = Physics2D.CircleCast(origin.position, viewDistance, ViewDirection, 0f, playerMask);
        if (hit.collider == null)
        {
            PlayerInSight = false;
            return;
        }

        target = hit.transform;
        Vector2 toTarget = target.position - origin.position;
        float dist = toTarget.magnitude;

        if (dist > viewDistance)
        {
            PlayerInSight = false;
            return;
        }

        float angle = Vector2.Angle(ViewDirection, toTarget);
        PlayerInSight = angle <= viewAngle * 0.5f;

        if (PlayerInSight)
        {
            lastKnownPosition = target.position;
            isChasing = true;
            RotateTowards(target.position);
        }
    }

    private void ChasePlayer()
    {
        if (target == null || isAttacking) return;

        if (PlayerInSight)
        {
            if (!IsPlayerInAttackRange())
            {
                MoveTowards(target.position);
                SetMoveAnimation(true);
            }
            else
            {
                SetMoveAnimation(false);
            }
            RotateTowards(target.position);
        }
        else
        {
            RotateTowards(lastKnownPosition);
            MoveTowards(lastKnownPosition);
            SetMoveAnimation(true);

            if (Vector2.Distance(transform.position, lastKnownPosition) < chaseStopDistance)
            {
                isChasing = false;
                SetMoveAnimation(false);
            }
        }
    }

    private void MoveTowards(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void SetMoveAnimation(bool state)
    {
        if (animator != null)
            animator.SetBool("IsMove", state);
    }

    private void SetAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    private void OnDrawGizmos()
    {
        if (!showDebug) return;

        Transform origin = viewOrigin != null ? viewOrigin : transform;
        Vector3 pos = origin.position;
        Vector3 rightDir = Quaternion.Euler(0, 0, viewAngle / 2f) * ViewDirection * viewDistance;
        Vector3 leftDir = Quaternion.Euler(0, 0, -viewAngle / 2f) * ViewDirection * viewDistance;

        Gizmos.color = PlayerInSight ? detectedColor : viewColor;
        Gizmos.DrawRay(pos, rightDir);
        Gizmos.DrawRay(pos, leftDir);
        Gizmos.DrawWireSphere(pos, viewDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos, ViewDirection * distance);
        for (int i = 1; i < countRays; i++)
        {
            Vector3 lastray = viewOrigin.up * (range * i / countRays);
            Gizmos.DrawRay(pos, (ViewDirection + (Vector2)lastray).normalized * distance);

            if (i + 1 < countRays)
            {
                i++;
                Gizmos.DrawRay(pos, (ViewDirection - (Vector2)lastray).normalized * distance);
            }
        }
    }
}
