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
    private PolygonCollider2D[] _audioSwappers;
    #endregion

    #region Action Events
    public event Action<SimpleAudioEvent> StoreOldFootsteps;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSwappers = FindObjectsOfType<PolygonCollider2D>();
        if (_audioSwappers != null)
        {
            foreach (var audioSwapper in _audioSwappers)
            {
                var footstepEvent = audioSwapper.GetComponent<OnTriggerSwitchPlayerFootsteps>();

                if (footstepEvent != null)
                    footstepEvent.SwapPlayerFootsteps += ChangePlayerStepsTo;
                else
                    continue;                
            }
        }

        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep += PlayPlayerFootStepSound;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep += PlayEnemyFootStepSound;
    }    

    private void OnDisable()
    {
        DeregisterEvents();
    }

    private void OnDestroy()
    {
        DeregisterEvents();
    }
    #endregion

    #region Class Defined Methods
    private void DeregisterEvents()
    {
        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep -= PlayPlayerFootStepSound;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep -= PlayEnemyFootStepSound;

        if (_audioSwappers != null)
        {
            foreach (var audioSwapper in _audioSwappers)
            {
                audioSwapper.GetComponent<OnTriggerSwitchPlayerFootsteps>().SwapPlayerFootsteps -= ChangePlayerStepsTo;
            }
        }
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

    private void ChangePlayerStepsTo(SimpleAudioEvent newAudioEvent)
    {
        StoreOldFootsteps?.Invoke(_footstepEvent);
        _footstepEvent = newAudioEvent;
    }
    #endregion
}
