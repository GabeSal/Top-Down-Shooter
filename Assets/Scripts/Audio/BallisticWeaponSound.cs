using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BallisticWeaponSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _firedEvent;
    [SerializeField]
    private SimpleAudioEvent _outOfAmmoEvent;
    [SerializeField]
    private SimpleAudioEvent _manualReloadEvent;
    [SerializeField]
    private SimpleAudioEvent _reloadEvent;
    #endregion

    #region Private Fields
    private AudioSource _shootSource;
    private AudioSource _reloadSource;
    private BallisticWeapon _ballisticWeapon;
    private WeaponAmmo _weaponAmmo;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _shootSource = audioSources[0];
        _reloadSource = audioSources[1];

        _ballisticWeapon = GetComponent<BallisticWeapon>();
        _weaponAmmo = GetComponent<WeaponAmmo>();

        _ballisticWeapon.OnFire += Weapon_OnFire;
        _ballisticWeapon.OutOfAmmo += Weapon_OutOfAmmo;
        _ballisticWeapon.OnShotgunPump += BallisticWeapon_OnShotgunPump;
        _weaponAmmo.OnReload += WeaponAmmo_OnReload;
        _weaponAmmo.OnManualReload += WeaponAmmo_OnManualReload;
        _weaponAmmo.OnManualReloadFinish += WeaponAmmo_OnManualReloadFinish;
    }

    private void OnDisable()
    {
        _ballisticWeapon.OnFire -= Weapon_OnFire;
        _ballisticWeapon.OutOfAmmo -= Weapon_OutOfAmmo;
        _ballisticWeapon.OnShotgunPump -= BallisticWeapon_OnShotgunPump;
        _weaponAmmo.OnReload -= WeaponAmmo_OnReload;
        _weaponAmmo.OnManualReload -= WeaponAmmo_OnManualReload;
        _weaponAmmo.OnManualReloadFinish -= WeaponAmmo_OnManualReloadFinish;
    }

    private void OnDestroy()
    {
        _ballisticWeapon.OnFire -= Weapon_OnFire;
        _ballisticWeapon.OutOfAmmo -= Weapon_OutOfAmmo;
        _ballisticWeapon.OnShotgunPump -= BallisticWeapon_OnShotgunPump;
        _weaponAmmo.OnReload -= WeaponAmmo_OnReload;
        _weaponAmmo.OnManualReload -= WeaponAmmo_OnManualReload;
        _weaponAmmo.OnManualReloadFinish -= WeaponAmmo_OnManualReloadFinish;
    }
    #endregion

    #region Received Event Methods
    /// <summary>
    /// Plays the _outOfAmmoEvent SimpleAudioEvent object through the audio source when the 
    /// OutOfAmmo() event is invoked.
    /// </summary>
    private void Weapon_OutOfAmmo()
    {
        _outOfAmmoEvent.Play(_shootSource, true);
    }
    /// <summary>
    /// Plays the _firedEvent SimpleAudioEvent object through the audio source when the 
    /// OnFire() event is invoked.
    /// </summary>
    private void Weapon_OnFire()
    {
        _firedEvent.Play(_shootSource);
    }
    /// <summary>
    /// Plays the appropriate _reloadEvent SimpleAudioEvent object through the audio source when the 
    /// OnShotgunPump() event is invoked to simulate the shotgun being cocked after a shot.
    /// </summary>
    private void BallisticWeapon_OnShotgunPump()
    {
        _reloadEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the _manualReloadEvent SimpleAudioEvent object the audio source when the OnManualReload() event is invoked.
    /// </summary>
    private void WeaponAmmo_OnManualReload()
    {
        _manualReloadEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the appropriate _reloadEvent SimpleAudioEvent object through the audio source when the OnManualReloadFinish() 
    /// event is invoked to simulate the shotgun being cocked after all shells are reloaded.
    /// </summary>
    private void WeaponAmmo_OnManualReloadFinish()
    {
        _reloadEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the _reloadEvent SimpleAudioEvent object the audio source when the OnReload() event is invoked.
    /// </summary>
    private void WeaponAmmo_OnReload()
    {
        _reloadEvent.Play(_reloadSource, true);
    }
    #endregion
}
