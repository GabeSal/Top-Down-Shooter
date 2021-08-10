using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _explosionEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _explosionEvent.Play(_audioSource, true);
    }
    #endregion
}
