using UnityEngine;

public class EnemyCloseQuartersAttacks : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Range(3f, 4.5f)]
    private float _attackRange;
    [SerializeField]
    [Range(0.5f, 1.5f)]
    private float _timeUntilNextAttack = 1f;
    [SerializeField]
    [Range(1, 5)]
    private int _damage;
    #endregion

    #region Private Fields
    private float _attackTimer = 0;
    private LayerMask _layerMask;
    private Collider2D _target;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _layerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (GetPlayerDistance() <= _attackRange)
        {
            _attackTimer += Time.deltaTime;

            if (_target != null)
                LookAtTarget();

            if (_attackTimer > _timeUntilNextAttack)
            {
                AttackPlayer();
            }
        }
        else
        {
            _attackTimer = 0;
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Resets the _attackTimer and detects for player collisions around the enemy using an overlap circle.
    /// Once detected, the player will take damage and handle the impact of the attack appropriately.
    /// </summary>
    private void AttackPlayer()
    {
        _attackTimer = 0;

        Collider2D player = CheckForPlayerCollision();

        if (player != null)
        {
            player.GetComponent<Health>().TakeHit(_damage);

            var direction = -(transform.position - player.transform.position).normalized;
            player.GetComponent<HandlePlayerImpact>().SpawnBloodSplatterParticle(player.transform.position, direction);
            player.GetComponent<HandlePlayerImpact>().MeleeAttack(direction);
        }
    }

    /// <summary>
    /// Gets the distance between the enemy and player object.
    /// </summary>
    /// <returns>Float value that will be compared in the Update() function to check if the 
    /// player is within attack range.</returns>
    private float GetPlayerDistance()
    {
        Collider2D player = CheckForPlayerCollision();

        if (player != null)
            return Vector3.Distance(transform.position, player.transform.position);
        else
            return _attackRange + 1f;
    }

    /// <summary>
    /// Creates an overlap circle that has radius of _attackRange to find a player around the enemy.
    /// </summary>
    /// <returns>Collider2D object that will be used to represent the active player object in the scene.</returns>
    private Collider2D CheckForPlayerCollision()
    {
        _target = Physics2D.OverlapCircle(transform.position, _attackRange, _layerMask);
        return _target;
    }

    /// <summary>
    /// Changes the rotation of the enemy to look at the player according to their current position.
    /// </summary>
    private void LookAtTarget()
    {
        Vector2 direction = (_target.transform.position - this.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    #endregion
}
