using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Range(4f, 10f)]
    private float _movementSpeed;
    [SerializeField]
    [Range(0.4f, 0.7f)]
    private float _walkDelayTimer;
    [SerializeField]
    [Range(0.2f, 0.35f)]
    private float _runDelayTimer;
    [SerializeField]
    private int _sprintSpeedMultiplier;
    [SerializeField]
    private KeyCode _sprintKey;
    #endregion

    #region Private Fields
    private Vector3 _movement;
    private float _stepTimer = 1f;
    #endregion

    #region Action Events
    public event Action OnPlayerStep;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (GameManager.Instance.InputsAllowed)
        {
            _stepTimer += Time.deltaTime;

            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            if (PlayerIsMoving() && !Input.GetKey(_sprintKey) && _stepTimer >= _walkDelayTimer)
                SendPlayerStepEvent();
            
            if (PlayerIsMoving() && Input.GetKey(_sprintKey) && _stepTimer >= _runDelayTimer)
                SendPlayerStepEvent();

            // Check if the sprint key is held down
            if (Input.GetKey(_sprintKey))
                    transform.position += _movement * _movementSpeed * _sprintSpeedMultiplier * Time.deltaTime;
                else
                    transform.position += _movement * _movementSpeed * Time.deltaTime;
        }
    }
    #endregion

    /// <summary>
    /// Checks to see if any of the movement keys assigned from the Unity InputManager are being held down.
    /// </summary>
    /// <returns>True if the player is moving horizontally or vertically</returns>
    private bool PlayerIsMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
    }

    /// <summary>
    /// Resets the step timer value and invokes the OnPlayerStep event to any listeners.
    /// </summary>
    private void SendPlayerStepEvent()
    {
        _stepTimer = 0;
        OnPlayerStep?.Invoke();
    }
}
