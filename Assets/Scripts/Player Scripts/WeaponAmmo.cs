using System;
using System.Collections;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Range(1, 100)]
    private int _clipSize;
    [SerializeField]
    [Range(20, 999)]
    private int _maxAmmo;
    [SerializeField]
    private bool _infiniteAmmo;
    [SerializeField]
    [Range(0.25f, 2.2f)]
    private float _reloadTime;
    #endregion

    #region Private Fields
    private int _ammoInClip;
    private int _ammoInReserve;
    private BallisticWeapon _ballisticWeapon;
    private bool _isReloading;
    #endregion

    #region Properties
    public float ReloadTime { get => _reloadTime; }
    public int AmmoInClip { get => _ammoInClip; }
    #endregion

    #region Action Events
    public event Action OnAmmoChanged;
    public event Action OnReload;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _isReloading = false;
        _ammoInClip = _clipSize;
        _ammoInReserve = _maxAmmo - _ammoInClip;

        _ballisticWeapon = GetComponent<BallisticWeapon>();
        _ballisticWeapon.OnFire += Weapon_OnFire;
    }

    private void Start()
    {
        // Change the initial text on startup
        OnAmmoChanged?.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.InputsAllowed)
        {
            if (Input.GetKeyDown(KeyCode.R) && HasEnoughAmmo() && !_isReloading)
            {
                _isReloading = true;
                StartCoroutine(Reload());
            }
        }        
    }

    private bool HasEnoughAmmo()
    {
        return _ammoInReserve > 0 && _ammoInClip < _clipSize;
    }

    private void OnDisable()
    {
        _ballisticWeapon.OnFire -= Weapon_OnFire;
    }

    private void OnDestroy()
    {
        _ballisticWeapon.OnFire -= Weapon_OnFire;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Response method when the OnFire() event is invoked. Calls the RemoveAmmot() method.
    /// </summary>
    private void Weapon_OnFire()
    {
        RemoveAmmo();
    }

    /// <summary>
    /// Method that decrements the _ammoInClip value and invokes the OnAmmoChanged event.
    /// </summary>
    private void RemoveAmmo()
    {
        _ammoInClip--;
        OnAmmoChanged?.Invoke();
    }

    /// <summary>
    /// Coroutine that stores the difference in _clipSize and _ammoInClip to replenish the spent the 
    /// ammo back into the weapon clip. If _infiniteAmmo is true however, we just set the ammo to replenish
    /// to the specified _clipSize value and invoke the OnAmmoChanged() event for UI updates.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reload()
    {
        int ammoMissingFromClip = _clipSize - _ammoInClip;
        int ammoToReload = Math.Min(ammoMissingFromClip, _ammoInReserve);

        OnReload?.Invoke();

        if (_infiniteAmmo)
            ammoToReload = _clipSize;

        if (ammoToReload > 0)
        {
            yield return new WaitForSeconds(_reloadTime);

            _ammoInClip += ammoToReload;
            _ammoInReserve -= ammoToReload;
            OnAmmoChanged?.Invoke();
            _isReloading = false;
        }
    }

    #region Public Methods
    public bool HasAmmo()
    {
        return _ammoInClip > 0;
    }
    #endregion

    #region Internal Class Methods
    internal string GetCurrentAmmoText()
    {
        return string.Format("{0}/", _ammoInClip);
    }

    internal string GetNewMaxAmmoText()
    {
        if (_infiniteAmmo)
            return "999";
        else
            return string.Format("{0}", _ammoInReserve);
    }
    #endregion

    #endregion
}
