using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour
{
    #region Private Fields
    private KeyCode _escapeKey = KeyCode.Escape;
    private int _leftMouseClick = 0;
    #endregion

    #region Properties
    public Texture2D crosshair;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        Vector2 centerPivot = new Vector2(crosshair.width / 2, crosshair.height / 2);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(crosshair, centerPivot, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_escapeKey) || GameManager.Instance.PlayerIsDead)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(_leftMouseClick))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    } 
    #endregion
}
