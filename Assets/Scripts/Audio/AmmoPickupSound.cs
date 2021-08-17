using UnityEngine;

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
    }

    private void OnEnable()
    {
        GetComponent<AmmoDrop>().OnAmmoPickup += AmmoPickupSound_OnAmmoPickup;
    }

    private void OnDisable()
    {
        GetComponent<AmmoDrop>().OnAmmoPickup -= AmmoPickupSound_OnAmmoPickup;
    }

    private void OnDestroy()
    {
        GetComponent<AmmoDrop>().OnAmmoPickup -= AmmoPickupSound_OnAmmoPickup;
    }
    #endregion

    #region Class Defined Methods
    private void AmmoPickupSound_OnAmmoPickup()
    {
        var ammoDrop = this.GetComponent<AmmoDrop>();
        _ammoPickupSoundEvent.Play(_audioSource, true);

        if (!ammoDrop.infiniteAmmoDrop)
            this.GetComponent<PooledMonoBehaviour>().ReturnToPool(1.5f);
    }
    #endregion
}
