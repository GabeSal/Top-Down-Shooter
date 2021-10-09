using System;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _footstepEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    private List<OnTriggerSwitchPlayerFootsteps> _audioSwappers = new List<OnTriggerSwitchPlayerFootsteps>();
    #endregion

    #region Action Events
    public event Action<SimpleAudioEvent> StoreOldFootsteps;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        StoreAllAudioSwappersInScene();

        foreach (var audioSwapper in _audioSwappers)
        {
            audioSwapper.SwapPlayerFootsteps += ChangePlayerStepsTo;
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
        if (_audioSwappers != null)
        {
            foreach (var audioSwapper in _audioSwappers)
            {
                audioSwapper.SwapPlayerFootsteps -= ChangePlayerStepsTo;
            }
        }

        if (transform.CompareTag("Player"))
            GetComponent<PlayerMovement>().OnPlayerStep -= PlayPlayerFootStepSound;

        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyMovementSoundHandler>().OnEnemyStep -= PlayEnemyFootStepSound;
    }

    private void StoreAllAudioSwappersInScene()
    {
        var polygonColliders = FindObjectsOfType<PolygonCollider2D>();

        foreach (var polygonCollider in polygonColliders)
        {
            Debug.Log(polygonCollider);

            var footstepEvent = polygonCollider.GetComponent<OnTriggerSwitchPlayerFootsteps>();

            if (footstepEvent != null)
            {
                _audioSwappers.Add(footstepEvent);
            }
            else
            {
                continue;
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
