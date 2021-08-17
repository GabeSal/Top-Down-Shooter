using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _ammoInClipText;
    [SerializeField]
    private TextMeshProUGUI _ammoInReserveText;
    #endregion

    #region Private Fields
    private WeaponAmmo _currentWeaponAmmo;
    private WeaponInventory _weaponInventory;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _weaponInventory = GameManager.Instance.GetComponentInChildren<WeaponInventory>();
        _weaponInventory.OnWeaponChanged += AmmoTextUI_OnWeaponChanged;
        _weaponInventory.FirstWeaponFound += AmmoTextUI_FirstWeaponFound;

        SetAmmoTextToDefaultText();
    }

    private void OnDestroy()
    {
        UnsubscribeToWeaponAmmoEvents();
        _weaponInventory.OnWeaponChanged -= AmmoTextUI_OnWeaponChanged;
        _weaponInventory.FirstWeaponFound -= AmmoTextUI_FirstWeaponFound;

        _ammoInClipText = null;
        _ammoInReserveText = null;
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Uses a foreach loop to find the active transform object in the players weapon inventory object. Once
    /// an active weapon is found, we subscribe to it's weapon ammo events.
    /// </summary>
    private void FindEquippedPlayerWeaponInScene()
    {
        var equippedWeapon = _weaponInventory.GetCurrentlyEquippedWeaponGameObject();
        if (equippedWeapon != null)
        {
            _currentWeaponAmmo = equippedWeapon.GetComponent<WeaponAmmo>();
            SubscribeToWeaponAmmoEvents();
            SetCurrentWeaponAmmo();
        }
        else
        {
            SetAmmoTextToDefaultText();
        }
    }

    /// <summary>
    /// Subscribes to all of the necessary _currentWeaponAmmo events to make the appropriate UI updates.
    /// </summary>
    private void SubscribeToWeaponAmmoEvents()
    {
        if (_currentWeaponAmmo != null)
        {
            _currentWeaponAmmo.OnAmmoChanged += SetCurrentWeaponAmmo;
            _currentWeaponAmmo.OnReload += HideAmmoTextOnReload;
            _currentWeaponAmmo.OnReloadFinish += ShowAmmoTextOnReloadFinish;
            _currentWeaponAmmo.OnReloadCancel += ShowAmmoTextOnReloadCancel;
            _currentWeaponAmmo.OnManualReload += HideAmmoTextOnManualReload;
            _currentWeaponAmmo.OnManualReloadFinish += ShowAmmoTextOnManualReloadFinish;
        }        
    }

    /// <summary>
    /// Unsubscribes to all of the necessary _currentWeaponAmmo events.
    /// </summary>
    private void UnsubscribeToWeaponAmmoEvents()
    {
        if (_currentWeaponAmmo != null)
        {
            _currentWeaponAmmo.OnAmmoChanged -= SetCurrentWeaponAmmo;
            _currentWeaponAmmo.OnReload -= HideAmmoTextOnReload;
            _currentWeaponAmmo.OnReloadFinish -= ShowAmmoTextOnReloadFinish;
            _currentWeaponAmmo.OnReloadCancel -= ShowAmmoTextOnReloadCancel;
            _currentWeaponAmmo.OnManualReload -= HideAmmoTextOnManualReload;
            _currentWeaponAmmo.OnManualReloadFinish -= ShowAmmoTextOnManualReloadFinish;
        }        
    }

    /// <summary>
    /// Activates the ammo text objects in the UI Canvas.
    /// </summary>
    private void ShowAmmoText()
    {
        _ammoInClipText.gameObject.SetActive(true);
        _ammoInReserveText.gameObject.SetActive(true);
    }
    /// <summary>
    /// Disables the ammo text objects in the UI Canvas.
    /// </summary>
    private void HideAmmoText()
    {
        _ammoInClipText.gameObject.SetActive(false);
        _ammoInReserveText.gameObject.SetActive(false);
    }

    private void SetAmmoTextToDefaultText()
    {
        _ammoInClipText.text = "0 /";
        _ammoInReserveText.text = "0";
    }

    /// <summary>
    /// Invoked when the OnWeaponChanged in the WeaponInventory object is called.
    /// Unsubscribes to the currently referenced _currentWeaponAmmo and finds the newly equipped
    /// weapon in the player's weapon holder transform and subscribes to its events.
    /// </summary>
    private void AmmoTextUI_OnWeaponChanged()
    {
        UnsubscribeToWeaponAmmoEvents();

        FindEquippedPlayerWeaponInScene();
        ShowAmmoText();
    }

    private void AmmoTextUI_FirstWeaponFound()
    {
        FindEquippedPlayerWeaponInScene();
    }

    /// <summary>
    /// Invoked when the OnAmmoChanged() event is called. Sets the _ammoInClipText and _ammoRemainingText
    /// text components to their appropriate values when the player weapon is fired and reloaded.
    /// </summary>
    private void SetCurrentWeaponAmmo()
    {
        if (_currentWeaponAmmo != null)
        {
            _ammoInClipText.text = _currentWeaponAmmo.GetCurrentClipAmmoText();
            _ammoInReserveText.text = _currentWeaponAmmo.GetNewReserveAmmoText();
        }        
    }
    /// <summary>
    /// Method called when OnReload() event is invoked.
    /// </summary>
    private void HideAmmoTextOnReload()
    {
        HideAmmoText();
    }
    /// <summary>
    /// Method called when OnReloadFinish() event is invoked.
    /// </summary>
    private void ShowAmmoTextOnReloadFinish()
    {
        ShowAmmoText();
    }
    /// <summary>
    /// Method called when OnManualReload() event is invoked.
    /// </summary>
    private void HideAmmoTextOnManualReload()
    {
        HideAmmoText();
    }
    /// <summary>
    /// Method called when OnReloadCancel() event is invoked.
    /// </summary>
    private void ShowAmmoTextOnReloadCancel()
    {
        ShowAmmoText();
    }
    /// <summary>
    /// Method called when OnManualReloadFinish() event is invoked.
    /// </summary>
    private void ShowAmmoTextOnManualReloadFinish()
    {
        ShowAmmoText();
    }
    #endregion
}
