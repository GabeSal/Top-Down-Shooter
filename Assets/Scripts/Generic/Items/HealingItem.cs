using System;
using UnityEngine;

public class HealingItem : Item
{
    #region Serialized Fields
    [SerializeField]
    private int _healingAmount;
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
    #endregion
}
