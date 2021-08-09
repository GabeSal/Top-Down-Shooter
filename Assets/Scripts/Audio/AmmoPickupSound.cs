using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AmmoPickupSound : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private SimpleAudioEvent _ammoPickupSoundEvent;
    #endregion

    #region Private Fields
    private AudioSource _audioSource;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        GetComponent<AmmoDrop>().OnAmmoPickup += AmmoPickupSound_OnAmmoPickup;
    }

    private void OnDestroy()
    {
        GetComponent<AmmoDrop>().OnAmmoPickup -= AmmoPickupSound_OnAmmoPickup;
    }

    private void OnDisable()
    {
        GetComponent<AmmoDrop>().OnAmmoPickup -= AmmoPickupSound_OnAmmoPickup;
    }
    #endregion

    #region Class Defined Methods
    private void AmmoPickupSound_OnAmmoPickup()
    {
        _ammoPickupSoundEvent.Play(_audioSource, true);

        StartCoroutine(DisableAmmoDrop());
    }

    /// <summary>
    /// Coroutine that waits until the audio clip has finished playing before disabling the ammo drop
    /// object in the scene.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableAmmoDrop()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        this.gameObject.SetActive(false);
    }
    #endregion
}
