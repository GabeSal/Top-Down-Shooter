using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private TextMeshProUGUI _ammoInClipText;
    [SerializeField]
    private TextMeshProUGUI _ammoRemainingText;
    #endregion

    #region Private Fields
    private WeaponAmmo _currentWeaponAmmo;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _currentWeaponAmmo = FindObjectOfType<Weapon>().GetComponent<WeaponAmmo>();

        _currentWeaponAmmo.OnAmmoChanged += CurrentWeaponAmmo_OnAmmoChanged;
    }
    private void OnDisable()
    {
        _currentWeaponAmmo.OnAmmoChanged -= CurrentWeaponAmmo_OnAmmoChanged;
    }

    private void OnDestroy()
    {
        _currentWeaponAmmo.OnAmmoChanged -= CurrentWeaponAmmo_OnAmmoChanged;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked when the OnAmmoChanged() event is called. Sets the _ammoInClipText and _ammoRemainingText
    /// text components to their appropriate values when the player weapon is fired and reloaded.
    /// </summary>
    private void CurrentWeaponAmmo_OnAmmoChanged()
    {
        _ammoInClipText.text = _currentWeaponAmmo.GetCurrentAmmoText();
        _ammoRemainingText.text = _currentWeaponAmmo.GetNewMaxAmmoText();
    }  
    #endregion
}
