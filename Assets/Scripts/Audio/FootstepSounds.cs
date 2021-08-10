using System;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _footstepEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep += FootstepSounds_OnPlayerStep;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep += FootstepSounds_OnEnemyStep;
    }

    private void OnDisable()
    {
        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep -= FootstepSounds_OnPlayerStep;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep -= FootstepSounds_OnEnemyStep;
    }

    private void OnDestroy()
    {
        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep -= FootstepSounds_OnPlayerStep;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep -= FootstepSounds_OnEnemyStep;
    }
    #endregion

    #region Received Event Methods
    private void FootstepSounds_OnPlayerStep()
    {
        _footstepEvent.Play(_audioSource, true);
    }

    private void FootstepSounds_OnEnemyStep()
    {
        _footstepEvent.Play(_audioSource, true);
    }
    #endregion
}
