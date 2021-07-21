using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _currentAmmoText;
    [SerializeField]
    private TextMeshProUGUI _maxAmmoText;

    private WeaponAmmo _currentWeaponAmmo;

    private void Awake()
    {
        _currentWeaponAmmo = FindObjectOfType<Weapon>().GetComponent<WeaponAmmo>();

        _currentWeaponAmmo.OnAmmoChanged += CurrentWeaponAmmo_OnAmmoChanged;
    }

    private void CurrentWeaponAmmo_OnAmmoChanged()
    {
        _currentAmmoText.text = _currentWeaponAmmo.GetCurrentAmmoText();
        _maxAmmoText.text = _currentWeaponAmmo.GetNewMaxAmmoText();
    }

    private void OnDestroy()
    {
        _currentWeaponAmmo.OnAmmoChanged -= CurrentWeaponAmmo_OnAmmoChanged;
    }
}
