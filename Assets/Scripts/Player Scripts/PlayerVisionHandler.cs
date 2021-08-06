using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerVisionHandler : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Light2D _visionConeDefault;
    [SerializeField]
    private Light2D _visionConeAimed;
    #endregion

    #region Private Fields
    private float _angleOffset = 90f;  
    private bool _isAiming { get => Input.GetKeyDown((KeyCode)PlayerControls.aim); }
    #endregion

    #region Standard Unity Methods
    private void Update()
    {
        if (GameManager.Instance.InputsAllowed && GameManager.GameIsPaused == false)
        {
            float angle = ConvertMousePositionToLookAngle();
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Input.GetKeyDown((KeyCode)PlayerControls.aim))
            {
                SwitchVisionCone();
            }

            if (Input.GetKeyUp((KeyCode)PlayerControls.aim))
            {
                SwitchVisionCone();
            }
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Toggles the active state of the _visionConeDefault and _visionConeAimed objects.
    /// </summary>
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
    /// <returns>Float value that represents the angle between the mouse and gameobject.</returns>
    private float ConvertMousePositionToLookAngle()
    {
        Vector3 playerPositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - playerPositionOnScreen;

        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - _angleOffset;

        return angle;
    }
    #endregion
}
