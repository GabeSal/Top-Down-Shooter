using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _explosionEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    private EnemySelfDestruct _enemySelfDestruct;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _enemySelfDestruct = GetComponent<EnemySelfDestruct>();

        _enemySelfDestruct.OnExplosion += EnemySelfDestruct_OnExplosion;
    }

    private void OnDisable()
    {
        _enemySelfDestruct.OnExplosion -= EnemySelfDestruct_OnExplosion;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked method when the OnExplosion() is called. Plays the explosion SimpleAudioEvent.
    /// </summary>
    private void EnemySelfDestruct_OnExplosion()
    {
        _explosionEvent.Play(_audioSource);
    }     
    #endregion
}
