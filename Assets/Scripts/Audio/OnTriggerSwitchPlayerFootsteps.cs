using System;
using UnityEngine;

public class OnTriggerSwitchPlayerFootsteps : MonoBehaviour
{
    #region Private Fields

    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _newFootstepEvent;
    #endregion

    private SimpleAudioEvent _previousFootstepEvent;
    private Collider2D _collider2D;
    private Transform _player;
    #endregion

    #region Action Events
    public event Action<SimpleAudioEvent> SwapPlayerFootsteps;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _player = FindObjectOfType<PlayerMovement>().transform;

        _player.GetComponent<FootstepSounds>().StoreOldFootsteps += ReceivePreviousFootstepEvent;
    }

    private void OnDestroy()
    {
        _player.GetComponent<FootstepSounds>().StoreOldFootsteps -= ReceivePreviousFootstepEvent;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SwapPlayerFootsteps?.Invoke(_newFootstepEvent);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_previousFootstepEvent != null)
            SwapPlayerFootsteps?.Invoke(_previousFootstepEvent);
    }
    #endregion

    #region Received Event Methods
    private void ReceivePreviousFootstepEvent(SimpleAudioEvent previousPlayerAudioEvent)
    {
        _previousFootstepEvent = previousPlayerAudioEvent;
    }
    #endregion
}
