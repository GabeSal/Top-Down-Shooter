using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private KeyCode _weaponHotKey;
    [SerializeField]
    private float _recoilForce = 2f;
    [SerializeField]
    private float _fireDelay;
    [SerializeField]
    private bool _isFullAuto;

    private float _fireTimer;

    public event Action OnFire = delegate { };
    public float RecoilForce { get; private set; }
    public bool IsFullAuto { get { return _isFullAuto; } }
    public bool IsFiring { get { return CanFire(); } }
    public KeyCode WeaponHotKey { get { return _weaponHotKey; } }

    private void Awake()
    {
        RecoilForce = _recoilForce;
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (_isFullAuto == true && Input.GetButton("Fire1"))
        {
            if (CanFire())
            {
                FireWeapon();
            }
        }

        if (_isFullAuto == false && Input.GetButtonDown("Fire1"))
        {
            if (CanFire())
            {
                FireWeapon();
            }
        }
    }

    private bool CanFire()
    {
        return _fireTimer >= _fireDelay;
    }

    private void FireWeapon()
    {
        _fireTimer = 0;
        OnFire();
    }
}
