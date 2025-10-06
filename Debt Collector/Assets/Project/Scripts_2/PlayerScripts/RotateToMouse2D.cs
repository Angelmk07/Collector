using UnityEngine;
using UnityEngine.Events;

public class RotateToMouse2D : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform targetY;
    [SerializeField] private bool flipY = false;
    [SerializeField] private bool smoothRotation = false;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool IsUseRotateMod = false;

    private Camera cam;
    public UnityEvent<Vector3> OnMouseMove;
    public UnityEvent StopOnMousceMove;

    private void Awake()
    {
        cam = Camera.main;
        if (target == null)
            target = transform;
    }

    private void Update()
    {
        if (IsUseRotateMod)
        {
            flipY = targetY.localScale.x < 0;
            RotateToMouse();
        }

    }
    public void Stop()
    {
        IsUseRotateMod = false;
        StopOnMousceMove?.Invoke();
    }
    public void Continue()
    {
        IsUseRotateMod = true;
    }
    private void RotateToMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - target.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (flipY)
            angle += 180f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        if (smoothRotation)
            target.rotation = Quaternion.Lerp(target.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        else
            target.rotation = targetRotation;


        OnMouseMove?.Invoke(direction);

    }
}
