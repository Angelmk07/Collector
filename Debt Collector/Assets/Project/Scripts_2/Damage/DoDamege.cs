using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamege : MonoBehaviour
{
    [SerializeField] private bool destroyOnCooldown;
    [SerializeField] private float destroyDelay = 3f;
    [SerializeField] private LayerMask Damageble;
    [SerializeField] private int Damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & Damageble) != 0)
        {
            if(collision.gameObject.TryGetComponent(out HpController hp))
            {
                hp.TakeDamage(Damage);
            }
        }
        Destroy(gameObject);
    }
    private void Start()
    {
        if (destroyOnCooldown)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
