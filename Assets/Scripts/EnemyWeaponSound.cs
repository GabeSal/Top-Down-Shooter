using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyWeaponSound : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent _firedEvent;

    private AudioSource _audioSource;
    private EnemyShootingHandler _enemyShootingHandler;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _enemyShootingHandler = GetComponent<EnemyShootingHandler>();

        _enemyShootingHandler.OnFire += EnemyShootingHandler_OnFire;
    }

    private void EnemyShootingHandler_OnFire()
    {
        _firedEvent.Play(_audioSource);
    }

    private void OnDestroy()
    {
        _enemyShootingHandler.OnFire -= EnemyShootingHandler_OnFire;
    }
}
