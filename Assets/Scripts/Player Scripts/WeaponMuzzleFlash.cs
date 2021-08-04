using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class WeaponMuzzleFlash : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Transform _muzzleLight;
    #endregion

    #region Private Fields
    private BallisticWeapon _weapon;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _weapon = GetComponentInParent<BallisticWeapon>();

        _weapon.OnFire += Weapon_OnFire;
    }
    private void OnDisable()
    {
        _weapon.OnFire -= Weapon_OnFire;
    }

    private void OnDestroy()
    {
        _weapon.OnFire -= Weapon_OnFire;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked method when the OnFire() event is called. Starts the MuzzleLightFlash() coroutine.
    /// </summary>
    private void Weapon_OnFire()
    {
        if (_muzzleLight != null)
        {
            StartCoroutine(MuzzleLightFlash());
        }
    }

    /// <summary>
    /// Coroutine that "flashes" the _muzzleLight object by enabling the light2D component, then disabling
    /// the component after a short duration.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MuzzleLightFlash()
    {
        Light2D lightFlash = _muzzleLight.GetComponent<Light2D>();
        lightFlash.enabled = true;

        yield return new WaitForSeconds(0.08f);

        lightFlash.enabled = false;
    } 
    #endregion
}
