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

        _playerWeaponInventory = GameManager.Instance.GetComponentInChildren<WeaponInventory>();
        if(_playerWeaponInventory != null)
            _playerWeaponInventory.OnWeaponDropped += SwapSpriteOfDroppedWeapon;        
    }

    private void SwapSpriteOfDroppedWeapon(BallisticWeapon droppedWeapon)
    {
        if (_isTouchingPlayer)
            ChangeSpriteToTradedWeapon(droppedWeapon);
    }

    private void Update()
    {
        if (_isTouchingPlayer)
        {
            if (PlayerInteracted())
                AddWeaponToWeaponInventory();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isTouchingPlayer = true;       

        if (CheckPlayerWeaponInventorySpace() == false)
            OnWeaponInventoryFull?.Invoke(_playerWeaponInventory.GetCurrentlyEquippedWeaponGameObject());
        else
            OnWeaponInteraction?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
        if (_playerWeaponInventory.transform.childCount < 2)
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
        var weaponHolder = FindObjectOfType<PlayerWeaponHolder>().transform;
        var weaponIndex = _weapon.GetComponent<BallisticWeapon>().SlotNumber;

        _weapon.SetParent(weaponHolder, false);
        _weapon.SetSiblingIndex(weaponIndex);
        _playerWeaponInventory.AddWeapon(_weapon.transform, this.transform);

        if (this.transform.GetChild(0).GetComponent<BallisticWeapon>() != null)
            _weapon = transform.GetChild(0);
        else
            this.gameObject.SetActive(false);
    }

    private void ChangeSpriteToTradedWeapon(BallisticWeapon weaponToReplace)
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = weaponToReplace.weaponUIImage.sprite;
        return;
    }
    #endregion
}
