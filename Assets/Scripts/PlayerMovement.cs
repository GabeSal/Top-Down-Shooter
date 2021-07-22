using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Range(4f, 10f)]
    private float _movementSpeed;
    [SerializeField]
    private int _sprintSpeedMultiplier;
    [SerializeField]
    private KeyCode _sprintKey;
    #endregion

    #region Private Fields
    private Vector3 _movement;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        // Check if the sprint key is held down
        if (Input.GetKey(_sprintKey))
            transform.position += _movement * _movementSpeed * _sprintSpeedMultiplier * Time.deltaTime;
        else
            transform.position += _movement * _movementSpeed * Time.deltaTime;
        
    } 
    #endregion
}
