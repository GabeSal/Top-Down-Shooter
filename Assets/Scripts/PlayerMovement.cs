using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 10f;

    private Vector3 _movement;

    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        transform.position += _movement * _movementSpeed * Time.deltaTime;
    }
}
