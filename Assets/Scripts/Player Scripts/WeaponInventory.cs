using System;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Transform[] _weapons;
    #endregion

    #region Action Events
    public event Action OnWeaponChanged;
    #endregion

    #region Properties
    public Transform[] WeaponsInInventory { get => _weapons; }
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (GameManager.Instance.InputsAllowed && GameManager.GameIsPaused == false)
        {
            foreach (var weapon in _weapons)
            {
                BallisticWeapon ballisticWeapon = weapon.GetComponent<BallisticWeapon>();

                if (ballisticWeapon != null)
                {
                    if (Input.GetKeyDown((KeyCode)PlayerControls.lightWeapon) &&
                    CompareWeaponKeyCodeWithPlayerControls(PlayerControls.lightWeapon, ballisticWeapon.WeaponHotKey))
                    {
                        SwitchToWeapon(weapon);
                        break;
                    }

                    if (Input.GetKeyDown((KeyCode)PlayerControls.mediumWeapon) &&
                        CompareWeaponKeyCodeWithPlayerControls(PlayerControls.mediumWeapon, ballisticWeapon.WeaponHotKey))
                    {
                        SwitchToWeapon(weapon);
                        break;
                    }

                    if (Input.GetKeyDown((KeyCode)PlayerControls.heavyWeapon) &&
                        CompareWeaponKeyCodeWithPlayerControls(PlayerControls.heavyWeapon, ballisticWeapon.WeaponHotKey))
                    {
                        SwitchToWeapon(weapon);
                        break;
                    }
                }
            }
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Loops through the _weapons array and searches for the weapon that matches the weaponToSwitchTo object
    /// in order to activate it and hide the other weapons.
    /// </summary>
    /// <param name="weaponToSwitchTo">Transform of the weapon object we wish to switch to.</param>
    private void SwitchToWeapon(Transform weaponToSwitchTo)
    {
        foreach (var weapon in _weapons)
        {
            weapon.gameObject.SetActive(weapon == weaponToSwitchTo);
            OnWeaponChanged?.Invoke();
        }
    }

    /// <summary>
    /// Compares the KeyCode values of the player input from the PlayerControls enum and the weapons
    /// KeyCode "hotkey" value.
    /// </summary>
    /// <param name="input">PlayerControl enum value that represents the player input.</param>
    /// <param name="weaponHotKey">KeyCode value that is passed from the WeaponHotKey property of the weapon object.</param>
    /// <returns>True if the PlayerControls KeyCode value and weaponHotKey value match.</returns>
    private static bool CompareWeaponKeyCodeWithPlayerControls(PlayerControls input, KeyCode weaponHotKey)
    {
        return (KeyCode)input == weaponHotKey;
    }
    #endregion
}
