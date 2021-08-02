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
    private SimpleAudioEvent _reloadEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    private BallisticWeapon _ballisticWeapon;
    private WeaponAmmo _weaponAmmo;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _ballisticWeapon = GetComponent<BallisticWeapon>();
        _weaponAmmo = GetComponent<WeaponAmmo>();

        _ballisticWeapon.OnFire += Weapon_OnFire;
        _ballisticWeapon.OutOfAmmo += Weapon_OutOfAmmo;
        _weaponAmmo.OnReload += WeaponAmmo_OnReload;
    }

    private void OnDestroy()
    {
        _ballisticWeapon.OnFire -= Weapon_OnFire;
        _ballisticWeapon.OutOfAmmo -= Weapon_OutOfAmmo;
    }
    #endregion

    #region Received Event Methods
    /// <summary>
    /// Plays the _outOfAmmoEvent SimpleAudioEvent object through the audio source when the 
    /// OutOfAmmo() event is invoked.
    /// </summary>
    private void Weapon_OutOfAmmo()
    {
        _outOfAmmoEvent.Play(_audioSource, true);
    }
    /// <summary>
    /// Plays the _firedEvent SimpleAudioEvent object through the audio source when the 
    /// OnFire() event is invoked.
    /// </summary>
    private void Weapon_OnFire()
    {
        _firedEvent.Play(_audioSource);
    }
    /// <summary>
    /// Plays the _reloadEvent SimpleAudioEvent object the audio source when the OnReload() event is invoked.
    /// </summary>
    private void WeaponAmmo_OnReload()
    {
        _reloadEvent.Play(_audioSource, true);
    }
    #endregion
}
