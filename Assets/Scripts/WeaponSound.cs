using UnityEngine;

[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(AudioSource))]
public class WeaponSound : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent _firedEvent;

    private AudioSource _audioSource;
    private Weapon _weapon;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _weapon = GetComponent<Weapon>();

        _weapon.OnFire += Weapon_OnFire;
    }

    private void Weapon_OnFire()
    {
        _firedEvent.Play(_audioSource);
    }

    private void OnDestroy()
    {
        _weapon.OnFire -= Weapon_OnFire;
    }
}
