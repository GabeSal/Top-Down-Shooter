using System;
using UnityEngine;

public class WeaponPickup : Item
{
    #region Private Fields
    private Transform _weapon;
    private WeaponInventory _playerWeaponInventory;
    #endregion

    #region Properties
    public Transform Weapon { get => _weapon; }
    #endregion

    #region Action Events
    public event Action OnWeaponInteraction;
    public event Action OnLeavingWeaponPickup;
    public event Action<GameObject> OnWeaponInventoryFull;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _weapon = transform.GetChild(0);
    }

    private void Update()
    {
        if (_isTouchingPlayer)
        {
            if (CheckPlayerWeaponInventorySpace())
            {
                if (PlayerInteracted())
                    AddWeaponToWeaponInventory();
            }
            else
            {
                if (PlayerInteracted())
                    TradeWeaponForCurrentlyEquippedWeapon();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision;
        _isTouchingPlayer = true;
        _playerWeaponInventory = _player.transform.GetChild(0).GetComponent<WeaponInventory>();        

        if (CheckPlayerWeaponInventorySpace() == false)
            OnWeaponInventoryFull?.Invoke(_playerWeaponInventory.GetCurrentlyEquippedWeaponGameObject());
        else
            OnWeaponInteraction?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _player = null;
        _isTouchingPlayer = false;
        OnLeavingWeaponPickup?.Invoke();
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Checks to see if the players weapon inventory game object has a children length between 1 and 3.
    /// </summary>
    /// <returns>True if there are less than 3 and greater than 0 children within the Weapon Inventory transform.</returns>
    private bool CheckPlayerWeaponInventorySpace()
    {
        var playerWeaponInventory = _player.transform.GetChild(0).GetComponent<WeaponInventory>();
        if (playerWeaponInventory.transform.childCount > 0 && playerWeaponInventory.transform.childCount < 3)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Sets the parent of the _weapon transform to the Weapon Inventory transform and assigns it into the
    /// WeaponInventory's _weapons array.
    /// </summary>
    private void AddWeaponToWeaponInventory()
    {
        _isTouchingPlayer = false;
        var playerWeaponInventory = _player.transform.GetChild(0).GetComponent<WeaponInventory>();

        _weapon.parent = playerWeaponInventory.transform;
        _weapon.SetAsLastSibling();
        playerWeaponInventory.AddWeapon(_weapon.transform);

        if (transform.GetChild(0).GetComponent<BallisticWeapon>() == null)
            this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Replaces the currently active weapon within the WeaponInventory's transform children with the
    /// _weapon transform and "switches" places with the traded weapon (ie equipped weapon is dropped
    /// while the picked up weapon replaces the weapon slot occupied by previously equipped weapon.)
    /// </summary>
    private void TradeWeaponForCurrentlyEquippedWeapon()
    {
        _isTouchingPlayer = false;
        var playerWeaponInventory = _player.transform.GetChild(0).GetComponent<WeaponInventory>();
        GameObject weaponToReplace = null;

        foreach (var weapon in playerWeaponInventory.WeaponsInInventory)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                weaponToReplace = playerWeaponInventory.GetCurrentlyEquippedWeaponGameObject();
                ChangeSpriteToTradedWeapon(weaponToReplace.transform);
                break;
            }
        }
        _weapon.parent = _playerWeaponInventory.transform;
        playerWeaponInventory.ReplaceWeaponAtIndexOf(weaponToReplace, _weapon, this.transform);
        _weapon = weaponToReplace.transform;
    }

    private void ChangeSpriteToTradedWeapon(Transform weaponToReplace)
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = weaponToReplace.GetComponent<BallisticWeapon>().weaponUIImage.sprite;
        return;
    }
    #endregion
}
