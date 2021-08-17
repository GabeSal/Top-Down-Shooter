using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class HealingItemSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _healingSoundEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        GetComponent<HealingItem>().OnHealingItemPickup += PlayHealingItemSound;
    }

    private void OnDestroy()
    {
        GetComponent<HealingItem>().OnHealingItemPickup -= PlayHealingItemSound;
    }

    private void OnDisable()
    {
        GetComponent<HealingItem>().OnHealingItemPickup -= PlayHealingItemSound;
    }
    #endregion

    #region Class Defined Methods
    private void PlayHealingItemSound()
    {
        _healingSoundEvent.Play(_audioSource, true);

        StartCoroutine(DisableHealingItem());
    }

    /// <summary>
    /// Coroutine that waits until the audio clip has finished playing before disabling the healing item
    /// object in the scene.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableHealingItem()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        this.gameObject.SetActive(false);
    }
    #endregion
}
