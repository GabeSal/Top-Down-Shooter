using System;
using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyShootingHandler : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Range(5.5f, 16f)]
    [Tooltip("Determines the range in which the enemy is allowed to fire their weapon.")]
    private float _fireRange;
    [SerializeField]
    [Tooltip("Used to specify the origin of which to draw the raycast for the weapon.")]
    private Transform _firePoint;
    #endregion

    #region Private Fields
    private float _shootingTimer = 0;
    private AIPath _aiPath;
    private LayerMask _playerLayerMask;
    private Transform _target;
    private EnemyWeapon _enemyWeapon;
    #endregion

    #region Action Events
    public event Action EnemyOnFire;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _playerLayerMask = LayerMask.GetMask("Player");
        _enemyWeapon = GetComponentInChildren<EnemyWeapon>();
    }

    private void Update()
    {
        if (_aiPath.reachedDestination || _enemyWeapon.canShootAndRun)
        {
            _shootingTimer += Time.deltaTime;

            if (_target != null)
                LookAtTarget();

            if (_shootingTimer >= _enemyWeapon.TimeUntilNextShot && !_enemyWeapon.isBurstFire)
                ShootPlayer();

            if (_shootingTimer >= _enemyWeapon.TimeUntilNextShot && _enemyWeapon.isBurstFire)
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

        Collider2D player = Physics2D.OverlapCircle(transform.position, _fireRange, _playerLayerMask);

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

        RaycastHit2D target = Physics2D.Raycast(transform.position, shootingDirection, 
            _enemyWeapon.Range, _enemyWeapon.CollisionLayers);

        if (_enemyWeapon.bulletTrail != null)
            StartCoroutine(DrawBulletTrail(target, shootingDirection));

        if (target.collider != null)
        {
            if (target.collider.CompareTag("Player"))
            {
                target.collider.GetComponent<HandlePlayerImpact>().SpawnBloodSplatterParticle(target.point, target.normal);
                target.collider.GetComponent<Health>().TakeHit(_enemyWeapon.Damage);
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
        var ricochet = _enemyWeapon.ImpactParticle.Get<PooledMonoBehaviour>(origin, Quaternion.LookRotation(-direction));
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
        float randomlyGeneratedValue = UnityEngine.Random.Range(-_enemyWeapon.weaponSwayAmount, _enemyWeapon.weaponSwayAmount);
        return randomlyGeneratedValue;
    }

    /// <summary>
    /// Coroutine that visualizes the bullet path for the enemy weapon. First the method enables the line renderer 
    /// component of the _bulletTrail object, then sets the positions of the line renderer to match the trajectory
    /// of the raycast/bullet path, and finally disables the line renderer after a short duration.
    /// </summary>
    /// <param name="target">RaycastHit2D object passed from the ShootPlayer() method where the
    /// ray collided with some object of interest (player or environment).</param>
    /// /// <param name="shotDirection">Vector3 that is passed from the shootingDirection variable from the
    /// ShootPlayer() method.</param>
    /// <returns></returns>
    private IEnumerator DrawBulletTrail(RaycastHit2D target, Vector3 shotDirection)
    {
        _enemyWeapon.bulletTrail.enabled = true;

        _enemyWeapon.bulletTrail.SetPosition(0, _firePoint.position);
        if (target.point != Vector2.zero)
        {
            _enemyWeapon.bulletTrail.SetPosition(1, target.point);
        }
        else
        {
            _enemyWeapon.bulletTrail.SetPosition(1, _firePoint.position + (shotDirection * _enemyWeapon.Range));
        }

        yield return new WaitForSeconds(0.04f);

        _enemyWeapon.bulletTrail.enabled = false;
    }

    /// <summary>
    /// Coroutine that begins a for loop that fires a round and delays the next shot in the burst fire 
    /// until the amount of rounds fired reaches the _shotsPerBurst value.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BurstFire()
    {
        for (int i = 0; i < _enemyWeapon.shotsPerBurst; i++)
        {
            ShootPlayer();
            yield return new WaitForSeconds(_enemyWeapon.timeUntilNextBurstShot);
        }
    }
    #endregion
}
