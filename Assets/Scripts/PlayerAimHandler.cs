using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimHandler : MonoBehaviour
{
    [SerializeField]
    private float _angleOffset = -90f;

    private void Update()
    {
        float angle = ConvertMousePositionToLookAngle();
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Converts the mouse cursors position from the screen view into a world point minus the transforms current position.
    /// </summary>
    /// <returns>angle is the float value that represents the angle between the mouse and gameobject</returns>
    private float ConvertMousePositionToLookAngle()
    {
        Vector3 playerPositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - playerPositionOnScreen;

        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + _angleOffset;

        return angle;
    }
}
