using System;
using System.Collections;
using UnityEngine;

public class BallisticWeapon : WeaponBase
{
    #region Serialized Fields
    [Header("Ballistic Weapon Settings")]
    [SerializeField]
    [Range(0f, 1.5f)]
    [Tooltip("Weapon sway value reduces the accuracy of the weapon if > 0.")]
    private float _weaponSway;
    [SerializeField]
    [Range(0f, 1f)]
    private float _aimedWeaponSway;
    [SerializeField]
    private bool _isFullAuto;
    [SerializeField]
    private bool _isBurstFire;
    [SerializeField]
    [Range(2, 5)]
    private int _shotsPerBurst = 3;
    [SerializeField]
    [Range(0.05f, 0.1f)]
    private float _timeUntilNextBurstShot;

    [Header("Ballistic Weapon Prefabs")]
    [SerializeField]
    private LineRenderer _bulletTrail;
    #endregion

    #region Private Fields
    private float _previousWeaponSway;
    private bool _isFiring;
    #endregion

    #region Action Events
    public event Action OnFire;
    public event Action OutOfAmmo;
    #endregion

    #region Standard Unity Methods
    protected override void Awake()
    {
        _previousWeaponSway = _weaponSway;
        base.Awake();
    }

    private void Update()
    {
        if (GameManager.Instance.InputsAllowed)
        {
            _fireTimer += Time.deltaTime;

            // Change weapon sway value if player is "aiming"
            if (Input.GetButton("Fire2"))
                _weaponSway = _aimedWeaponSway;
            else
                _weaponSway = _previousWeaponSway;

            // Check if player is holding the fire button down
            if (Input.GetButton("Fire1") && _isFiring == false)
            {
                if (CanFire() && _isFullAuto && _isBurstFire == false)
                {
                    FireWeapon();
                }

                if (CanFire() && _isFullAuto && _isBurstFire)
                {
                    StartCoroutine(BurstFire());
                }
            }

            // Check if player just pressed fire button
            if (Input.GetButtonDown("Fire1") && _isFiring == false)
            {
                if (CanFire() && _isBurstFire)
                {
                    StartCoroutine(BurstFire());
                }

                if (CanFire() && _isFullAuto == false)
                {
                    FireWeapon();
                }                
            }

            // Play the ammo clicks event when out of ammo
            if (Input.GetButtonDown("Fire1") && _weaponAmmo.HasAmmo() == false)
                OutOfAmmo?.Invoke();
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Method that resets the _fireTimer and creates a 2D raycast to collide with objects of interest. 
    /// Once hit, the targets will have their TakeHit() and Spawn__Particle() methods called, if they are
    /// an enemy, or simply spawn a particle in the environment for hit confirmation.
    /// Then the OnFire() event will invoke whatever gameobject is receiving the broadcast.
    /// </summary>
    private void FireWeapon()
    {
        _fireTimer = 0;

        Vector2 shootingDirection = _playerShooting.SetShootingDirection
            (GetRandomValueFromWeaponSway(), GetRandomValueFromWeaponSway());

        RaycastHit2D hitInfo2D = Physics2D.Raycast(_firePoint.position, shootingDirection, _weaponRange, _collisionLayers);

        Collider2D target = hitInfo2D.collider;

        if (target != null)
        {
            if (_bulletTrail != null)
                StartCoroutine(DrawBulletTrailAtHitPoint(hitInfo2D));
            
            if (IsEnemy(target))
            {
                target.GetComponent<Health>().TakeHit(_weaponDamage);
                target.GetComponent<EnemyStatus>().SpawnBloodSplatterParticle(hitInfo2D.point, hitInfo2D.normal);
            }
            else
            {
                SpawnBulletImpactParticle(hitInfo2D.point, hitInfo2D.normal);
            }
        }
        OnFire?.Invoke();
    }

    /// <summary>
    /// Method that activates a member of the _bulletImpactParticle pool at a specified origin and orientation
    /// then deactivating the member after a specified amount of time (in seconds).
    /// </summary>
    /// <param name="origin">Vector2 passed from the FireWeapon() method using the hitInfo2D.point property.</param>
    /// <param name="direction">Vector2 passed from the FireWeapon() method using the hitInfo2D.normal property.</param>
    private void SpawnBulletImpactParticle(Vector2 origin, Vector2 direction)
    {
        var particle = _projectileImpactParticle.Get<PooledMonoBehaviour>(origin, Quaternion.LookRotation(-direction));
        particle.ReturnToPool(1f);
    }

    /// <summary>
    /// Coroutine that visualizes the bullet path of the weapon. First the method enables the line renderer 
    /// component of the _bulletTrail object, then sets the positions of the line renderer to match the trajectory
    /// of the raycast/bullet path, and finally disables the line renderer after a short duration.
    /// </summary>
    /// <param name="hit2D"></param>
    /// <returns></returns>
    private IEnumerator DrawBulletTrailAtHitPoint(RaycastHit2D hit2D)
    {
        _bulletTrail.enabled = true;

        _bulletTrail.SetPosition(0, _firePoint.position);
        _bulletTrail.SetPosition(1, hit2D.point);

        yield return new WaitForSeconds(0.03f);

        _bulletTrail.enabled = false;
    }

    /// <summary>
    /// Coroutine that begins a for loop that fires a round and delays the next shot in the burst fire 
    /// until the amount of rounds fired reaches the _shotsPerBurst value.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BurstFire()
    {
        for (int i = 0; i < _shotsPerBurst && _weaponAmmo.AmmoInClip > 0; i++)
        {
            FireWeapon();
            _isFiring = true;
            yield return new WaitForSeconds(_timeUntilNextBurstShot);
        }

        _isFiring = false;
    }

    /// <summary>
    /// Method that generates a value between the + and - _weaponSway values, which are used as a range.
    /// </summary>
    /// <returns>Float value that will be used to offset the raycast direction to reduce accuracy of the
    /// weapon.</returns>
    private float GetRandomValueFromWeaponSway()
    {
        float randomGeneratedValue = UnityEngine.Random.Range(-_weaponSway, _weaponSway);

        return randomGeneratedValue;
    }

    /// <summary>
    /// Checks if the player has sufficient ammo in the clip and if the _fireTimer has surpassed the _fireDelay value.
    /// </summary>
    /// <returns>True if there is enough ammo in the current weapons clip and _fireTimer is greater than _fireDelay.</returns>
    private bool CanFire()
    {
        if (_weaponAmmo != null && _weaponAmmo.HasAmmo() == false)
            return false;

        return _fireTimer >= _fireDelay;
    }

    /// <summary>
    /// Method that checks if the target is tagged with the "Enemy" label.
    /// </summary>
    /// <param name="target">Collider2D object that is passed from the FireWeapon() 
    /// method once the raycast hit something.</param>
    /// <returns>True if the target is tagged as an Enemy type.</returns>
    private bool IsEnemy(Collider2D target)
    {
        return target.CompareTag("Enemy");
    }
    #endregion
}
