using System;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Transform[] _weapons = new Transform[3];
    #endregion

    #region Action Events
    public event Action OnWeaponChanged;
    public event Action OnWeaponInventoryUpdate;
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
                if (weapon != null)
                {
                    BallisticWeapon ballisticWeapon = weapon.GetComponent<BallisticWeapon>();

                    if (ballisticWeapon != null)
                    {
                        if (Input.GetKeyDown((KeyCode)PlayerControls.weapon1) &&
                        CompareWeaponKeyCodeWithPlayerControls(PlayerControls.weapon1, ballisticWeapon.weaponHotKey))
                        {
                            SwitchToWeapon(weapon);
                            break;
                        }

                        if (Input.GetKeyDown((KeyCode)PlayerControls.weapon2) &&
                            CompareWeaponKeyCodeWithPlayerControls(PlayerControls.weapon2, ballisticWeapon.weaponHotKey))
                        {
                            SwitchToWeapon(weapon);
                            break;
                        }

                        if (Input.GetKeyDown((KeyCode)PlayerControls.weapon3) &&
                            CompareWeaponKeyCodeWithPlayerControls(PlayerControls.weapon3, ballisticWeapon.weaponHotKey))
                        {
                            SwitchToWeapon(weapon);
                            break;
                        }
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
            if (weapon != null)
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

    /// <summary>
    /// Resets the local position and rotation of the transform holding the weapon components for the player weapon
    /// being added into the inventory.
    /// </summary>
    /// <param name="weaponToAdd">Transform which holds the coordinates and rotation of the weapon game object
    /// that will be added to the _weapons array.</param>
    private static void ResetWeaponPositionAndRotation(Transform weaponToAdd)
    {
        weaponToAdd.localRotation = Quaternion.identity;
        weaponToAdd.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Adds the weaponToAdd object passed from the WeaponPickup item into the _weapons array
    /// and then equips the newly added weapon. OnWeaponInventoryUpdate will then be invoked to update the UI elements
    /// in the WeaponInventoryUI canvas group.
    /// </summary>
    /// <param name="weaponToAdd">Transform of the weapon game object that will be added to the _weapons array.</param>
    internal void AddWeapon(Transform weaponToAdd)
    {
        ResetWeaponPositionAndRotation(weaponToAdd);

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
            {
                continue;
            }
            else
            {
                _weapons[i] = weaponToAdd;
                SwitchToWeapon(_weapons[i]);
                AssignHotKeyForAddedWeapon(i);
                break;
            }
        }
        OnWeaponInventoryUpdate?.Invoke();
    }

    private void AssignHotKeyForAddedWeapon(int weaponIndex)
    {
        if (weaponIndex == 0)
            _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = (KeyCode)PlayerControls.weapon1;

        if (weaponIndex == 1)
            _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = (KeyCode)PlayerControls.weapon2;

        if (weaponIndex == 2)
            _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = (KeyCode)PlayerControls.weapon3;
    }

    internal Transform TradeWeaponInCurrentWeaponSlot(Transform weaponToAdd)
    {
        OnWeaponInventoryUpdate?.Invoke();
        return null;
    }
    #endregion
}
