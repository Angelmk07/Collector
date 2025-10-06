using UnityEngine;

public class NoRotateChild : MonoBehaviour
{
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation; 
    }

    void LateUpdate()
    {
        transform.rotation = startRotation; 
    }
}
