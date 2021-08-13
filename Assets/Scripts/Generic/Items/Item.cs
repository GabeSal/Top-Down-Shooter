using UnityEngine;

public class Item : MonoBehaviour
{
    #region Protected Fields
    protected bool _isTouchingPlayer;
    protected Collider2D _player;
    #endregion

    #region Standard Unity Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision;
        _isTouchingPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _player = null;
        _isTouchingPlayer = false;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Checks to see if the player pressed the interaction keys.
    /// </summary>
    /// <returns>True if player pressed the specified interaction keys. (See the PlayerControls enum)</returns>
    protected bool PlayerInteracted()
    {
        return Input.GetKeyDown((KeyCode)PlayerControls.interact) || Input.GetKeyDown((KeyCode)PlayerControls.interact2);
    }

    /// <summary>
    /// Accesses the child game object which holds the sprite component and disables the game object to
    /// hide the sprite.
    /// </summary>
    protected void HideSprite()
    {
        var sprite = GetComponentInChildren<SpriteRenderer>().gameObject;

        sprite.SetActive(false);
    }
    #endregion
}
