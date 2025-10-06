using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HpController : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private GameObject coin;
    public UnityEvent OnDead;

    public void TakeDamage(int value)
    {
        hp -= value;
        OnDead.Invoke();
        Instantiate(coin, transform.position, Quaternion.identity);
    }

}
