using System;
using UnityEngine;

public class AmmoDrop : Item
{
    #region Serialized Fields
    [SerializeField]
    private int _ammoAmount;
    #endregion

    #region Properties
    public int AmmoAmount { get => _ammoAmount; }
    public Transform WeaponToAddAmmo { get; private set; }
    #endregion

    #region Action Events
    public event Action OnAmmoPickup;
    public event Action OnMaxAmmoReached;
    public event Action OnLeavingAmmoPickup;
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (_isTouchingPlayer)
            GivePlayerAmmo();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnLeavingAmmoPickup?.Invoke();
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
                WeaponToAddAmmo = weapon;
                var currentWeaponAmmo = weapon.GetComponent<WeaponAmmo>();
                if (currentWeaponAmmo.AmmoInReserve >= currentWeaponAmmo.TrueMaxAmmo)
                {
                    OnMaxAmmoReached?.Invoke();
                    break;
                }
                else
                {
                    int ammoReserveDifferenceFromTrueMax = currentWeaponAmmo.TrueMaxAmmo - currentWeaponAmmo.AmmoInReserve;
                    // Changes the value of the _ammoAmount to ensure we don't go over the max ammo for the weapon.
                    if (_ammoAmount > ammoReserveDifferenceFromTrueMax)
                        _ammoAmount = ammoReserveDifferenceFromTrueMax;

                    currentWeaponAmmo.AddAmmo(_ammoAmount);
                    OnAmmoPickup?.Invoke();
                    HideSprite();
                    break;
                }
            }
        }
    }
    #endregion
}
