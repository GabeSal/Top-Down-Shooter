using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class EnemyMuzzleFlash : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Transform _muzzleLight;
    #endregion

    #region Private Fields
    private EnemyShootingHandler _enemyShootingHandler;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        _enemyShootingHandler = GetComponentInParent<EnemyShootingHandler>();

        _enemyShootingHandler.EnemyOnFire += EnemyWeapon_EnemyOnFire;
    }

    private void OnDisable()
    {
        _enemyShootingHandler.EnemyOnFire -= EnemyWeapon_EnemyOnFire;
    }

    private void OnDestroy()
    {
        _enemyShootingHandler.EnemyOnFire -= EnemyWeapon_EnemyOnFire;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked when the EnemyOnFire() event is called. Starts the MuzzleLightFlash() coroutine.
    /// </summary>
    private void EnemyWeapon_EnemyOnFire()
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

        yield return new WaitForSeconds(0.03f);

        lightFlash.enabled = false;
    } 
    #endregion
}
