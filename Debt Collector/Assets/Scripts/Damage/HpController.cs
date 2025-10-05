using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HpController : MonoBehaviour
{
    [SerializeField] private int hp;
    private UnityEvent OnDead;

    public void TakeDamage(int value)
    {
        hp -= value;
        OnDead.Invoke();
    }

}
