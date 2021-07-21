using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class EnemyMuzzleFlash : MonoBehaviour
{
    [SerializeField]
    private Transform _muzzleLight;

    private EnemyShootingHandler _enemyShootingHandler;

    private void Awake()
    {
        _enemyShootingHandler = GetComponentInParent<EnemyShootingHandler>();

        _enemyShootingHandler.OnFire += EnemyWeapon_OnFire;
    }

    private void EnemyWeapon_OnFire()
    {
        if (_muzzleLight != null)
        {
            StartCoroutine(MuzzleLightFlash());
        }
    }

    private IEnumerator MuzzleLightFlash()
    {
        Light2D lightFlash = _muzzleLight.GetComponent<Light2D>();
        lightFlash.enabled = true;

        yield return new WaitForSeconds(0.03f);

        lightFlash.enabled = false;
    }

    private void OnDisable()
    {
        _enemyShootingHandler.OnFire -= EnemyWeapon_OnFire;
    }

    private void OnDestroy()
    {
        _enemyShootingHandler.OnFire -= EnemyWeapon_OnFire;
    }
}
