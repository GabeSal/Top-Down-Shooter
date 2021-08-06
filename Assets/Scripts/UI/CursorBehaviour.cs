using UnityEngine;

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
    private void Start()
    {
        LockAndSetCursorImage();
        GameManager.Instance.OnGameOver += CursorBehaviour_OnGameOver;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(_leftMouseClick))
        {
            LockAndSetCursorImage();
        }
        if (Input.GetKeyDown((KeyCode)PlayerControls.pauseGame))
        {
            UnlockAndShowMouseCursor();
        }
    }

    private void OnDestroy()
    {
        UnlockAndShowMouseCursor();
        GameManager.Instance.OnGameOver -= CursorBehaviour_OnGameOver;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Method invoked when the Game Managers OnGameOver() event is called.
    /// </summary>
    private void CursorBehaviour_OnGameOver()
    {
        UnlockAndShowMouseCursor();
    }

    /// <summary>
    /// Unrestrains the mouse cursor to the game window and shows the default mouse cursor.
    /// </summary>
    private static void UnlockAndShowMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Restrains the mouse cursor within the window and shows the crosshair texture to replace the default
    /// mouse cursor image.
    /// </summary>
    private void LockAndSetCursorImage()
    {
        Vector2 centerPivot = new Vector2(crosshair.width / 2, crosshair.height / 2);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(crosshair, centerPivot, CursorMode.Auto);
    }
    #endregion
}
