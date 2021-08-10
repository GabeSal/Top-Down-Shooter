using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(EnemySelfDestruct))]
public class FullStopWhenReachedDestination : MonoBehaviour
{
    #region Private Fields
    private AIPath _aiPath;
    private AIDestinationSetter _aiDestinationSetter;
    private EnemySelfDestruct _enemySelfDestruct;
    private EnemyMovementSoundHandler _enemyMovementSoundHandler;

    private Vector3 _targetPosition;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _enemySelfDestruct = GetComponent<EnemySelfDestruct>();
    }

    private void Update()
    {
        _targetPosition = _aiDestinationSetter.target.transform.position;

        if (_aiPath.destination == _targetPosition && _aiPath.reachedDestination)
        {
            _aiDestinationSetter.enabled = false;
            _aiPath.isStopped = true;

            _enemySelfDestruct.BeginCountdown();

            this.enabled = false;
        }
    } 
    #endregion
}
