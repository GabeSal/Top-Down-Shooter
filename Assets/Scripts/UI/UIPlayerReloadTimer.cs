using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

        GameManager.Instance.OnGameOver += GameManagerInstance_OnGameOver;

        ResetReloadUI();
    }

    private void OnDestroy()
    {
        _playerAmmo.OnReload -= ReloadUI_OnReload;
        _playerAmmo.OnManualReload -= ReloadUI_OnManualReload;
        _playerAmmo.OnReloadCancel -= ReloadUI_OnReloadCancel;

        GameManager.Instance.OnGameOver -= GameManagerInstance_OnGameOver;
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

    private void GameManagerInstance_OnGameOver()
    {
        StopAllCoroutines();
        ResetReloadUI();
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
            _reloadBarFill.fillAmount += (Time.deltaTime * 5) / _playerAmmo.ReloadTime;
        } while (_reloadBarFill.fillAmount < 1f);

        ResetReloadUI();
    }
    #endregion
}
