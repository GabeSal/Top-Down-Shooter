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
        {
            GetComponent<PlayerMovement>().OnPlayerStep += FootstepSounds_OnPlayerStep;
        }
    }

    private void OnDisable()
    {
        GetComponent<PlayerMovement>().OnPlayerStep -= FootstepSounds_OnPlayerStep;
    }

    private void OnDestroy()
    {
        GetComponent<PlayerMovement>().OnPlayerStep -= FootstepSounds_OnPlayerStep;
    }
    #endregion

    #region Received Event Methods
    private void FootstepSounds_OnPlayerStep()
    {
        _footstepEvent.Play(_audioSource, true);
    }
    #endregion
}
