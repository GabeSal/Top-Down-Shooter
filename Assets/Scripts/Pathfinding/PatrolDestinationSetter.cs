using UnityEngine;
using Pathfinding;
using System.Collections;

public class PatrolDestinationSetter : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private bool _isPatrolling;
    [SerializeField]
    private float _delayPathTimer;
    #endregion

    #region Private Fields
    private Collider2D _target;
    private EnemyVision _vision;
    private EnemyShootingHandler _enemyShootingHandler;

    // Pathfinding fields
    private IAstarAI _ai;
    private GridGraph _grid;
    private GraphNode _randomNode;
    private AIDestinationSetter _aiDestinationSetter;
    #endregion

    #region Properties
    public bool isPatrolling { get => _isPatrolling; }
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _ai = GetComponent<IAstarAI>();
        _grid = AstarPath.active.data.gridGraph;
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();

        _vision = GetComponentInChildren<EnemyVision>();
        _vision.OnPlayerInVision += AIBehaviour_OnPlayerInVision;
        _vision.OnPlayerOutOfVision += AIBehaviour_OnPlayerOutOfVision;

        _target = _aiDestinationSetter.target.GetComponent<Collider2D>();        
        _enemyShootingHandler = GetComponent<EnemyShootingHandler>();

        if (_target == null)
            Debug.LogError("AI destination setter has no target.");

        if (_isPatrolling)
            TurnOffAggressiveBehaviour();
    }

    private void LateUpdate()
    {
        if (_isPatrolling)
        {
            _randomNode = _grid.nodes[Random.Range(0, _grid.nodes.Length)];

            if (CheckPathConditions() && !_vision.GetComponent<Collider2D>().IsTouching(_target))
            {
                if (_ai.reachedEndOfPath)
                    StartCoroutine(DelayPatrol());

                _ai.destination = (Vector3)_randomNode.position;
            }
        }
    }
    #endregion

    #region Class Defined Methods
    private GraphNode GetNearestGridGraphNode()
    {
        GraphNode node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
        return node1;
    }

    /// <summary>
    /// Checks if the random node chosen is traversible ("Walkable") for the AI, whether the AI
    /// is not currently searching for a path, if the path is possible to draw between the node
    /// nearest to the AI and the random node, and finally if the AI has reached the end of the path
    /// or is not currently following any path.
    /// </summary>
    /// <returns></returns>
    private bool CheckPathConditions()
    {
        return _randomNode.Walkable && !_ai.pathPending &&
            PathUtilities.IsPathPossible(GetNearestGridGraphNode(), _randomNode) &&
            _ai.reachedEndOfPath || !_ai.hasPath;
    }

    private void TurnOffAggressiveBehaviour()
    {
        this.enabled = true;
        _aiDestinationSetter.enabled = false;

        if (_enemyShootingHandler != null)
            _enemyShootingHandler.enabled = false;
    }

    private void TurnOnAggressiveBehaviour()
    {
        this.enabled = false;
        _aiDestinationSetter.enabled = true;

        if (_enemyShootingHandler != null)
            _enemyShootingHandler.enabled = true;
    }

    private void AIBehaviour_OnPlayerInVision()
    {
        TurnOnAggressiveBehaviour();
    }

    private void AIBehaviour_OnPlayerOutOfVision()
    {
        TurnOffAggressiveBehaviour();
    }

    private IEnumerator DelayPatrol()
    {
        _ai.isStopped = true;
        yield return new WaitForSeconds(Random.Range(2.0f, _delayPathTimer));
        _ai.isStopped = false;
    }
    #endregion
}
