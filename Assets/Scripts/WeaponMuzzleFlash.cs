using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WeaponMuzzleFlash : MonoBehaviour
{
    [SerializeField]
    private Transform _muzzleLight;

    private Weapon _weapon;

    private void Awake()
    {
        _weapon = GetComponentInParent<Weapon>();

        _weapon.OnFire += Weapon_OnFire;
    }

    private void Weapon_OnFire()
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
        _weapon.OnFire -= Weapon_OnFire;
    }

    private void OnDestroy()
    {
        _weapon.OnFire -= Weapon_OnFire;
    }
}
