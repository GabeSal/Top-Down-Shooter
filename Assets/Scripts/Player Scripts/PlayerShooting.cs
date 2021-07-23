using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Transform _firePoint;
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
    #endregion
}
