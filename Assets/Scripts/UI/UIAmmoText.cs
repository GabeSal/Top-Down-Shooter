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
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _currentWeaponAmmo = FindObjectOfType<BallisticWeapon>().GetComponent<WeaponAmmo>();

        _currentWeaponAmmo.OnAmmoChanged += CurrentWeaponAmmo_OnAmmoChanged;
        _currentWeaponAmmo.OnReload += CurrentWeaponAmmo_OnReload;
        _currentWeaponAmmo.OnReloadCancel += CurrentWeaponAmmo_OnReloadCancel;
        _currentWeaponAmmo.OnManualReload += CurrentWeaponAmmo_OnManualReload;
        _currentWeaponAmmo.OnManualReloadFinish += CurrentWeaponAmmo_OnManualReloadFinish;
    }

    private void OnDisable()
    {
        _currentWeaponAmmo.OnAmmoChanged -= CurrentWeaponAmmo_OnAmmoChanged;
        _currentWeaponAmmo.OnReload -= CurrentWeaponAmmo_OnReload;
        _currentWeaponAmmo.OnReloadCancel -= CurrentWeaponAmmo_OnReloadCancel;
        _currentWeaponAmmo.OnManualReload -= CurrentWeaponAmmo_OnManualReload;
        _currentWeaponAmmo.OnManualReloadFinish -= CurrentWeaponAmmo_OnManualReloadFinish;
    }

    private void OnDestroy()
    {
        _currentWeaponAmmo.OnAmmoChanged -= CurrentWeaponAmmo_OnAmmoChanged;
        _currentWeaponAmmo.OnReload -= CurrentWeaponAmmo_OnReload;
        _currentWeaponAmmo.OnReloadCancel -= CurrentWeaponAmmo_OnReloadCancel;
        _currentWeaponAmmo.OnManualReload -= CurrentWeaponAmmo_OnManualReload;
        _currentWeaponAmmo.OnManualReloadFinish -= CurrentWeaponAmmo_OnManualReloadFinish;
    }
    #endregion

    #region Class Defined Methods
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
