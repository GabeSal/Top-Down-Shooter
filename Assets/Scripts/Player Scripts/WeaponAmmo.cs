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
    private bool _manualReload;
    [SerializeField]
    [Range(0.2f, 2.2f)]
    private float _reloadTime;
    #endregion

    #region Private Fields
    private int _ammoInClip;
    private int _ammoInReserve;
    private BallisticWeapon _ballisticWeapon;
    private bool _isReloading;
    #endregion

    #region Properties
    public int AmmoInClip { get => _ammoInClip; }
    public float ReloadTime { get => _reloadTime; }
    #endregion

    #region Action Events
    public event Action OnAmmoChanged;
    public event Action OnReload;
    public event Action OnReloadCancel;
    public event Action OnManualReload;
    public event Action OnManualReloadFinish;
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
                StartCoroutine(Reload());
            }

            // Cancel manual reload if there is enough ammo in clip
            if (Input.GetButtonDown("Fire1") && _isReloading)
            {
                _isReloading = false;
                OnReloadCancel?.Invoke();
            }
        }        
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
    /// Method that checks if there is less ammo in the current weapon clip and if there is enough ammo to take from reserve.
    /// </summary>
    /// <returns>True if _ammoInReserve is not 0 and if we spent some ammo in the current clip.</returns>
    private bool HasEnoughAmmo()
    {
        return _ammoInReserve > 0 && _ammoInClip < _clipSize;
    }

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
    /// Coroutine that stores the difference in _clipSize and _ammoInClip to replenish the spent ammo 
    /// back into the weapon clip. If _infiniteAmmo is true however, we just set the ammo to replenish
    /// the specified _clipSize value and invoke the OnAmmoChanged() event for UI updates. Lastly, we invoke 
    /// the OnReload() event if we have enough ammo to reload (> 0). If the weapon is a shotgun type, 
    /// then we invoke OnManualReload() and check to see if the player wishes to cancel the OnManualReload() event
    /// when they fire the weapon mid-reload to then invoke OnReloadCancel().
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reload()
    {
        _isReloading = true;

        int ammoMissingFromClip = _clipSize - _ammoInClip;
        int ammoToReload = Math.Min(ammoMissingFromClip, _ammoInReserve);

        if (_infiniteAmmo)
            ammoToReload = _clipSize;

        if (_manualReload)
        {
            for (int missingBullets = 0; missingBullets < ammoToReload; missingBullets++)
            {
                if (_isReloading)
                {
                    OnManualReload?.Invoke();
                    yield return new WaitForSeconds(_reloadTime);
                    MoveAmmo(1);
                }
                else
                {
                    yield break;
                }                
            }
            OnManualReloadFinish?.Invoke();
            _isReloading = false;
        }

        if (!_manualReload && ammoToReload > 0)
        {
            OnReload?.Invoke();
            yield return new WaitForSeconds(_reloadTime);
            MoveAmmo(ammoToReload);
            _isReloading = false;
        }        
    }

    /// <summary>
    /// Moves the specified amount of ammo to the weapon clip and subtract from the weapons reserves.
    /// </summary>
    /// <param name="ammoToReload">Int value that will be moved into the weapon clip and removed 
    /// from the weapons reserved ammo.</param>
    private void MoveAmmo(int ammoToReload)
    {
        _ammoInClip += ammoToReload;
        _ammoInReserve -= ammoToReload;
        OnAmmoChanged?.Invoke();
    }

    #region Public Methods
    /// <summary>
    /// Checks if _ammoInClip has enough ammo to fire from the weapon.
    /// </summary>
    /// <returns>True if _ammoInClip is not 0.</returns>
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
