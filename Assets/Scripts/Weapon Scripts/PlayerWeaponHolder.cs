using System;
using UnityEngine;

public class PlayerWeaponHolder : MonoBehaviour
{
    #region Private Fields
    private WeaponInventory _playerWeapons;
    #endregion

    #region Action Events
    public event Action RequestWeapon;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _playerWeapons = GameManager.Instance.GetComponentInChildren<WeaponInventory>();

        if (_playerWeapons.WeaponsInInventory == null)
            Debug.LogError("There is no weapon inventory object found in this scene!");
        else if (_playerWeapons.WeaponsInInventory[0] != null)
            RequestFirstWeaponFromPlayerInventory();

    }
    #endregion

    #region Class Defined Methods
    private void RequestFirstWeaponFromPlayerInventory()
    {
        RequestWeapon?.Invoke();
        _playerWeapons.transform.GetChild(0).parent = this.transform;
    }
    #endregion
}
