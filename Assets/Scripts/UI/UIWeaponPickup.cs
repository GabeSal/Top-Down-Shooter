using System;
using TMPro;
using UnityEngine;

public class UIWeaponPickup : MonoBehaviour
{
    #region Private Fields
    private WeaponPickup _weaponPickup;
    private Animator _animator;
    private TextMeshProUGUI _text;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _weaponPickup = GetComponentInParent<WeaponPickup>();

        _weaponPickup.OnLeavingWeaponPickup += WeaponPickupUI_OnLeavingWeaponPickup;
        _weaponPickup.OnWeaponInteraction += WeaponPickupUI_OnWeaponInteraction;
    }

    private void OnDisable()
    {
        if (NotEmpty())
            UnsubscribeFromAmmoDropEvents();
    }

    private void OnDestroy()
    {
        if (NotEmpty())
            UnsubscribeFromAmmoDropEvents();
    }
    #endregion

    #region Class Defined Methods
    private void UnsubscribeFromAmmoDropEvents()
    {
        _weaponPickup.OnLeavingWeaponPickup -= WeaponPickupUI_OnLeavingWeaponPickup;
    }

    private void ShowAndPlayTextAnimation()
    {
        _animator.SetTrigger("TouchPlayer");
    }

    private void HideText()
    {
        _animator.Play("Default");
    }

    /// <summary>
    /// Checks to see if we have stored the necessary components.
    /// </summary>
    /// <returns>True if we have stored the animator, textmeshpro text object, and AmmoDrop component in this object.</returns>
    private bool NotEmpty()
    {
        return _animator != null && _text != null && _weaponPickup != null;
    }

    private void WeaponPickupUI_OnWeaponInteraction()
    {
        ShowAndPlayTextAnimation();
        _text.text = $"Pick up {_weaponPickup.transform.GetChild(0).name}";
    }

    private void WeaponPickupUI_OnLeavingWeaponPickup()
    {
        HideText();
    }
    #endregion
}
