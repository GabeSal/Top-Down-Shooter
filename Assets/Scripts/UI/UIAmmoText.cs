using System;
using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _ammoInClipText;
    [SerializeField]
    private TextMeshProUGUI _ammoInReserve;
    #endregion

    #region Private Fields
    private WeaponAmmo _currentWeaponAmmo;
    private WeaponInventory _weaponInventory;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _weaponInventory = FindObjectOfType<WeaponInventory>();
        _weaponInventory.OnWeaponChanged += WeaponInventory_OnWeaponChanged;

        FindActivePlayerWeaponInScene();        
    }

    private void OnDestroy()
    {
        UnsubscribeToWeaponAmmoEvents();
        _weaponInventory.OnWeaponChanged -= WeaponInventory_OnWeaponChanged;
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Uses a foreach loop to find the active transform object in the players weapon inventory object. Once
    /// an active weapon is found, we subscribe to it's weapon ammo events.
    /// </summary>
    private void FindActivePlayerWeaponInScene()
    {
        foreach (var weapon in _weaponInventory.WeaponsInInventory)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                _currentWeaponAmmo = weapon.GetComponent<WeaponAmmo>();
                break;
            }
        }

        SubscribeToWeaponAmmoEvents();
    }

    /// <summary>
    /// Subscribes to all of the necessary _currentWeaponAmmo events to make the appropriate UI updates.
    /// </summary>
    private void SubscribeToWeaponAmmoEvents()
    {
        _currentWeaponAmmo.OnAmmoChanged += CurrentWeaponAmmo_OnAmmoChanged;
        _currentWeaponAmmo.OnReload += CurrentWeaponAmmo_OnReload;
        _currentWeaponAmmo.OnReloadFinish += CurrentWeaponAmmo_OnReloadFinish;
        _currentWeaponAmmo.OnReloadCancel += CurrentWeaponAmmo_OnReloadCancel;
        _currentWeaponAmmo.OnManualReload += CurrentWeaponAmmo_OnManualReload;
        _currentWeaponAmmo.OnManualReloadFinish += CurrentWeaponAmmo_OnManualReloadFinish;
    }

    /// <summary>
    /// Unsubscribes to all of the necessary _currentWeaponAmmo events.
    /// </summary>
    private void UnsubscribeToWeaponAmmoEvents()
    {
        _currentWeaponAmmo.OnAmmoChanged -= CurrentWeaponAmmo_OnAmmoChanged;
        _currentWeaponAmmo.OnReload -= CurrentWeaponAmmo_OnReload;
        _currentWeaponAmmo.OnReloadFinish -= CurrentWeaponAmmo_OnReloadFinish;
        _currentWeaponAmmo.OnReloadCancel -= CurrentWeaponAmmo_OnReloadCancel;
        _currentWeaponAmmo.OnManualReload -= CurrentWeaponAmmo_OnManualReload;
        _currentWeaponAmmo.OnManualReloadFinish -= CurrentWeaponAmmo_OnManualReloadFinish;
    }

    /// <summary>
    /// Invoked when the OnWeaponChanged in the WeaponInventory object is called.
    /// </summary>
    private void WeaponInventory_OnWeaponChanged()
    {
        UnsubscribeToWeaponAmmoEvents();
        FindActivePlayerWeaponInScene();
    }

    /// <summary>
    /// Invoked when the OnAmmoChanged() event is called. Sets the _ammoInClipText and _ammoRemainingText
    /// text components to their appropriate values when the player weapon is fired and reloaded.
    /// </summary>
    private void CurrentWeaponAmmo_OnAmmoChanged()
    {
        _ammoInClipText.text = _currentWeaponAmmo.GetCurrentClipAmmoText();
        _ammoInReserve.text = _currentWeaponAmmo.GetNewReserveAmmoText();
    }
    /// <summary>
    /// Method called when OnReload() event is invoked.
    /// </summary>
    private void CurrentWeaponAmmo_OnReload()
    {
        HideAmmoText();
    }
    /// <summary>
    /// Method called when OnReloadFinish() event is invoked.
    /// </summary>
    private void CurrentWeaponAmmo_OnReloadFinish()
    {
        ShowAmmoText();
    }
    /// <summary>
    /// Method called when OnManualReload() event is invoked.
    /// </summary>
    private void CurrentWeaponAmmo_OnManualReload()
    {
        HideAmmoText();
    }
    /// <summary>
    /// Method called when OnReloadCancel() event is invoked.
    /// </summary>
    private void CurrentWeaponAmmo_OnReloadCancel()
    {
        ShowAmmoText();
    }
    /// <summary>
    /// Method called when OnManualReloadFinish() event is invoked.
    /// </summary>
    private void CurrentWeaponAmmo_OnManualReloadFinish()
    {
        ShowAmmoText();
    }
    /// <summary>
    /// Activates the ammo text objects in the UI Canvas.
    /// </summary>
    private void ShowAmmoText()
    {
        _ammoInClipText.gameObject.SetActive(true);
        _ammoInReserve.gameObject.SetActive(true);
    }
    /// <summary>
    /// Disables the ammo text objects in the UI Canvas.
    /// </summary>
    private void HideAmmoText()
    {
        _ammoInClipText.gameObject.SetActive(false);
        _ammoInReserve.gameObject.SetActive(false);
    }
    #endregion
}
