using UnityEngine;

public class HandlePlayerImpact : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;
    #endregion

    #region Private Fields
    private Health _playerHealth;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _playerHealth = GetComponent<Health>();
        _playerHealth.OnDied += PlayerHealth_OnDied;
    }

    private void OnDisable()
    {
        _playerHealth.OnDied -= PlayerHealth_OnDied;
    }

    private void OnDestroy()
    {
        _playerHealth.OnDied -= PlayerHealth_OnDied;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked method when OnDied is called. Sets the player objects active state to false;
    /// </summary>
    private void PlayerHealth_OnDied()
    {
        this.gameObject.SetActive(false);
    }

    #region Internal Method
    /// <summary>
    /// Method that activates a member of the _bloodSplatterParticle pool at a specified origin and orientation
    /// then deactivating the member after a specified amount of time (in seconds).
    /// </summary>
    /// <param name="origin">Vector2 passed from an enemy component method using the players position.</param>
    /// <param name="direction">Vector2 passed from the enemy component method using a generated direction vector 
    /// or raycasthit normal vector.</param>
    internal void SpawnBloodSplatterParticle(Vector2 origin, Vector2 direction)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(origin, Quaternion.LookRotation(direction));
        bloodSplatter.ReturnToPool(1f);
    }

    /// <summary>
    /// Called from the EnemyCloseQuartersAttacks component. Adds a force to the player to push them away from the 
    /// impact of the enemy melee attack.
    /// </summary>
    /// <param name="forceDirection">A normalized Vector3 calculated from the AttackPlayer() method.</param>
    internal void MeleeAttack(Vector3 forceDirection)
    {
        GetComponent<Rigidbody2D>().AddForce(forceDirection * 25f, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Called from the EnemySelfDestruct component. Adds a force to the player to push them away from the 
    /// impact of the enemy explosion.
    /// </summary>
    /// <param name="forceDirection">A normalized Vector3 calculated from the SelfDestruct() method.</param>
    internal void Explosion(Vector3 forceDirection)
    {
        GetComponent<Rigidbody2D>().AddForce(forceDirection * 100f, ForceMode2D.Impulse);
    } 
    #endregion

    #endregion
}
