using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 10f;

    private Vector3 _movement;

    public bool TankControlled = false;

    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        // Tank Controls
        if (TankControlled)
        {
            transform.Translate(_movement.x * _movementSpeed * Time.deltaTime, _movement.y * _movementSpeed * Time.deltaTime, 0f);
        }
        else
        {
            transform.position += _movement * _movementSpeed * Time.deltaTime;
        }
    }
}
