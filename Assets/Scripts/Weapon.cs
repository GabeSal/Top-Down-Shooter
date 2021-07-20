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
    private float _weaponRange = 50f;
    [SerializeField] [Range(0, 1.5f)]
    private float _weaponSway;    // Used to throw the weapon off target for accuracy control
    [SerializeField] [Range(0, 1)]
    private float _aimedWeaponSway;
    [SerializeField]
    private float _fireDelay;

    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private PooledMonoBehaviour _bulletImpactParticle;    
    [SerializeField]
    private bool _isFullAuto;
    [SerializeField]
    private KeyCode _weaponHotKey;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private LayerMask _layerMask;

    private float _fireTimer;
    private float _previousWeaponSway;
    private PlayerShooting _playerShooting;

    public event Action OnFire = delegate { };
    public float RecoilForce { get; private set; }
    public bool IsFullAuto { get { return _isFullAuto; } }
    public bool IsFiring { get { return CanFire(); } }
    public KeyCode WeaponHotKey { get { return _weaponHotKey; } }

    private void Start()
    {
        RecoilForce = _recoilForce;
        _previousWeaponSway = _weaponSway;
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

    private void FireWeapon()
    {
        _fireTimer = 0;

        RaycastHit2D hitInfo2D = Physics2D.Raycast(_firePoint.position, 
            _playerShooting.GetMouseDirection(GetRandomValueFromWeaponSway()), _weaponRange, _layerMask);
        Collider2D something = hitInfo2D.collider;

        if (something != null)
        {
            StartCoroutine(DrawBulletTrailAtHitPoint(hitInfo2D));
            if (IsEnemy(something))
            {
                something.GetComponent<Health>().TakeHit(_weaponDamage);
                something.GetComponent<EnemyStatus>().RecoilFromHit(-hitInfo2D.normal);
                something.GetComponent<EnemyStatus>().SpawnBloodSplatterParticle(hitInfo2D.point, hitInfo2D.normal);
            }
            else
            {
                SpawnBulletImpactParticle(hitInfo2D.point, hitInfo2D.normal);
            }
        }
        OnFire();
    }

    private float GetRandomValueFromWeaponSway()
    {
        if (Input.GetButton("Fire2"))
            _weaponSway = _aimedWeaponSway;
        else
            _weaponSway = _previousWeaponSway;

        return UnityEngine.Random.Range(-_weaponSway, _weaponSway);
    }

    private bool IsEnemy(Collider2D game_object)
    {
        return game_object.CompareTag("Enemy");
    }

    private void SpawnBulletImpactParticle(Vector2 point, Vector2 normal)
    {
        var particle = _bulletImpactParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(-normal));
        particle.ReturnToPool(1f);
    }

    private IEnumerator DrawBulletTrailAtHitPoint(RaycastHit2D hit2D)
    {
        _lineRenderer.enabled = true;

        _lineRenderer.SetPosition(0, _firePoint.position);
        _lineRenderer.SetPosition(1, hit2D.point);

        yield return new WaitForSeconds(0.05f);

        _lineRenderer.enabled = false;
    }
}
