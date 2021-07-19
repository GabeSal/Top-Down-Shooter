using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent _explosionEvent;

    private AudioSource _audioSource;
    private EnemySelfDestruct _enemySelfDestruct;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _enemySelfDestruct = GetComponent<EnemySelfDestruct>();

        _enemySelfDestruct.OnExplosion += EnemySelfDestruct_OnExplosion;
    }

    private void EnemySelfDestruct_OnExplosion()
    {
        _explosionEvent.Play(_audioSource);
    }

    private void OnDisable()
    {
        _enemySelfDestruct.OnExplosion -= EnemySelfDestruct_OnExplosion;
    }
}
