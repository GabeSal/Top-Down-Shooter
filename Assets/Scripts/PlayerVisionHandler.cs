using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerVisionHandler : MonoBehaviour
{
    [SerializeField]
    private Light2D _visionConeDefault;
    [SerializeField]
    private Light2D _visionConeAimed;

    private float _angleOffset = 90f;
    private int _mouseRightClick = 1;

    private bool _isAiming { get { return Input.GetMouseButtonDown(_mouseRightClick) == true; } }

    private void Update()
    {
        float angle = ConvertMousePositionToLookAngle();
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Input.GetMouseButtonDown(_mouseRightClick))
        {
            SwitchVisionCone();
        }

        if (Input.GetMouseButtonUp(_mouseRightClick))
        {
            SwitchVisionCone();
        }
    }

    private void SwitchVisionCone()
    {
        if (_isAiming)
        {
            _visionConeDefault.gameObject.SetActive(false);
            _visionConeAimed.gameObject.SetActive(true);
        }
        else
        {
            _visionConeDefault.gameObject.SetActive(true);
            _visionConeAimed.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Converts the mouse cursors position from the screen view into a world point minus the transforms current position.
    /// </summary>
    /// <returns>angle is the float value that represents the angle between the mouse and gameobject</returns>
    private float ConvertMousePositionToLookAngle()
    {
        Vector3 playerPositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - playerPositionOnScreen;

        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - _angleOffset;

        return angle;
    }
}
