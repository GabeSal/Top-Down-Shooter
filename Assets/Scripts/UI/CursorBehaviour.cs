using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    private KeyCode _escapeKey = KeyCode.Escape;
    private int _leftMouseClick = 0;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_escapeKey))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(_leftMouseClick))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
