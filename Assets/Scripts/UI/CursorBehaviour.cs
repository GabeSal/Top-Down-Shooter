using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour
{
    public Texture2D crosshair;

    private KeyCode _escapeKey = KeyCode.Escape;
    private int _leftMouseClick = 0;

    private void Awake()
    {
        Vector2 centerPivot = new Vector2(crosshair.width / 2, crosshair.height / 2);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(crosshair, centerPivot, CursorMode.Auto);
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
