using System;
using UnityEngine;

public class WeaponPickup : Item
{
    #region Private Fields
    private Transform _weapon;
    #endregion

    #region Properties
    public Transform Weapon { get => _weapon; }
    #endregion

    #region Action Events
    public event Action OnWeaponInteraction;
    public event Action OnLeavingWeaponPickup;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _weapon = transform.GetChild(0);
    }

    private void Update()
    {
        if (_isTouchingPlayer && PlayerInteracted())
            AddWeaponToWeaponInventory();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision;
        _isTouchingPlayer = true;
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
    private void AddWeaponToWeaponInventory()
    {
        _isTouchingPlayer = false;
        var playerWeaponInventory = _player.transform.GetChild(0).GetComponent<WeaponInventory>();

        if (playerWeaponInventory.transform.childCount < 3)
        {
            _weapon.parent = playerWeaponInventory.transform;
            _weapon.SetAsLastSibling();
            playerWeaponInventory.AddWeapon(_weapon.transform);
            this.gameObject.SetActive(false);
        }
        else
        {
            foreach (var weapon in playerWeaponInventory.WeaponsInInventory)
            {
                if (weapon.gameObject.activeInHierarchy)
                {
                    var tradedWeapon = playerWeaponInventory.TradeWeaponInCurrentWeaponSlot(_weapon.transform);
                    tradedWeapon.parent = this.transform;
                    tradedWeapon.SetAsFirstSibling();
                    ChangeSpriteToTradedWeapon(tradedWeapon);
                    break;
                }
            }
        }
    }

    private void ChangeSpriteToTradedWeapon(Transform tradedWeapon)
    {
        var sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        return;
    }
    #endregion
}
