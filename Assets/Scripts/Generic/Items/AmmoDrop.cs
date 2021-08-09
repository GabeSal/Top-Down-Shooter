using System;
using UnityEngine;

public class AmmoDrop : Item
{
    #region Serialized Fields
    [SerializeField]
    private int _ammoAmountInDrop;
    #endregion

    #region Action Events
    public event Action OnAmmoPickup;
    public event Action OnMaxAmmoReached;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (_isTouchingPlayer && PlayerInteracted())
            GivePlayerAmmo();
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Iterates through the weapons in the players weapon inventory to check which weapon is currently 
    /// equipped by the player (active in scene). When the weapon is found, we access their WeaponAmmo
    /// component to call the AddAmmo() method and increase the ammo in reserve for the weapon.
    /// </summary>
    private void GivePlayerAmmo()
    {
        _isTouchingPlayer = false;
        var playerWeapons = _player.GetComponentInChildren<WeaponInventory>().WeaponsInInventory;

        foreach (var weapon in playerWeapons)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                var currentWeaponAmmo = weapon.GetComponent<WeaponAmmo>();
                if (currentWeaponAmmo.AmmoInReserve < currentWeaponAmmo.TrueMaxAmmo)
                {
                    currentWeaponAmmo.AddAmmo(_ammoAmountInDrop);
                    OnAmmoPickup?.Invoke();
                    break;
                }
                else
                {
                    OnMaxAmmoReached?.Invoke();
                }                
            }
        }        

        HideSprite();
    }
    #endregion
}
