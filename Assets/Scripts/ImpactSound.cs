using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ImpactSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _impactEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _impactEvent.Play(_audioSource);
    } 
    #endregion
}
