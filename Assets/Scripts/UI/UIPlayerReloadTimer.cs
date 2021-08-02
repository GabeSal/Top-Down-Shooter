using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerReloadTimer : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Image _reloadBarFill;
    #endregion

    #region Private Fields
    private WeaponAmmo _playerAmmo;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _playerAmmo = FindObjectOfType<BallisticWeapon>().GetComponent<WeaponAmmo>();
        _playerAmmo.OnReload += ReloadUI_OnReload;
        _playerAmmo.OnManualReload += ReloadUI_OnManualReload;
        _playerAmmo.OnReloadCancel += ReloadUI_OnReloadCancel;

        _reloadBarFill.fillAmount = 0f;
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _playerAmmo.OnReload -= ReloadUI_OnReload;
        _playerAmmo.OnManualReload -= ReloadUI_OnManualReload;
        _playerAmmo.OnReloadCancel -= ReloadUI_OnReloadCancel;
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Called method when the WeaponAmmo component has invoked the OnReload() event.
    /// </summary>
    private void ReloadUI_OnReload()
    {
        ShowReloadUI();
        StartCoroutine(FillReloadBar());
    }
    /// <summary>
    /// Called method when the WeaponAmmo component has invoked the OnManualReload() event.
    /// </summary>
    private void ReloadUI_OnManualReload()
    {
        ShowReloadUI();
        StartCoroutine(FillReloadBar());
    }
    /// <summary>
    /// Called method when the WeaponAmmo component has invoked the OnReloadCancel() event.
    /// </summary>
    private void ReloadUI_OnReloadCancel()
    {
        ResetReloadUI();
    }

    /// <summary>
    /// Activates the reload bar game object in the scene.
    /// </summary>
    private void ShowReloadUI()
    {
        this.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hides the reload bar game object in the scene and sets the reloadBar fill amount back to 0.
    /// </summary>
    private void ResetReloadUI()
    {
        _reloadBarFill.fillAmount = 0;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Method that scales the fill of the reload bar image by the _playerAmmo.ReloadTime property. After the image has been
    /// filled, the fill for the reload bar is reset to 0 and the Canvas for this UI is then deactivated.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillReloadBar()
    {
        do
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _reloadBarFill.fillAmount += (1 * (_playerAmmo.ReloadTime * 50)) * Time.deltaTime;
        } while (_reloadBarFill.fillAmount < 1f);

        ResetReloadUI();
    }
    #endregion
}
