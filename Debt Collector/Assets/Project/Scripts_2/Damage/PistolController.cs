using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class PistolController : MonoBehaviour
{
    public UnityEvent OnThrow;
    public bool canRange;
    [Header("Ammo")]
    [SerializeField] private int magazineSize = 6;
    [SerializeField] private int currentAmmo;

    [Header("Fire settings")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private bool IsOun = false;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float volume =0.02f;
    [SerializeField] private int additionalmoney =5;
    private Coroutine fadeCoroutine;
    private static AudioSource musicSource;

    [Header("Line settings")]
    [SerializeField] private float lineDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float hitRange = 50f;
    [SerializeField] private LineRenderer line;

    [Header("Throw settings")]
    [SerializeField] private float throwSpeed = 8f;
    [SerializeField] private bool autoThrowOnEmpty = true;
    [SerializeField] private bool keepScriptAfterThrow = false;
    [SerializeField] private GameObject thrownPrefab;
    [SerializeField] private float thrownGravityScale = 0f;
    [SerializeField] private float rotationSpeed = 200f;
    private Transform throwPoint;
    private Rigidbody2D thrownRb;
    private Collider2D thrownCollider;

    [Header("Events")]
    [SerializeField] private UnityEvent OnFire;
    [SerializeField] private UnityEvent OnEmpty;

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
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.volume = volume;

        if (thrownRb != null) thrownRb.gravityScale = 0;
        line.enabled = false;
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    public void TryShoot()
    {
        if (IsThrown|| !IsOun|| !canRange) return;
        if (Time.time < nextFireTime) return;

        if (currentAmmo > 0)
        {
            Shoot();
        }
        else
        {
            OnEmpty?.Invoke();
            if (autoThrowOnEmpty)
                Throw();
        }
    }

    private void Shoot()
    {
        if (shootPoint == null || gameObject == null) return;
        Vector2 directionCast = Vector2.zero;
        if (shootPoint.lossyScale.x > 0)
        {
            directionCast = shootPoint.right;
        }
        else
        {
            directionCast = -shootPoint.right;
        }
  

        RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, directionCast, hitRange, hitMask);
        if (hit.collider != null)
        {
            if(hit.collider.TryGetComponent(out HpController hp))
            {
                hp.TakeDamage(damage);
                hp.SpawnCoinAdditional(additionalmoney);
            }
     
        }
        Vector3 endPoint = hit.collider ? (Vector3)hit.point : (Vector3)(shootPoint.position + (Vector3)directionCast * hitRange);

        Color startColor = line.startColor;
        Color endColor = line.endColor;
        startColor.a = 1f; 
        endColor.a = 1f;

        line.startColor = startColor;
        line.endColor = endColor;

        line.enabled = true;
        line.SetPosition(0, shootPoint.position);
        line.SetPosition(1, endPoint);

        currentAmmo--;
        OnFire?.Invoke();
        OnAmmoChanged?.Invoke(currentAmmo);

        nextFireTime = Time.time + fireRate;
        musicSource.PlayOneShot(musicClip);
        if (currentAmmo <= 0)
            OnEmpty?.Invoke();
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeLine(startColor, endColor));
    }

    public void Throw()
    {
        if (IsThrown) return;

        Vector2 directionCast = Vector2.zero;
        if (shootPoint.lossyScale.x > 0)
        {
            directionCast = shootPoint.right;
        }
        else
        {
            directionCast = -shootPoint.right;
        }

        if (thrownPrefab != null)
        {
            Transform spawn = throwPoint;
            GameObject t = Instantiate(thrownPrefab, spawn.position, spawn.rotation);
            Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = thrownGravityScale;
                rb.AddForce(directionCast * throwSpeed, ForceMode2D.Force);
                rb.angularVelocity = rotationSpeed;
            }
        }
        else
        { 
            if (thrownRb != null)
            {
                thrownRb.isKinematic = false;
                thrownRb.velocity = directionCast.normalized * throwSpeed;
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
        IsOun = true;
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
    private System.Collections.IEnumerator FadeLine(Color startColor, Color endColor)
    {
        yield return new WaitForSecondsRealtime(lineDuration);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            line.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            line.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

            yield return null;
        }

        line.enabled = false;
    }
    private void OnDrawGizmos()
    {
        Vector2 directionCast = Vector2.zero;
        if (shootPoint.lossyScale.x > 0)
        {
            directionCast = shootPoint.right;
        }
        else
        {
            directionCast = -shootPoint.right;
        }
        Gizmos.DrawLine(shootPoint.position, directionCast * hitRange);
    }
}
