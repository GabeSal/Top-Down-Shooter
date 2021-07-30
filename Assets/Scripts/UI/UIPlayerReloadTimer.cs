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
        _playerAmmo = FindObjectOfType<Weapon>().GetComponent<WeaponAmmo>();

        _playerAmmo.OnReload += ReloadTimer_OnReload;
        _reloadBarFill.fillAmount = 0f;
        this.gameObject.SetActive(false);
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked method thats called from the WeaponAmmos OnReload() event.
    /// </summary>
    private void ReloadTimer_OnReload()
    {
        this.gameObject.SetActive(true);

        StartCoroutine(FillReloadBar());
    }

    /// <summary>
    /// Coroutine that scales the increments by the _playerAmmo.ReloadTime property for the fill of the reload bar image.
    /// After the image has been filled, the fill for the reload bar is reset to 0 and the Canvas for this UI is then deactivated.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillReloadBar()
    {
        do
        {
            yield return new WaitForSeconds(0.008f);
            _reloadBarFill.fillAmount += Time.deltaTime * _playerAmmo.ReloadTime;
        } while (_reloadBarFill.fillAmount < 1f);

        _reloadBarFill.fillAmount = 0;
        this.gameObject.SetActive(false);
    }
    #endregion
}
