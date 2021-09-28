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
    private int _currentWeaponIndex;
    #endregion

    #region Action Events
    public event Action OnWeaponChanged;
    public event Action OnWeaponInventoryUpdate;
    public event Action FirstWeaponFound;
    public event Action<BallisticWeapon> OnWeaponDropped;
    #endregion

    #region Properties
    public Transform[] WeaponsInInventory { get => _weapons; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        GameManager.Instance.LoadingMainMenu += WeaponInventory_OnMainMenuLoaded;
        GameManager.Instance.LoadingPlayableScene += GetPlayerWeaponHolderAndSubcribeToEndPointEvents;
        GameManager.Instance.RestartingLevel += MoveWeaponFromWeaponHolderToInventory;
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
                        #region Alpha Number Controls
                        if (Input.GetKeyDown((KeyCode)PlayerControls.weapon1) &&
                                            CompareWeaponKeyCodeWithPlayerControls(PlayerControls.weapon1, ballisticWeapon.weaponHotKey))
                        {
                            if (_weapons[_currentWeaponIndex] == weapon)
                            {
                                return;
                            }
                            else
                            {
                                SwitchToWeapon(weapon);
                                break;
                            }
                        }

                        if (Input.GetKeyDown((KeyCode)PlayerControls.weapon2) &&
                            CompareWeaponKeyCodeWithPlayerControls(PlayerControls.weapon2, ballisticWeapon.weaponHotKey))
                        {
                            if (_weapons[_currentWeaponIndex] == weapon)
                            {
                                return;
                            }
                            else
                            {
                                SwitchToWeapon(weapon);
                                break;
                            }
                        }

                        if (Input.GetKeyDown((KeyCode)PlayerControls.weapon3) &&
                            CompareWeaponKeyCodeWithPlayerControls(PlayerControls.weapon3, ballisticWeapon.weaponHotKey))
                        {
                            if (_weapons[_currentWeaponIndex] == weapon)
                            {
                                return;
                            }
                            else
                            {
                                SwitchToWeapon(weapon);
                                break;
                            }
                        }
                        #endregion

                        #region Mouse Wheel Controls
                        if (Input.GetAxisRaw("Mouse Scroll Wheel") < 0f)
                        {
                            if (_currentWeaponIndex == _weapons.Length - 1)
                            {
                                SwitchToWeapon(GetFirstWeaponInInventory());
                            }
                            else
                            {
                                if (_weapons[_currentWeaponIndex + 1] != null)
                                    SwitchToWeapon(_weapons[_currentWeaponIndex + 1]);
                                else
                                    SwitchToWeapon(GetLastWeaponInInventory());
                            }
                            break;
                        }
                        else if (Input.GetAxisRaw("Mouse Scroll Wheel") > 0f)
                        {
                            if (_currentWeaponIndex <= 0)
                            {
                                SwitchToWeapon(GetLastWeaponInInventory());
                            }
                            else
                            {
                                if (_weapons[_currentWeaponIndex - 1] != null)
                                    SwitchToWeapon(_weapons[_currentWeaponIndex - 1]);
                                else
                                    SwitchToWeapon(GetFirstWeaponInInventory());
                            }
                            break;
                        }
                        #endregion
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
        if (_playerWeaponHolder.transform.childCount > 0 && weaponToSwitchTo != null)
        {
            Transform equippedWeapon = _playerWeaponHolder.transform.GetChild(0);
            int equippedWeaponsSlotNumber = equippedWeapon.GetComponent<BallisticWeapon>().SlotNumber - 1;

            equippedWeapon.SetParent(this.transform);
            equippedWeapon.SetSiblingIndex(equippedWeaponsSlotNumber);

            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(weapon == weaponToSwitchTo);

                    if (!weapon.gameObject.activeInHierarchy)
                    {
                        weapon.SetParent(this.transform);
                    }
                    else
                    {
                        weapon.SetParent(_playerWeaponHolder.transform, false);
                        ResetWeaponPositionAndRotation(weapon);
                    }
                }
            }
            OnWeaponChanged?.Invoke();
        }        
    }

    private Transform GetFirstWeaponInInventory()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
                return _weapons[i];
        }
        return null;
    }

    private Transform GetLastWeaponInInventory()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[_weapons.Length - 1 - i] != null)
                return _weapons[_weapons.Length - 1 - i];
        }
        return null;
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
        _currentWeaponIndex = weaponToAdd.GetComponent<BallisticWeapon>().SlotNumber - 1;
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
        _weapons[weaponIndex].SetParent(newParent, false);
        _weapons[weaponIndex].SetAsFirstSibling();
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

        equippedWeapon.SetParent(this.transform);
        equippedWeapon.SetSiblingIndex(equippedWeaponSlotNumber - 1);
        equippedWeapon.gameObject.SetActive(false);

        ActivateFirstAvailableWeapon();
    }

    /// <summary>
    /// Uses a for loop to find the first available game object in the WeaponInventory to activate it
    /// in the scene hierarchy and then break out of the loop once the weapon is found.
    /// </summary>
    private void ActivateFirstAvailableWeapon()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
            {
                _weapons[i].gameObject.SetActive(true);
                break;
            }
        }
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
        GameObject weapon = null;

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null && _weapons[i].gameObject.activeInHierarchy)
            {
                weapon = _weapons[i].gameObject;
                break;
            }
        }
        return weapon;
    }

    /// <summary>
    /// Checks for each weapon in the WeaponInventory and finds the first weapon assigned in the array
    /// to be parented to the player weapon holder and equipped.
    /// </summary>
    private void GetFirstAvailableWeapon()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null && _weapons[i].gameObject.activeInHierarchy)
            {
                _weapons[i].SetParent(_playerWeaponHolder.transform, false);
                ResetWeaponPositionAndRotation(_weapons[i]);
                break;
            }
        }
        FirstWeaponFound?.Invoke();
    }

    private void UnsubscribeFromEndPointEvents()
    {
        _levelEndPoint.OnEndPointLevelTransition -= CallMoveWeaponToInventoryMethod;
        _levelEndPoint = null;
    }

    /// <summary>
    /// Method invoked from the LoadingPlayableScene event from the GameManager Instance.
    /// Assigns the _playerWeaponHolder object in the newly loaded scene and finds an EndPoint object
    /// if one has been set in the level. If there are any weapons in the WeaponInventory _weapons array,
    /// we will activate and equip the first available weapon from the array.
    /// </summary>
    private void GetPlayerWeaponHolderAndSubcribeToEndPointEvents()
    {
        _playerWeaponHolder = FindObjectOfType<PlayerWeaponHolder>();
        GetFirstAvailableWeapon();
        SubscribeToEndPointEvents();
    }

    private void SubscribeToEndPointEvents()
    {
        _levelEndPoint = FindObjectOfType<EndPoint>();

        if (_levelEndPoint != null)
            _levelEndPoint.OnEndPointLevelTransition += CallMoveWeaponToInventoryMethod;
        else
            Debug.LogWarning("There is no end point to this scene. Do you need one for this current level?");
    }

    private void CallMoveWeaponToInventoryMethod(string s)
    {
        MoveWeaponFromWeaponHolderToInventory();
    }

    /// <summary>
    /// Invoked method called from an EndPoint's OnEndPointLevelTransition event.
    /// Checks to see if there is an equipped weapon on the player object through counting
    /// its weapon holders transform children. If there is a weapon nested in the weapon holder
    /// transform, then we parent the weapon to our WeaponInventory object before loading up the
    /// next level/scene in order to save the weapon from being destroyed during the loading process.
    /// </summary>
    private void MoveWeaponFromWeaponHolderToInventory()
    {
        if (_playerWeaponHolder != null && _playerWeaponHolder.transform.childCount > 0)
            KeepPlayerInventoryForNextScene();
        else
            Debug.LogWarning("There is no weapon holder transform for the player object! Please assign one to the prefab.");

        if (_levelEndPoint != null)
            UnsubscribeFromEndPointEvents();
    }

    /// <summary>
    /// Invoked method called from the pause menu or gameover screen using the OnMainMenuLoaded event.
    /// Empties out the _weapons array when loading up the main menu and pairs other weapon
    /// transforms under the player to be destroyed during the loading process.
    /// </summary>
    private void WeaponInventory_OnMainMenuLoaded()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
            {
                _weapons[i].SetParent(null);
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
