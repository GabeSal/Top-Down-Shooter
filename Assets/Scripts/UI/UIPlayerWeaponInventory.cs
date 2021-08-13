using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerWeaponInventory : MonoBehaviour
{
    #region Private Fields
    private WeaponInventory _playerWeapons;
    private GameObject[] _weaponSlots;

    private float _fadeOutValue = 110f;
    private float _fullOpacityValue = 1f;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _playerWeapons = FindObjectOfType<PlayerMovement>().GetComponentInChildren<WeaponInventory>();

        if (_playerWeapons != null)
        {
            _playerWeapons.OnWeaponChanged += WeaponInventoryUI_OnWeaponChanged;
            _playerWeapons.OnWeaponInventoryUpdate += WeaponInventoryUI_OnWeaponInventoryUpdate;
        }
        else
        {
            Debug.LogError("There is no weapon inventory component found in the scene.");
        }

        OccupyWeaponSlots();
        SetPlayerWeaponImages();
    }

    private void OnDestroy()
    {
        _playerWeapons.OnWeaponChanged -= WeaponInventoryUI_OnWeaponChanged;
        _playerWeapons.OnWeaponInventoryUpdate -= WeaponInventoryUI_OnWeaponInventoryUpdate;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Specifies the _weaponSlots array size to the amount of children paired to this object, then
    /// the child objects are stored in said array.
    /// </summary>
    private void OccupyWeaponSlots()
    {
        _weaponSlots = new GameObject[this.transform.childCount];

        for (int i = 0; i < _weaponSlots.Length; i++)
        {
            _weaponSlots[i] = this.transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// Sets the image for each weapon slot in _weaponSlots by accessing the weaponUIImage public field
    /// then activating and deactivating the weapon slots accordingly by calling the FadeNonEquippedWeapons()
    /// method.
    /// </summary>
    private void SetPlayerWeaponImages()
    {
        for (int i = 0; i < _playerWeapons.WeaponsInInventory.Length; i++)
        {
            if (_playerWeapons.WeaponsInInventory[i] != null)
            {
                Image weaponImage = _weaponSlots[i].GetComponent<Image>();
                Sprite playerWeaponSprite = _playerWeapons.WeaponsInInventory[i]
                    .GetComponent<BallisticWeapon>().weaponUIImage.sprite;

                weaponImage.sprite = playerWeaponSprite;
                weaponImage.preserveAspect = true;
            }
            else
            {
                break;
            }
            
        }
        FadeNonEquippedWeapons();
    }

    /// <summary>
    /// Sets the alpha value of each weapon slot image for visual feedback on which weapon is currently
    /// equipped on the player.
    /// </summary>
    private void FadeNonEquippedWeapons()
    {
        for (int i = 0; i < _weaponSlots.Length; i++)
        {
            Image weaponImage = _weaponSlots[i].GetComponent<Image>();
            Color weaponImageAlpha = weaponImage.color;

            if (PlayerWeaponEquipped(i))
                weaponImageAlpha.a = _fullOpacityValue;
            else
                weaponImageAlpha.a = _fadeOutValue / 255f;

            if (weaponImage.sprite == null)
                weaponImageAlpha.a = 0;

            weaponImage.color = weaponImageAlpha;
        }
    }

    /// <summary>
    /// Method that checks to see if the weapon being referenced in the players WeaponInventory component
    /// is active in the scene.
    /// </summary>
    /// <returns>True if the player weapon is currently active in the scene.</returns>
    private bool PlayerWeaponEquipped(int weaponIndex)
    {
        if (weaponIndex < _playerWeapons.WeaponsInInventory.Length && _playerWeapons.WeaponsInInventory[weaponIndex] != null)
        {
            GameObject playerWeapon = _playerWeapons.WeaponsInInventory[weaponIndex].gameObject;

            if (playerWeapon.activeInHierarchy)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Invoked method that's called when the WeaponInventory calls the OnWeaponChanged() event.
    /// </summary>
    private void WeaponInventoryUI_OnWeaponChanged()
    {
        FadeNonEquippedWeapons();
    }

    private void WeaponInventoryUI_OnWeaponInventoryUpdate()
    {
        OccupyWeaponSlots();
        SetPlayerWeaponImages();
    }
    #endregion
}
