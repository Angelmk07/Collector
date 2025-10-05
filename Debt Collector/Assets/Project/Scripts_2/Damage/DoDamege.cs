using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamege : MonoBehaviour
{
    [SerializeField] private LayerMask Damageble;
    [SerializeField] private int Damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & Damageble) != 0)
        {
            if(collision.gameObject.TryGetComponent(out HpController hp))
            {
                hp.TakeDamage(Damage);
            }
        }
    }
}
