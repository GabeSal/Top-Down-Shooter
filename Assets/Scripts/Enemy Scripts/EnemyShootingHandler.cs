using System;
using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyShootingHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Enemy Weapon Settings")]
    [SerializeField]
    [Range(2, 12)]
    private int _damage;
    [SerializeField]
    [Range(5.5f, 16f)]
    [Tooltip("Determines the range in which the enemy is allowed to fire their weapon.")]
    private float _range;
    [SerializeField]
    [Range(50f, 1000f)]
    [Tooltip("Determines the actual distance (for the raycast) in which the enemy weapon can shoot.")]
    private float _weaponRange;
    [SerializeField]
    [Range(0, 1.5f)]
    private float _weaponSwayAmount;
    [SerializeField]
    [Range(0.04f, 2f)]
    private float _timeUntilNextShot;
    [SerializeField]
    [Tooltip("Determines if the enemy weapon is fired in bursts rather than semi or fully automatic.")]
    private bool _isBurstFire;
    [SerializeField]
    [Range(2, 5)]
    private int _shotsPerBurst;
    [SerializeField]
    [Range(0.05f, 0.1f)]
    private float _timeUntilNextBurstShot;
    [SerializeField]
    [Tooltip("Determines if the enemy can shoot the player while chasing them.")]
    private bool _canShootAndRun;
    [SerializeField]
    [Tooltip("Determines which game object layers the bullets can collide with.")]
    private LayerMask _collisionLayers;

    [Header("Enemy Weapon Prefabs")]
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private PooledMonoBehaviour _bulletImpactParticle;
    [SerializeField]
    private LineRenderer _bulletTrail;
    #endregion

    #region Private Fields
    private float _shootingTimer = 0;
    private AIPath _aiPath;
    private LayerMask _playerLayerMask;
    private Transform _target;
    #endregion

    #region Action Events
    public event Action EnemyOnFire;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _playerLayerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (_aiPath.reachedDestination || _canShootAndRun)
        {
            _shootingTimer += Time.deltaTime;

            if (_target != null)
                LookAtTarget();

            if (_shootingTimer >= _timeUntilNextShot && _isBurstFire == false)
                ShootPlayer();

            if (_shootingTimer >= _timeUntilNextShot && _isBurstFire)
            {
                StartCoroutine(BurstFire());
            }
        }
        else
        {
            _shootingTimer = 0;
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Changes the rotation of the enemy to look at the player according to their current position.
    /// </summary>
    private void LookAtTarget()
    {
        Vector2 direction = (_target.position - this.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Resets the shooting timer and checks to see if the player is within range. Once the player has been
    /// targeted, the raycast for the enemy weapon will call AimAtPlayer() method to generate a direction
    /// for the raycast and line renderer to be drawn then have the player take damage if they are colliding
    /// with the ray. Otherwise, the pooled bullet ricochet particle will be instantiated at the point of contact.
    /// </summary>
    private void ShootPlayer()
    {
        _shootingTimer = 0;

        Collider2D player = Physics2D.OverlapCircle(transform.position, _range, _playerLayerMask);

        if (player != null)
        {
            _target = player.transform;
        }
        else
        {
            _target = null;
            return;
        }

        Vector2 shootingDirection = AimAtPlayer(player);

        RaycastHit2D target = Physics2D.Raycast(transform.position, shootingDirection, _weaponRange, _collisionLayers);

        if (target.collider != null)
        {
            if (_bulletTrail != null)
                StartCoroutine(DrawBulletTrail(target));            

            if (target.collider.CompareTag("Player"))
            {
                target.collider.GetComponent<HandlePlayerImpact>().SpawnBloodSplatterParticle(target.point, target.normal);
                target.collider.GetComponent<Health>().TakeHit(_damage);
            }
            else
            {
                SpawnBulletRicochetParticle(target.point, target.normal);
            }

            EnemyOnFire?.Invoke();
        }
    }

    /// <summary>
    /// Enables a member of the _bulletRicochetParticle pool before returning them back
    /// after a specified amount of time (in seconds).
    /// </summary>
    /// <param name="origin">Vector2 that positions the particle at this point.</param>
    /// <param name="direction">Vector2 that sets the orientation of the particle at the origin.</param>
    private void SpawnBulletRicochetParticle(Vector2 origin, Vector2 direction)
    {
        var ricochet = _bulletImpactParticle.Get<PooledMonoBehaviour>(origin, Quaternion.LookRotation(-direction));
        ricochet.ReturnToPool(1f);
    }

    /// <summary>
    /// Creates a Vector2 that is used to set the direction of the raycast in the ShootPlayer() method.
    /// </summary>
    /// <param name="player">Collider2D object that is passed from the collision detected in the
    /// overlap circle generated in the ShootPlayer() method.</param>
    /// <returns></returns>
    private Vector2 AimAtPlayer(Collider2D player)
    {
        Vector3 weaponSwayOffset = new Vector3(GetRandomValueFromWeaponSway(), GetRandomValueFromWeaponSway(), 0f);

        var aimDirection = (player.transform.position - this.transform.position +
            weaponSwayOffset).normalized;

        return aimDirection;
    }

    /// <summary>
    /// Generates a value between the + and - _weaponSway values, which are used as a range.
    /// </summary>
    /// <returns>Float value that will be used to offset the raycast direction to reduce accuracy
    /// of the weapon.</returns>
    private float GetRandomValueFromWeaponSway()
    {
        float randomlyGeneratedValue = UnityEngine.Random.Range(-_weaponSwayAmount, _weaponSwayAmount);
        return randomlyGeneratedValue;
    }

    /// <summary>
    /// Coroutine that visualizes the bullet path for the enemy weapon. First the method enables the line renderer 
    /// component of the _bulletTrail object, then sets the positions of the line renderer to match the trajectory
    /// of the raycast/bullet path, and finally disables the line renderer after a short duration.
    /// </summary>
    /// <param name="target">RaycastHit2D object passed from the ShootPlayer() method where the
    /// ray collided with some object of interest (player or environment).</param>
    /// <returns></returns>
    private IEnumerator DrawBulletTrail(RaycastHit2D target)
    {
        _bulletTrail.enabled = true;

        _bulletTrail.SetPosition(0, _firePoint.position);
        _bulletTrail.SetPosition(1, target.point);

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
        for (int i = 0; i < _shotsPerBurst; i++)
        {
            ShootPlayer();
            yield return new WaitForSeconds(_timeUntilNextBurstShot);
        }
    } 
    #endregion
}
