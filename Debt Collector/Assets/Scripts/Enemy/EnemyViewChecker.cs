using UnityEngine;
using System.Collections;

public class EnemyViewChecker : MonoBehaviour
{
    public bool PlayerInSight { get; private set; }

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
    [SerializeField] private float distance;
    [SerializeField] private float range;
    [SerializeField] private int countRays;
    [SerializeField] private RaycastHit2D[] hits;

    private Transform target;
    private Vector3 lastKnownPosition;
    private bool isChasing;
    private bool isAttacking;

    private void Awake()
    {
        hits = new RaycastHit2D[countRays];
    }

    private void Update()
    {
        DetectPlayer();

        if (isChasing && !isAttacking)
            ChasePlayer();

        // Проверяем возможность атаки
        if (PlayerInSight && canAttack && IsPlayerInAttackRange())
        {
            StartAttack();
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

        // Останавливаем преследование на время атаки
        StopMovement();

        // Вызываем метод атаки
        PerformAttack();

        // Ждем кулдаун
        yield return new WaitForSeconds(attackCooldown);

        // Возобновляем возможность атаки и движения
        canAttack = true;
        isAttacking = false;
    }

    private void StopMovement()
    {
        // Можно добавить дополнительную логику остановки здесь
        // Например, сброс velocity если используете Rigidbody2D
    }

    private void PerformAttack()
    {
        // Здесь реализуйте логику атаки
        Debug.Log("Атакую игрока!");

        // Вызов вашего метода TryMelee или другой логики атаки
        TryMelee();

        // Дополнительно: можно добавить анимацию, нанесение урона и т.д.
    }

    void TryMelee()
    {
        if (countRays <= 0) return;

        hits[0] = Physics2D.Raycast(transform.position, transform.right * distance);

        for (int i = 1; i < countRays; i++)
        {
            Vector3 lastray = transform.up * (range * i / countRays);
            hits[i] = Physics2D.Raycast(transform.position, (transform.right + lastray) * distance);

            if (i + 1 < countRays)
            {
                i++;
                hits[i] = Physics2D.Raycast(transform.position, (transform.right - lastray) * distance);
            }
        }

        // Проверка попаданий по игроку
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // Нанесение урона игроку
                Debug.Log("Попал по игроку!");
                // hit.collider.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }

    private void DetectPlayer()
    {
        Transform origin = viewOrigin != null ? viewOrigin : transform;

        RaycastHit2D hit = Physics2D.CircleCast(origin.position, viewDistance, origin.right, 0f, playerMask);

        if (hit.collider == null)
        {
            PlayerInSight = false;
            return;
        }

        target = hit.transform;
        Vector2 toTarget = target.position - origin.position;
        float distance = toTarget.magnitude;

        if (distance > viewDistance)
        {
            PlayerInSight = false;
            return;
        }

        float angle = Vector2.Angle(origin.right, toTarget);
        PlayerInSight = angle <= viewAngle * 0.5f;

        if (PlayerInSight)
        {
            lastKnownPosition = target.position;
            isChasing = true;

            // Поворачиваемся к игроку
            RotateTowards(target.position);
        }
    }

    private void ChasePlayer()
    {
        if (target == null || isAttacking) return;

        if (PlayerInSight)
        {
            // Если игрок в зоне атаки - не двигаемся, только поворачиваемся
            if (!IsPlayerInAttackRange())
            {
                MoveTowards(target.position);
            }
            RotateTowards(target.position);
        }
        else
        {
            // Поворачиваемся к последней известной позиции
            RotateTowards(lastKnownPosition);
            MoveTowards(lastKnownPosition);

            if (Vector2.Distance(transform.position, lastKnownPosition) < chaseStopDistance)
            {
                isChasing = false;
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

        // Вычисляем угол поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Применяем поворот (в 2D обычно вокруг оси Z)
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnDrawGizmos()
    {
        if (!showDebug) return;

        Transform origin = viewOrigin != null ? viewOrigin : transform;
        Vector3 pos = origin.position;
        Vector3 rightDir = Quaternion.Euler(0, 0, viewAngle / 2f) * origin.right * viewDistance;
        Vector3 leftDir = Quaternion.Euler(0, 0, -viewAngle / 2f) * origin.right * viewDistance;

        Gizmos.color = PlayerInSight ? detectedColor : viewColor;
        Gizmos.DrawRay(pos, rightDir);
        Gizmos.DrawRay(pos, leftDir);
        Gizmos.DrawWireSphere(pos, viewDistance);

        // Визуализация дистанции атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * distance);
        for (int i = 1; i < countRays; i++)
        {
            Vector3 lastray = transform.up * (range * i / countRays);
            Gizmos.DrawRay(transform.position, (transform.right + lastray) * distance);

            if (i + 1 < countRays)
            {
                i++;
                Gizmos.DrawRay(transform.position, (transform.right - lastray) * distance);
            }
        }
    }
}