using System;
using System.Collections;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [SerializeField]
    private bool _infiniteAmmo;
    [SerializeField]
    private int _maxAmmo;
    [SerializeField]
    private int _maxAmmoInClip;
    [SerializeField]
    private float _reloadSpeed = 0.25f;
    [SerializeField]
    private SimpleAudioEvent _reloadSound;

    private int _ammoInClip;
    private int _ammoNotInClip;
    private Weapon _weapon;
    private AudioSource _audioSource;

    public event Action OnAmmoChanged = delegate { };

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _ammoInClip = _maxAmmoInClip;
        _ammoNotInClip = _maxAmmo - _ammoInClip;

        _weapon = GetComponent<Weapon>();
        _weapon.OnFire += Weapon_OnFire;
    }

    private void Start()
    {
        // Change the initial text on startup
        OnAmmoChanged();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    public bool IsAmmoReady()
    {
        return _ammoInClip > 0;
    }

    private void Weapon_OnFire()
    {
        RemoveAmmo();
    }

    private void RemoveAmmo()
    {
        _ammoInClip--;
        OnAmmoChanged();
    }

    private IEnumerator Reload()
    {
        int ammoMissingFromClip = _maxAmmoInClip - _ammoInClip;
        int ammoToReload = Math.Min(ammoMissingFromClip, _ammoNotInClip);

        _reloadSound.Play(_audioSource);

        if (_infiniteAmmo)
            ammoToReload = _maxAmmoInClip;

        if (ammoToReload > 0)
        {
            yield return new WaitForSeconds(_reloadSpeed);

            _ammoInClip += ammoToReload;
            _ammoNotInClip -= ammoToReload;
            OnAmmoChanged();
        }        
    }

    internal string GetCurrentAmmoText()
    {
        return string.Format("{0}/", _ammoInClip);
    }

    internal string GetNewMaxAmmoText()
    {
        if (_infiniteAmmo)
            return "999";
        else
            return string.Format("{0}", _ammoNotInClip);
    }
}
