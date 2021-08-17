using System;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Transform[] _weapons = new Transform[3];
    #endregion

    #region Private Fields
    private PlayerWeaponHolder _playerWeaponHolder;
    private EndPoint _levelEndPoint;
    #endregion

    #region Action Events
    public event Action OnWeaponChanged;
    public event Action OnWeaponInventoryUpdate;
    public event Action<BallisticWeapon> OnWeaponDropped;
    #endregion

    #region Properties
    public Transform[] WeaponsInInventory { get => _weapons; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        GameManager.Instance.LoadingMainMenu += WeaponInventory_OnMainMenuLoaded;
        GameManager.Instance.LoadingPlayableScene += WeaponInventory_LoadingPlayableScene;
        GameManager.Instance.RestartingLevel += WeaponInventory_RestartingLevel;
    }

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
    /// Sets the equipped weapon's parent transform to the WeaponInventory object and then loops
    /// through the _weapons array and searches for the weapon that matches the weaponToSwitchTo object
    /// in order to activate it, parent it to the _playerWeaponHolder transform, and hide the other weapons.
    /// </summary>
    /// <param name="weaponToSwitchTo">Transform of the weapon object we wish to switch to.</param>
    private void SwitchToWeapon(Transform weaponToSwitchTo)
    {
        if (_playerWeaponHolder.transform.childCount > 0)
        {
            Transform equippedWeapon = _playerWeaponHolder.transform.GetChild(0);
            int equippedWeaponsSlotNumber = equippedWeapon.GetComponent<BallisticWeapon>().SlotNumber - 1;

            equippedWeapon.parent = this.transform;
            equippedWeapon.SetSiblingIndex(equippedWeaponsSlotNumber);

            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(weapon == weaponToSwitchTo);

                    if (!weapon.gameObject.activeInHierarchy)
                    {
                        weapon.parent = this.transform;
                    }
                    else
                    {
                        weapon.parent = _playerWeaponHolder.transform;
                        ResetWeaponPositionAndRotation(weapon);
                    }
                }
            }
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
    private void ResetWeaponPositionAndRotation(Transform weaponToAdd)
    {
        weaponToAdd.localRotation = Quaternion.identity;
        weaponToAdd.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Moves the previously equipped weapon into the interacted Weapon Pickup object in the scene.
    /// </summary>
    /// <param name="newParent">Transform that will become the new parent to the weapon at the 
    /// specified weaponIndex in the _weapons array.</param>
    private void SetNewParentForReplacedWeapon(Transform newParent, int weaponIndex)
    {
        var weaponsInParent = newParent.GetComponentsInChildren<BallisticWeapon>();
        if (weaponsInParent.Length > 0) return;

        _weapons[weaponIndex].gameObject.SetActive(false);
        _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = KeyCode.None;
        _weapons[weaponIndex].transform.parent = newParent;
        _weapons[weaponIndex].transform.SetAsFirstSibling();
    }

    /// <summary>
    /// Assigns the hot key to the appropriate weapon in the specified weapon index in the _weapons array.
    /// </summary>
    /// <param name="weaponIndex">Int value that represents the index of the stored weapon in the _weapons
    /// array.</param>
    private void AssignHotKeyForAddedWeapon(int weaponIndex)
    {
        if (weaponIndex == 0)
            _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = (KeyCode)PlayerControls.weapon1;

        if (weaponIndex == 1)
            _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = (KeyCode)PlayerControls.weapon2;

        if (weaponIndex == 2)
            _weapons[weaponIndex].GetComponent<BallisticWeapon>().weaponHotKey = (KeyCode)PlayerControls.weapon3;
    }

    /// <summary>
    /// Stores all of the weapons in the player inventory into the WeaponInventory transform to avoid 
    /// destroying the instance of the weapon in the next scene.
    /// </summary>
    private void KeepPlayerInventoryForNextScene()
    {
        var equippedWeapon = _playerWeaponHolder.transform.GetChild(0);
        int equippedWeaponSlotNumber = equippedWeapon.GetComponent<BallisticWeapon>().SlotNumber;

        equippedWeapon.parent = this.transform;

        if (equippedWeaponSlotNumber == 1)
            equippedWeapon.SetAsFirstSibling();
        else if (equippedWeaponSlotNumber == 2)
            equippedWeapon.SetSiblingIndex(1);
        else if (equippedWeaponSlotNumber == 3)
            equippedWeapon.SetAsLastSibling();

        equippedWeapon.gameObject.SetActive(false);

        _weapons[0].gameObject.SetActive(true);
    }

    /// <summary>
    /// Adds the weaponToAdd object passed from the WeaponPickup item into the _weapons array
    /// and then equips the newly added weapon. OnWeaponInventoryUpdate will then be invoked to update the UI elements
    /// in the WeaponInventoryUI canvas group.
    /// </summary>
    /// <param name="weaponToAdd">Transform of the weapon game object that will be added to the _weapons array.</param>
    internal void AddWeapon(Transform weaponToAdd, Transform newParent)
    {
        int slotNumber = weaponToAdd.GetComponent<BallisticWeapon>().SlotNumber - 1;
        ResetWeaponPositionAndRotation(weaponToAdd);

        // Drop current weapon in slot number, if it already exist
        if (_weapons[slotNumber] != null)
        {
            SetNewParentForReplacedWeapon(newParent, slotNumber);
            OnWeaponDropped?.Invoke(_weapons[slotNumber].GetComponent<BallisticWeapon>());
        }

        _weapons[slotNumber] = weaponToAdd;
        SwitchToWeapon(_weapons[slotNumber]);
        AssignHotKeyForAddedWeapon(slotNumber);

        OnWeaponInventoryUpdate?.Invoke();
    }

    /// <summary>
    /// Requests the transform of the currently active player weapon in the scene.
    /// </summary>
    /// <returns>Transform object that will be removed from the _weapons array and placed into the weapon pickup object.</returns>
    internal GameObject GetCurrentlyEquippedWeaponGameObject()
    {
        GameObject weaponToReplace = null;

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i].gameObject.activeInHierarchy)
            {
                weaponToReplace = _weapons[i].gameObject;
                break;
            }
        }
        return weaponToReplace;
    }

    private void WeaponInventory_LoadingPlayableScene()
    {
        _playerWeaponHolder = FindObjectOfType<PlayerWeaponHolder>();
        _levelEndPoint = FindObjectOfType<EndPoint>();

        if (_weapons[0] != null && _weapons[0].gameObject.activeInHierarchy)
        {
            _weapons[0].parent = _playerWeaponHolder.transform;
            ResetWeaponPositionAndRotation(_weapons[0]);
        }

        OnWeaponChanged?.Invoke();

        if (_levelEndPoint != null)
            _levelEndPoint.OnEndPointLevelTransition += WeaponInventory_OnEndPointLevelTransition;
        else
            Debug.LogWarning("There is no end point to this scene. Do you need one for this current level?");
    }

    private void WeaponInventory_OnEndPointLevelTransition(string s)
    {
        if (_playerWeaponHolder != null && _playerWeaponHolder.transform.childCount > 0)
            KeepPlayerInventoryForNextScene();
        else
            Debug.LogWarning("There is no weapon holder transform for the player object! Please assign one to the prefab.");

        UnsubscribeFromEndPointEvents();
    }

    private void WeaponInventory_RestartingLevel()
    {
        if (_playerWeaponHolder != null && _playerWeaponHolder.transform.childCount > 0)
            KeepPlayerInventoryForNextScene();
        else
            Debug.LogWarning("There is no weapon holder transform for the player object! Please assign one to the prefab.");
    }

    private void UnsubscribeFromEndPointEvents()
    {
        _levelEndPoint.OnEndPointLevelTransition -= WeaponInventory_OnEndPointLevelTransition;
        _levelEndPoint = null;
    }

    private void WeaponInventory_OnMainMenuLoaded()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
            {
                _weapons[i].transform.parent = FindObjectOfType<PlayerWeaponHolder>().transform;
                _weapons[i] = null;
            }
            else
            {
                continue;
            }
        }
    }
    #endregion
}
