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
    private WeaponInventory _weaponInventory;
    private float _reloadBarRefreshTime = 0.01f;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _weaponInventory = GameManager.Instance.GetComponentInChildren<WeaponInventory>();
        _weaponInventory.OnWeaponChanged += ReloadUI_OnWeaponChanged;

        GameManager.Instance.OnGameOver += GameManagerInstance_OnGameOver;

        FindActivePlayerWeaponInScene();
        ResetReloadUI();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= GameManagerInstance_OnGameOver;
        _weaponInventory.OnWeaponChanged -= ReloadUI_OnWeaponChanged;
        UnsubscribeToPlayerAmmoEvents();
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Uses a foreach loop to find the active transform object in the players weapon inventory object. Once
    /// an active weapon is found, we subscribe to it's weapon ammo events.
    /// </summary>
    private void FindActivePlayerWeaponInScene()
    {
        if (_playerAmmo != null)
            UnsubscribeToPlayerAmmoEvents();

        var weaponHolder = FindObjectOfType<PlayerMovement>().transform.GetChild(0);
        _playerAmmo = weaponHolder.GetComponentInChildren<WeaponAmmo>();

        if (_playerAmmo != null)
            SubscribeToPlayerAmmoEvents();
    }

    /// <summary>
    /// Subscribes to all of the necessary _playerAmmo events to make the appropriate UI updates.
    /// </summary>
    private void SubscribeToPlayerAmmoEvents()
    {
        _playerAmmo.OnReload += ReloadUI_OnReload;
        _playerAmmo.OnManualReload += ReloadUI_OnManualReload;
        _playerAmmo.OnReloadCancel += ReloadUI_OnReloadCancel;
    }

    /// <summary>
    /// Unsubscribes to all of the necessary _playerAmmo events.
    /// </summary>
    private void UnsubscribeToPlayerAmmoEvents()
    {
        _playerAmmo.OnReload -= ReloadUI_OnReload;
        _playerAmmo.OnManualReload -= ReloadUI_OnManualReload;
        _playerAmmo.OnReloadCancel -= ReloadUI_OnReloadCancel;
    }

    /// <summary>
    /// Called method when the WeaponInventory component has invoked the OnWeaponChanged() event.
    /// </summary>
    private void ReloadUI_OnWeaponChanged()
    {
        FindActivePlayerWeaponInScene();
    }

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
            yield return new WaitForSeconds(_reloadBarRefreshTime);

            if (_playerAmmo.ReloadTime > 0.6f)
                _reloadBarFill.fillAmount += 1.3f / (_playerAmmo.ReloadTime / _reloadBarRefreshTime);
            else
                _reloadBarFill.fillAmount += 1.75f / (_playerAmmo.ReloadTime / _reloadBarRefreshTime);
            
        } while (_reloadBarFill.fillAmount < 1f);

        ResetReloadUI();
    }
    #endregion
}
