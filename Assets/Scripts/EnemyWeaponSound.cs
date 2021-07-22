using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyWeaponSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _firedEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    private EnemyShootingHandler _enemyShootingHandler; 
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _enemyShootingHandler = GetComponent<EnemyShootingHandler>();

        _enemyShootingHandler.EnemyOnFire += EnemyShootingHandler_EnemyOnFire;
    }
    private void OnDisable()
    {
        _enemyShootingHandler.EnemyOnFire -= EnemyShootingHandler_EnemyOnFire;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked when the EnemyOnFire() event is called. Plays the fired SimpleAudioEvent.
    /// </summary>
    private void EnemyShootingHandler_EnemyOnFire()
    {
        _firedEvent.Play(_audioSource);
    }  
    #endregion
}
