using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _firedEvent;
    [SerializeField]
    private SimpleAudioEvent _outOfAmmoEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    private Weapon _weapon;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _weapon = GetComponent<Weapon>();

        _weapon.OnFire += Weapon_OnFire;
        _weapon.OutOfAmmo += Weapon_OutOfAmmo;
    }
    private void OnDestroy()
    {
        _weapon.OnFire -= Weapon_OnFire;
        _weapon.OutOfAmmo -= Weapon_OutOfAmmo;
    }
    #endregion

    #region Received Event Methods
    /// <summary>
    /// Plays the _outOfAmmoEvent SimpleAudioEvent object through the audio source when the 
    /// OutOfAmmo() event is invoked.
    /// </summary>
    private void Weapon_OutOfAmmo()
    {
        _outOfAmmoEvent.Play(_audioSource);
    }
    /// <summary>
    /// Plays the _firedEvent SimpleAudioEvent object through the audio source when the 
    /// OnFire() event is invoked.
    /// </summary>
    private void Weapon_OnFire()
    {
        _firedEvent.Play(_audioSource);
    }
    #endregion
}
