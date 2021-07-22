using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Serialized Fields
    [Header("Weapon Settings")]
    [SerializeField]
    [Range(1, 15)]
    private int _weaponDamage;
    [SerializeField]
    [Range(40f, 1000f)]
    private float _weaponRange;
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
    [Range(0.04f, 2f)]
    private float _fireDelay;

    [Header("Weapon Prefabs")]
    [SerializeField]
    [Tooltip("Used to specify the origin of which to draw the raycast of the weapon.")]
    private Transform _firePoint;
    [SerializeField]
    private PooledMonoBehaviour _bulletImpactParticle;
    [SerializeField]
    private LineRenderer _bulletTrail;
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    [Tooltip("Assign the key to be pressed to select the weapon component defined in the editor.")]
    private KeyCode _weaponHotKey;    
    
    #endregion

    #region Private Fields
    private float _fireTimer;
    private float _previousWeaponSway;
    private PlayerShooting _playerShooting;
    private WeaponAmmo _ammo;
    #endregion

    #region Properties
    public bool IsFullAuto { get => _isFullAuto; }
    public bool IsFiring { get => CanFire(); }
    public KeyCode WeaponHotKey { get => _weaponHotKey; }
    #endregion

    #region Action Events
    public event Action OnFire;
    public event Action OutOfAmmo;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _previousWeaponSway = _weaponSway;
        _ammo = GetComponent<WeaponAmmo>();
        _playerShooting = GetComponentInParent<PlayerShooting>();
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        // Check if player is holding button down
        if (Input.GetButton("Fire1"))
        {
            if (CanFire() && _isFullAuto == true)
            {
                FireWeapon();
            }
        }

        // Check if player just pressed button
        if (Input.GetButtonDown("Fire1"))
        {
            if (CanFire() && _isFullAuto == false)
            {
                FireWeapon();
            }
        }

        // Play the ammo clicks event when out of ammo
        if (Input.GetButtonDown("Fire1") && _ammo.IsAmmoReady() == false)
        {
            OutOfAmmo?.Invoke();
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Checks if the player has sufficient ammo in the clip and if the _fireTimer has surpassed the _fireDelay value.
    /// </summary>
    /// <returns>True if there is enough ammo in the current weapons clip and _fireTimer is greater than _fireDelay.</returns>
    private bool CanFire()
    {
        if (_ammo != null && _ammo.IsAmmoReady() == false)
            return false;

        return _fireTimer >= _fireDelay;
    }

    /// <summary>
    /// Method that resets the _fireTimer and creates a 2D raycast to collide with objects of interest. 
    /// Once hit, the targets will have their TakeHit() and Spawn__Particle() methods called, if they are
    /// an enemy, or simply spawn a particle in the environment for hit confirmation.
    /// Then the OnFire() event will invoke whatever gameobject is receiving the broadcast.
    /// </summary>
    private void FireWeapon()
    {
        _fireTimer = 0;

        RaycastHit2D hitInfo2D = Physics2D.Raycast(_firePoint.position,
            _playerShooting.GetMouseDirection(GetRandomValueFromWeaponSway()), _weaponRange, _layerMask);

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
    /// Method that generates a value between the + and - _weaponSway values, which are used as a range.
    /// </summary>
    /// <returns>Float value that will be used to offset the raycast direction to reduce accuracy of the
    /// weapon.</returns>
    private float GetRandomValueFromWeaponSway()
    {
        if (Input.GetButton("Fire2"))
            _weaponSway = _aimedWeaponSway;
        else
            _weaponSway = _previousWeaponSway;

        float randomGeneratedValue = UnityEngine.Random.Range(-_weaponSway, _weaponSway);

        return randomGeneratedValue;
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

    /// <summary>
    /// Method that activates a member of the _bulletImpactParticle pool at a specified origin and orientation
    /// then deactivating the member after a specified amount of time (in seconds).
    /// </summary>
    /// <param name="origin">Vector2 passed from the FireWeapon() method using the hitInfo2D.point property.</param>
    /// <param name="direction">Vector2 passed from the FireWeapon() method using the hitInfo2D.normal property.</param>
    private void SpawnBulletImpactParticle(Vector2 origin, Vector2 direction)
    {
        var particle = _bulletImpactParticle.Get<PooledMonoBehaviour>(origin, Quaternion.LookRotation(-direction));
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

        yield return new WaitForSeconds(0.02f);

        _bulletTrail.enabled = false;
    }
    #endregion
}
