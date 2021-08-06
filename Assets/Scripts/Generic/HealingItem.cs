using System;
using UnityEngine;

public class HealingItem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private int _healingAmount;
    #endregion

    #region Private Fields
    private bool _isTouchingPlayer;
    private Collider2D _player;
    #endregion

    #region Properties
    public int HealingAmount { get => _healingAmount; }
    #endregion

    #region Action Events
    public event Action OnHealingItemPickup;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (_isTouchingPlayer && PlayerInteracted())
        {
            HealPlayer();
        }
    }

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
    /// Gets the Health component from the player and calls its Heal() method to pass the healing amount to be
    /// recovered. OnHealingItemPickup is also invoked to call the necessary events (audio, animation, etc.).
    /// </summary>
    private void HealPlayer()
    {
        _isTouchingPlayer = false;
        _player.GetComponent<Health>().Heal(_healingAmount);
        OnHealingItemPickup?.Invoke();

        HideSprite();
    }

    /// <summary>
    /// Accesses the child game object which holds the sprite component and disables the game object to
    /// hide the sprite.
    /// </summary>
    private void HideSprite()
    {
        var sprite = this.transform.GetChild(0).gameObject;

        sprite.SetActive(false);
    }

    /// <summary>
    /// Checks to see if the player pressed the interaction keys.
    /// </summary>
    /// <returns>True if player pressed the specified interaction keys. (See the PlayerControls enum)</returns>
    private bool PlayerInteracted()
    {
        return Input.GetKeyDown((KeyCode)PlayerControls.interact) || Input.GetKeyDown((KeyCode)PlayerControls.interact2);
    }
    #endregion
}
