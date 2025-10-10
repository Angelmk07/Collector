using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HpController : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private bool addcoin;
    [SerializeField] private GameObject coin;
    [SerializeField] private int coinCount;
    public UnityEvent OnDead;

    public void TakeDamage(int value)
    {
        hp -= value;
        OnDead.Invoke();
        SpawnCoinCount(coinCount);
    }
    public void SpawnCoinAdditional(int val)
    {
        if(hp<=0)
            SpawnCoinCount(val);
    }
    public void SpawnCoinCount(int val)
    {
        if (addcoin)
        {
            for(int i =0; i<val;i++)
                Instantiate(coin, transform.position, Quaternion.identity);
        }
            
    }

}
