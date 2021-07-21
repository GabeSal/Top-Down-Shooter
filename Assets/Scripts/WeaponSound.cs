using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponSound : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent _firedEvent;
    [SerializeField]
    private SimpleAudioEvent _outOfAmmoEvent;

    private AudioSource _audioSource;
    private Weapon _weapon;
    private WeaponAmmo _ammo;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _weapon = GetComponent<Weapon>();
        _ammo = GetComponent<WeaponAmmo>();

        _weapon.OnFire += Weapon_OnFire;
        _weapon.OutOfAmmo += Weapon_OutOfAmmo;
    }

    private void Weapon_OutOfAmmo()
    {
        _outOfAmmoEvent.Play(_audioSource);
    }

    private void Weapon_OnFire()
    {
        _firedEvent.Play(_audioSource);
    }

    private void OnDestroy()
    {
        _weapon.OnFire -= Weapon_OnFire;
        _weapon.OutOfAmmo -= Weapon_OutOfAmmo;
    }
}
