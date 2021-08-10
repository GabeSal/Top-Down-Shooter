using System;
using UnityEngine;
using Pathfinding;

public class EnemyMovementSoundHandler : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Range(0.3f, 0.75f)]
    private float _stepDelayTimer;
    #endregion

    #region Private Fields
    private float _stepTimer = 1f;
    private AIPath _aiPath;
    #endregion

    #region Action Events
    public event Action OnEnemyStep;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();

        GetComponent<Health>().OnDied += EnemyMovement_OnDied;
    }

    private void LateUpdate()
    {
        _stepTimer += Time.deltaTime;

        if (_stepTimer >= _stepDelayTimer && _aiPath.isStopped == false && 
            (!_aiPath.reachedEndOfPath || !_aiPath.reachedDestination))
        {
            _stepTimer = 0;
            OnEnemyStep?.Invoke();
        }
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnDied -= EnemyMovement_OnDied;
    }

    private void OnDestroy()
    {
        GetComponent<Health>().OnDied -= EnemyMovement_OnDied;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Method that disables this script when the enemy receives their health components' OnDied() event.
    /// </summary>
    private void EnemyMovement_OnDied()
    {
        this.enabled = false;
    }
    #endregion
}
