using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boostrapper : MonoBehaviour
{
    [SerializeField] private ClickListener ClickListener;
    [SerializeField] private PistolController pistol;
    [SerializeField] private Eyes eyes;
    [SerializeField] private RotateToMouse2D mouse2D;
    [SerializeField] private WeaponSwitcher weapon;
    private void Awake()
    {
        ClickListener.OnClicl.AddListener(pistol.TryShoot);
        mouse2D.StopOnMousceMove.AddListener(eyes.returnEyes);
        mouse2D.OnMouseMove.AddListener(eyes.MoveEyes);
        pistol.OnThrow.AddListener(weapon.RemoveWeaponAccess);
    }
}
