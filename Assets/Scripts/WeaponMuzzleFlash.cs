using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WeaponMuzzleFlash : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _muzzleFlash;
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
        _muzzleFlash.Play();
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
}
