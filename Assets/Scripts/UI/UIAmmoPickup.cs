using TMPro;
using UnityEngine;

public class UIAmmoPickup : MonoBehaviour
{
    #region Private Fields
    private AmmoDrop _ammoDrop;
    private Animator _animator;
    private TextMeshProUGUI _text;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _ammoDrop = GetComponentInParent<AmmoDrop>();

        _ammoDrop.OnMaxAmmoReached += UIAmmoPickup_OnMaxAmmoReached;
        _ammoDrop.OnAmmoPickup += UIAmmoPickup_OnAmmoPickup;
        _ammoDrop.OnLeavingAmmoPickup += UIAmmoDrop_OnLeavingAmmoPickup;
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
        _ammoDrop.OnMaxAmmoReached -= UIAmmoPickup_OnMaxAmmoReached;
        _ammoDrop.OnAmmoPickup -= UIAmmoPickup_OnAmmoPickup;
        _ammoDrop.OnLeavingAmmoPickup -= UIAmmoDrop_OnLeavingAmmoPickup;
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
        return _animator != null && _text != null && _ammoDrop != null;
    }

    private void UIAmmoPickup_OnAmmoPickup()
    {
        ShowAndPlayTextAnimation();
        _text.text = $"Picked up {_ammoDrop.AmmoAmount} ammo for {_ammoDrop.WeaponToAddAmmo.name}!";
    }

    private void UIAmmoPickup_OnMaxAmmoReached()
    {
        ShowAndPlayTextAnimation();
        _text.text = $"Max ammo reached for {_ammoDrop.WeaponToAddAmmo.name}!";
    }

    private void UIAmmoDrop_OnLeavingAmmoPickup()
    {
        HideText();
    }
    #endregion
}
