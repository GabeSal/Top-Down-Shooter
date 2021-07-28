using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(EnemySelfDestruct))]
public class FullStopWhenReachedDestination : MonoBehaviour
{
    #region Private Fields
    private AIPath _aiPath;
    private EnemySelfDestruct _enemySelfDestruct;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _enemySelfDestruct = GetComponent<EnemySelfDestruct>();
    }

    private void Update()
    {
        if (_aiPath.reachedDestination == true)
        {
            _aiPath.canMove = false;
            _aiPath.enabled = false;

            _enemySelfDestruct.BeginCountdown();

            this.enabled = false;
        }
    } 
    #endregion
}
