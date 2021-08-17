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
    [SerializeField]
    private SimpleAudioEvent _manualActionEvent;
    [SerializeField]
    private SimpleAudioEvent _changeWeaponEvent;
    #endregion

    #region Private Fields
    private AudioSource _shootSource;
    private AudioSource _reloadSource;
    private BallisticWeapon _ballisticWeapon;
    private WeaponInventory _weaponInventory;
    private WeaponAmmo _weaponAmmo;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _shootSource = audioSources[0];
        _reloadSource = audioSources[1];

        _ballisticWeapon = GetComponent<BallisticWeapon>();
        _weaponInventory = GameManager.Instance.GetComponentInChildren<WeaponInventory>();
        _weaponAmmo = GetComponent<WeaponAmmo>();
        SubscribeToWeaponEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToWeaponEvents();
    }
    #endregion

    #region Class Defined Methods
    private void SubscribeToWeaponEvents()
    {
        _ballisticWeapon.OnFire += PlayWeaponFireSound;
        _ballisticWeapon.OutOfAmmo += PlayOutOfAmmoSound;
        _ballisticWeapon.OnManualAction += PlayWeaponManualActionSound;
        _weaponInventory.OnWeaponChanged += PlayWeaponReadySound;
        _weaponAmmo.OnReload += PlayReloadSound;
        _weaponAmmo.OnManualReload += PlayManualReloadSound;
        _weaponAmmo.OnManualReloadFinish += PlayManualReloadFinishSound;
    }

    private void UnsubscribeToWeaponEvents()
    {
        _ballisticWeapon.OnFire -= PlayWeaponFireSound;
        _ballisticWeapon.OutOfAmmo -= PlayOutOfAmmoSound;
        _ballisticWeapon.OnManualAction -= PlayWeaponManualActionSound;
        _weaponInventory.OnWeaponChanged -= PlayWeaponReadySound;
        _weaponAmmo.OnReload -= PlayReloadSound;
        _weaponAmmo.OnManualReload -= PlayManualReloadSound;
        _weaponAmmo.OnManualReloadFinish -= PlayManualReloadFinishSound;
    }

    #region Received Event Methods
    /// <summary>
    /// Plays the _outOfAmmoEvent SimpleAudioEvent object through the audio source when the 
    /// OutOfAmmo() event is invoked.
    /// </summary>
    private void PlayOutOfAmmoSound()
    {
        _outOfAmmoEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the _firedEvent SimpleAudioEvent object through the audio source when the 
    /// OnFire() event is invoked.
    /// </summary>
    private void PlayWeaponFireSound()
    {
        _firedEvent.Play(_shootSource);
    }
    /// <summary>
    /// Plays the appropriate _reloadEvent SimpleAudioEvent object through the audio source when the 
    /// OnShotgunPump() event is invoked to simulate the shotgun being cocked after a shot.
    /// </summary>
    private void PlayWeaponManualActionSound()
    {
        _manualActionEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the _manualReloadEvent SimpleAudioEvent object the audio source when the OnManualReload() event is invoked.
    /// </summary>
    private void PlayManualReloadSound()
    {
        _manualReloadEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the appropriate _reloadEvent SimpleAudioEvent object through the audio source when the OnManualReloadFinish() 
    /// event is invoked to simulate the shotgun being cocked after all shells are reloaded.
    /// </summary>
    private void PlayManualReloadFinishSound()
    {
        _reloadEvent.Play(_reloadSource, true);
    }
    /// <summary>
    /// Plays the _reloadEvent SimpleAudioEvent object the audio source when the OnReload() event is invoked.
    /// </summary>
    private void PlayReloadSound()
    {
        _reloadEvent.Play(_reloadSource, true);
    }

    private void PlayWeaponReadySound()
    {
        _changeWeaponEvent.Play(_shootSource, true);
    }
    #endregion

    #endregion
}
