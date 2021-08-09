using UnityEngine;
using TMPro;

public class UIAmmoPickup : MonoBehaviour
{
    #region Private Fields
    private AmmoDrop _ammoDrop;
    private Animator _animator;
    private TextMeshProUGUI _text;
    #endregion

    #region Standard Unity Methods
    private void Awake()
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
        UnsubscribeFromAmmoDropEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromAmmoDropEvents();
    }
    #endregion

    #region Class Defined Methods
    private void UnsubscribeFromAmmoDropEvents()
    {
        _ammoDrop.OnMaxAmmoReached -= UIAmmoPickup_OnMaxAmmoReached;
        _ammoDrop.OnAmmoPickup -= UIAmmoPickup_OnAmmoPickup;
    }

    private void ShowAndPlayTextAnimation()
    {
        _text.alpha = 1f;
        _animator.SetTrigger("TouchPlayer");
    }

    private void HideText()
    {
        _text.alpha = 0f;
        _animator.Play("Default");
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
