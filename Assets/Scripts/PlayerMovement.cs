using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 10f;

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        transform.position += new Vector3(horizontal, vertical, 0f) * _movementSpeed * Time.deltaTime;
    }
}
