using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyStatus : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        GetComponent<Health>().OnDied += EnemyStatus_OnDied;
    }
    private void OnDisable()
    {
        GetComponent<Health>().OnDied -= EnemyStatus_OnDied;
    }
    private void OnDestroy()
    {
        GetComponent<Health>().OnDied -= EnemyStatus_OnDied;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked when the OnDied() event is called. Set the active state of the enemy in the scene to false.
    /// </summary>
    private void EnemyStatus_OnDied()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Method that activates a member of the _bloodSplatterParticle pool at a specified origin and orientation
    /// then deactivating the member after a specified amount of time (in seconds).
    /// </summary>
    /// <param name="origin">Vector2 passed from the player weapons FireWeapon() method using the hitInfo2D.point 
    /// property.</param>
    /// <param name="direction">Vector2 passed from the player weapons FireWeapon() method using the hitInfo2D.normal 
    /// property.</param>
    internal void SpawnBloodSplatterParticle(Vector2 origin, Vector2 direction)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(origin, Quaternion.LookRotation(direction));
        bloodSplatter.ReturnToPool(1f);
    } 
    #endregion
}
