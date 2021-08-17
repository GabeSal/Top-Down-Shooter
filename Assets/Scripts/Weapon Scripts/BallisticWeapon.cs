using System;
using System.Collections;
using UnityEngine;

public class BallisticWeapon : WeaponBase
{
    #region Serialized Fields
    [Header("Accuracy Settings")]
    [SerializeField]
    [Range(0f, 1.5f)]
    [Tooltip("Weapon sway value reduces the accuracy of the weapon if > 0.")]
    private float _weaponSway = 0;
    [SerializeField]
    [Range(0f, 1f)]
    private float _aimedWeaponSway = 0;
    [Header("Fire Mode Settings")]
    [SerializeField]
    private bool _isFullAuto;
    [SerializeField]
    private bool _isBurstFire;
    [SerializeField]
    private bool _isBoltAction;
    [SerializeField]
    private bool _isShotgun;
    [SerializeField]
    [Range(2, 5)]
    private int _shotsPerBurst = 3;
    [SerializeField]
    [Range(0.05f, 0.1f)]
    private float _timeUntilNextBurstShot = 0.05f;
    [SerializeField]
    [Range(6, 12)]
    private int _pelletsPerShotgunBlast = 6;
    [SerializeField]
    private float _manualActionDelay;
    [SerializeField]
    [Range(1, 3)]
    private int _slotNumber = 1;

    [Header("Ballistic Weapon Prefabs")]
    [SerializeField]
    private LineRenderer _bulletTrail;
    #endregion

    #region Private Fields
    private WeaponAmmo _weaponAmmo;
    private WeaponInventory _playerWeaponInventory;
    private PlayerShootingUtility _playerShootingHandler;
    private float _previousWeaponSway;
    private bool _isFiring;
    #endregion

    #region Public Fields
    [Tooltip("Assign the key to be pressed to select the weapon component defined in the editor.")]
    public KeyCode weaponHotKey;
    #endregion

    #region Properties
    public int SlotNumber { get => _slotNumber; }
    #endregion

    #region Action Events
    public event Action OnFire;
    public event Action OutOfAmmo;
    public event Action OnManualAction;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _previousWeaponSway = _weaponSway;
        _playerWeaponInventory = GameManager.Instance.GetComponentInChildren<WeaponInventory>();

        _playerWeaponInventory.OnWeaponInventoryUpdate += SetPlayerShootingHandlerForBallisticWeapon;
        _weaponAmmo = GetComponent<WeaponAmmo>();

        GameManager.Instance.LoadingPlayableScene += SetBulletTrail;
    }

    private void Update()
    {
        if (GameManager.Instance.InputsAllowed && GameManager.GameIsPaused == false)
        {
            _fireTimer += Time.deltaTime;

            // Change weapon sway value if player is "aiming"
            if (Input.GetKey((KeyCode)PlayerControls.aim))
                _weaponSway = _aimedWeaponSway;
            else
                _weaponSway = _previousWeaponSway;

            // Play out of ammo sound event
            if (Input.GetKeyDown((KeyCode)PlayerControls.fireWeapon) && !_weaponAmmo.HasAmmo() &&
                !_weaponAmmo.IsReloading)
                OutOfAmmo?.Invoke();

            if (!_weaponAmmo.IsReloading)
            {
                // Check if player is holding the fire button down
                if (Input.GetKey((KeyCode)PlayerControls.fireWeapon) && _isFullAuto && !_isFiring && !_isBoltAction)
                {
                    if (CanFire() && !_isShotgun && !_isBurstFire)
                    {
                        FireWeapon();
                    }

                    if (CanFire() && !_isShotgun && _isBurstFire)
                    {
                        StartCoroutine(BurstFire());
                    }

                    if (CanFire() && _isShotgun && !_isBurstFire)
                    {
                        FireShotgun();
                    }
                }

                // Check if player just pressed fire button
                if (Input.GetKeyDown((KeyCode)PlayerControls.fireWeapon) && !_isFullAuto && !_isFiring)
                {
                    if (CanFire() && !_isShotgun && !_isBurstFire)
                    {
                        FireWeapon();
                    }

                    if (CanFire() && !_isShotgun && _isBurstFire)
                    {
                        StartCoroutine(BurstFire());
                    }

                    if (CanFire() && _isShotgun && !_isBurstFire)
                    {
                        FireShotgun();
                    }
                }
            }            
        }
    }

    private void OnDestroy()
    {
        _playerWeaponInventory.OnWeaponInventoryUpdate -= SetPlayerShootingHandlerForBallisticWeapon;
        GameManager.Instance.LoadingPlayableScene -= SetBulletTrail;
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

        Vector2 shootingDirection = _playerShootingHandler.SetShootingDirection
            (GetRandomValueFromWeaponSway(), GetRandomValueFromWeaponSway());

        RaycastHit2D hitInfo2D = Physics2D.Raycast(transform.GetChild(0).position, shootingDirection, 
            _weaponRange, _collisionLayers);

        if (_bulletTrail != null)
            StartCoroutine(DrawBulletTrailAtHitPoint(hitInfo2D, shootingDirection));

        Collider2D target = hitInfo2D.collider;

        if (target != null)
        {
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

        if (_isBoltAction && !_isFullAuto)
            StartCoroutine(InvokeManualWeaponAction());
    }
    /// <summary>
    /// Method that resets the _fireTimer and creates a numerous amount of 2D raycasts with a for loop to collide with 
    /// objects of interest. Once hit, the targets will have their TakeHit() and Spawn__Particle() methods called,
    /// if they are an enemy, or simply spawn a particle in the environment for hit confirmation.
    /// Then the OnFire() event will invoke whatever gameobject is receiving the broadcast.
    /// </summary>
    private void FireShotgun()
    {
        _fireTimer = 0;

        for (int pellets = 0; pellets < _pelletsPerShotgunBlast; pellets++)
        {
            Vector2 shootingDirection = _playerShootingHandler.SetShootingDirection
            (GetRandomValueFromWeaponSway(), GetRandomValueFromWeaponSway());

            RaycastHit2D hitInfo2D = Physics2D.Raycast(transform.GetChild(0).position, shootingDirection,
                _weaponRange, _collisionLayers);

            Collider2D target = hitInfo2D.collider;

            if (target != null && IsEnemy(target))
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

        if (!_isFullAuto)
            StartCoroutine(InvokeManualWeaponAction());        
    }

    private IEnumerator InvokeManualWeaponAction()
    {
        yield return new WaitForSeconds(_manualActionDelay);

        OnManualAction?.Invoke();
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
    private IEnumerator DrawBulletTrailAtHitPoint(RaycastHit2D hit2D, Vector3 shotDirection)
    {
        _bulletTrail.enabled = true;

        _bulletTrail.SetPosition(0, transform.GetChild(0).position);
        if (hit2D.point != Vector2.zero)
        {
            _bulletTrail.SetPosition(1, hit2D.point);
        }
        else
        {
            _bulletTrail.SetPosition(1, transform.GetChild(0).position + (shotDirection * _weaponRange));
        }

        yield return new WaitForSeconds(0.05f);

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

    private void SetPlayerShootingHandlerForBallisticWeapon()
    {
        _playerShootingHandler = FindObjectOfType<PlayerShootingUtility>();
    }

    private void SetBulletTrail()
    {
        SetPlayerShootingHandlerForBallisticWeapon();

        if (_bulletTrail == null)
            _bulletTrail = FindObjectOfType<PlayerMovement>().GetComponentInChildren<LineRenderer>();
    }
    #endregion
}
