using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    [SerializeField] private Transform eye;
    [SerializeField] private float maxDistance = 0.5f;
    private Vector3 eyeDefault;
    private void Start()
    {
        eyeDefault = eye.localPosition;
    }
    public void MoveEyes(Vector3 vector)
    {
        Vector3 clampedVector = Vector3.ClampMagnitude(vector, maxDistance);
        eye.localPosition = eyeDefault + clampedVector;
    }
    public void returnEyes()
    {
        eye.localPosition = eyeDefault;
    }
}
