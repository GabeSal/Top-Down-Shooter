using System;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private string _sceneToTransitionTo;
    #endregion

    #region Private Fields
    private bool _collidingWithPlayer;
    #endregion

    #region Action Events
    public event Action<string> OnEndPointInteraction;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (_collidingWithPlayer && PlayerInteracted())
        {
            OnEndPointInteraction?.Invoke(_sceneToTransitionTo);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            _collidingWithPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _collidingWithPlayer = false;
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
