using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _recoilForce = 2f;
    [SerializeField]
    private int _weaponDamage = 1;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private float _fireDelay;
    [SerializeField]
    private bool _isFullAuto;
    [SerializeField]
    private KeyCode _weaponHotKey;

    private float _fireTimer;
    private PlayerShooting _playerShooting;

    public event Action OnFire = delegate { };
    public float RecoilForce { get; private set; }
    public bool IsFullAuto { get { return _isFullAuto; } }
    public bool IsFiring { get { return CanFire(); } }
    public KeyCode WeaponHotKey { get { return _weaponHotKey; } }

    private void Awake()
    {
        RecoilForce = _recoilForce;
        _playerShooting = GetComponentInParent<PlayerShooting>();
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (Input.GetButton("Fire1"))
        {
            if (CanFire() && _isFullAuto == true)
            {
                FireWeapon();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (CanFire() && _isFullAuto == false)
            {
                FireWeapon();
            }
        }
    }

    private bool CanFire()
    {
        return _fireTimer >= _fireDelay;
    }

    private bool IsEnemy(Collider2D game_object)
    {
        return game_object.GetComponent<Health>() != null;
    }

    private void FireWeapon()
    {
        _fireTimer = 0;

        RaycastHit2D hitInfo2D = Physics2D.Raycast(_firePoint.position, _playerShooting.GetMouseDirection());
        Collider2D something = hitInfo2D.collider;

        if (something != null)
        {
            if (IsEnemy(something))
            {
                something.GetComponent<EnemyStatus>().RecoilFromHit(-hitInfo2D.normal);
                something.GetComponent<Health>().TakeHit(_weaponDamage);
            }
        }

        OnFire();
    }
}
