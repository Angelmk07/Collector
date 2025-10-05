using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boostrapper : MonoBehaviour
{
    [SerializeField] private ClickListener ClickListener;
    [SerializeField] private PistolController pistol;
    private void Awake()
    {
        ClickListener.OnClicl.AddListener(pistol.TryShoot);
    }
}
