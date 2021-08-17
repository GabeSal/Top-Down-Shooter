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
            GetComponent<PlayerMovement>().OnPlayerStep += PlayPlayerFootStepSound;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep += PlayEnemyFootStepSound;
    }

    private void OnDisable()
    {
        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep -= PlayPlayerFootStepSound;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep -= PlayEnemyFootStepSound;
    }

    private void OnDestroy()
    {
        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep -= PlayPlayerFootStepSound;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep -= PlayEnemyFootStepSound;
    }
    #endregion

    #region Received Event Methods
    private void PlayPlayerFootStepSound()
    {
        _footstepEvent.Play(_audioSource, true);
    }

    private void PlayEnemyFootStepSound()
    {
        _footstepEvent.Play(_audioSource, true);
    }
    #endregion
}
