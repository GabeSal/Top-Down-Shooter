using System;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private string _sceneToTransitionTo;
    #endregion

    #region Private Fields
    private bool _isTouchingPlayer;
    #endregion

    #region Action Events
    public event Action OnEndPointCollision;
    public event Action OnLeavingEndPointCollision;
    public event Action<string> OnEndPointLevelTransition;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (_isTouchingPlayer && PlayerInteracted())
        {
            OnEndPointLevelTransition?.Invoke(_sceneToTransitionTo);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isTouchingPlayer = true;
        OnEndPointCollision?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isTouchingPlayer = false;
        OnLeavingEndPointCollision?.Invoke();
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Checks to see if the player pressed one of the interaction keys.
    /// </summary>
    /// <returns>True if the player had pressed either of the interaction keys.</returns>
    private static bool PlayerInteracted()
    {
        return Input.GetKeyDown((KeyCode)PlayerControls.interact) || Input.GetKeyDown((KeyCode)PlayerControls.interact2);
    }
    #endregion
}
