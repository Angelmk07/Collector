using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class PistolController : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int magazineSize = 6;
    [SerializeField] private int currentAmmo;

    [Header("Fire settings")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private bool IsOun = false;

    [Header("Line settings")]
    [SerializeField] private float lineDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float hitRange = 50f;
    private LineRenderer line;

    [Header("Throw settings")]
    [SerializeField] private float throwSpeed = 8f;
    [SerializeField] private bool autoThrowOnEmpty = true;
    [SerializeField] private bool keepScriptAfterThrow = false;
    [SerializeField] private GameObject thrownPrefab;
    [SerializeField] private float thrownGravityScale = 0f;
    [SerializeField] private float rotationDamping = 2f;
    private Transform throwPoint;
    private Rigidbody2D thrownRb;
    private Collider2D thrownCollider;

    [Header("Events")]
    [SerializeField] private UnityEvent OnFire;
    [SerializeField] private UnityEvent OnEmpty;
    [SerializeField] private UnityEvent OnThrow;
    [SerializeField] private UnityEvent<int> OnAmmoChanged;

    public bool IsEmpty => currentAmmo <= 0;
    public bool IsThrown { get; private set; }
    private float nextFireTime = 0f;

    private void Awake()
    {
        if (currentAmmo <= 0) currentAmmo = magazineSize;
        throwPoint = shootPoint;
        thrownRb = thrownPrefab.GetComponent<Rigidbody2D>();
        thrownCollider = thrownPrefab.GetComponent<Collider2D>();

        if (thrownRb != null) thrownRb.isKinematic = true;
        if (thrownRb != null) thrownRb.gravityScale = 0;
        if (thrownCollider != null) thrownCollider.enabled = false;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    public void TryShoot()
    {
        if (IsThrown|| !IsOun) return;
        if (Time.time < nextFireTime) return;

        if (currentAmmo > 0)
        {
            Shoot();
        }
        else
        {
            OnEmpty?.Invoke();
            if (autoThrowOnEmpty)
                Throw(shootPoint.forward);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null || gameObject == null)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, shootPoint.forward, hitRange, hitMask);
        Vector3 endPoint = hit.collider ? (Vector3)hit.point : shootPoint.position + (Vector3)shootPoint.forward * hitRange;
        line.enabled = true;
        line.SetPosition(0, shootPoint.position);
        line.SetPosition(1, endPoint);

        currentAmmo--;
        OnFire?.Invoke();
        OnAmmoChanged?.Invoke(currentAmmo);

        nextFireTime = Time.time + fireRate;



        if (currentAmmo <= 0)
            OnEmpty?.Invoke();
        StartCoroutine(FadeLine());
    }
    public void Throw(Vector2 direction)
    {
        if (IsThrown) return;

        if (thrownPrefab != null)
        {
            Transform spawn = throwPoint;
            GameObject t = Instantiate(thrownPrefab, spawn.position, spawn.rotation);
            Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = thrownGravityScale;
                rb.velocity = direction.normalized * throwSpeed;
                rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0f, rotationDamping * Time.deltaTime);
            }
        }
        else
        { 
            if (thrownRb != null)
            {
                thrownRb.isKinematic = false;
                thrownRb.velocity = direction.normalized * throwSpeed;
                thrownRb.gravityScale = thrownGravityScale;
            }
            if (thrownCollider != null)
                thrownCollider.enabled = true;
        }

        IsThrown = true;
        OnThrow?.Invoke();

        if (!keepScriptAfterThrow)
            enabled = false;
    }

    public void Throw()
    {
        Throw(transform.right);
    }

    public int Reload(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, magazineSize);
        OnAmmoChanged?.Invoke(currentAmmo);
        return currentAmmo;
    }

    public void SetAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, magazineSize);
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    public int GetAmmo() => currentAmmo;
    public void ResetState()
    {
        IsThrown = false;
        currentAmmo = magazineSize;
        nextFireTime = 0f;

        if (thrownRb != null)
        {
            thrownRb.isKinematic = true;
            thrownRb.velocity = Vector2.zero;
            thrownRb.angularVelocity = 0f;
        }

        if (thrownCollider != null)
            thrownCollider.enabled = false;

        OnAmmoChanged?.Invoke(currentAmmo);
    }
    private System.Collections.IEnumerator FadeLine()
    {
        float timer = 0f;

        Color startColor = line.startColor;
        Color endColor = line.endColor;
        float startAlpha = startColor.a;

        yield return new WaitForSecondsRealtime(lineDuration);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, 0f, t);
            Color newStart = new Color(startColor.r, startColor.g, startColor.b, alpha);
            Color newEnd = new Color(endColor.r, endColor.g, endColor.b, alpha);
            line.startColor = newStart;
            line.endColor = newEnd;

            yield return null;
        }

        line.enabled = false;
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            TryShoot();
    }
#endif
}
