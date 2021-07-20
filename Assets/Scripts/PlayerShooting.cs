using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private Transform _firePoint;

    public Vector3 GetMouseDirection(float weaponSwayOffset = 0)
    {
        Vector3 offset = new Vector3(weaponSwayOffset, weaponSwayOffset, 0f);

        return (GetMousePosition() - _firePoint.position + offset).normalized;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        return mousePosition;
    }
}
