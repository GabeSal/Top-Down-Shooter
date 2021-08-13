using UnityEngine;

public class PlayerShootingUtility : MonoBehaviour
{
    #region Private Fields
    private Transform _firePoint;
    private WeaponInventory _weaponInventory;
    #endregion

    #region Properties
    public Transform FirePoint { get => _firePoint; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _weaponInventory = GetComponentInChildren<WeaponInventory>();
        if (_weaponInventory != null)
            _weaponInventory.OnWeaponInventoryUpdate += PlayerFirePoint_OnWeaponInventoryUpdate;

        FindActiveWeaponInWeaponInventory();
    }

    private void OnDestroy()
    {
        if (_weaponInventory != null)
            _weaponInventory.OnWeaponInventoryUpdate -= PlayerFirePoint_OnWeaponInventoryUpdate;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Generates a normalized Vector3 that takes the difference between the mouse position and the _firePoint position
    /// and adds the offset vector before becoming normalized.
    /// </summary>
    /// <param name="weaponSwayOffsetX">Float value that is passed from the GetRandomValueFromWeaponSway()
    /// method in the child Weapon object to produce an offset in the x component.</param>
    /// /// <param name="weaponSwayOffsetY">Float value that is passed from the GetRandomValueFromWeaponSway()
    /// method in the child Weapon object to produce an offset in the y component.</param>
    /// <returns>Vector3 that will determine the direction in which the raycast of the weapon will be fired 
    /// in the Weapon component.</returns>
    public Vector3 SetShootingDirection(float weaponSwayOffsetX, float weaponSwayOffsetY)
    {
        Vector3 offset = new Vector3(weaponSwayOffsetX, weaponSwayOffsetY, 0f);
        Vector3 shootingDirection = (GetMousePosition() - _firePoint.position + offset).normalized;

        return shootingDirection;
    }

    /// <summary>
    /// Generates a Vector3 that converts the mouse position on screen into world coordinates within the Unity Engine.
    /// </summary>
    /// <returns>Vector3 that is used in the SetShootingDirection() property.</returns>
    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        return mousePosition;
    }

    /// <summary>
    /// Loops through all of the transforms in the _weaponInventory array and checks which one is currently
    /// active in the scene. If the weapon is active, then we set the _firePoint to the firepoint object
    /// nested in the weapon transform.
    /// </summary>
    private void FindActiveWeaponInWeaponInventory()
    {
        for (int i = 0; i < _weaponInventory.transform.childCount; i++)
        {
            var weapon = _weaponInventory.transform.GetChild(i);
            if (weapon.gameObject.activeInHierarchy)
            {
                _firePoint = weapon.GetChild(0);
                break;
            }
        }
    }

    private void PlayerFirePoint_OnWeaponInventoryUpdate()
    {
        FindActiveWeaponInWeaponInventory();
    }
    #endregion
}
