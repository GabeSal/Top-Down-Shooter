using System;
using TMPro;
using UnityEngine;

public class UIEndPointInteraction : MonoBehaviour
{
    #region Private Fields
    private EndPoint _endPoint;
    private Animator _animator;
    private TextMeshProUGUI _text;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _endPoint = GetComponentInParent<EndPoint>();

        _endPoint.OnEndPointCollision += EndPointUI_OnEndPointCollision;
        _endPoint.OnLeavingEndPointCollision += EndPointUI_OnLeavingEndPointCollision;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEndPointEvents();
    }
    #endregion

    #region Class Defined Methods
    private void ShowAndPlayTextAnimation()
    {
        _animator.SetTrigger("TouchPlayer");
    }

    private void HideText()
    {
        _animator.Play("Default");
    }

    private void UnsubscribeFromEndPointEvents()
    {
        _endPoint.OnEndPointCollision -= EndPointUI_OnEndPointCollision;
        _endPoint.OnLeavingEndPointCollision -= EndPointUI_OnLeavingEndPointCollision;
    }

    private void EndPointUI_OnEndPointCollision()
    {
        ShowAndPlayTextAnimation();
        _text.text = $"Enter {_endPoint.name}";
    }

    private void EndPointUI_OnLeavingEndPointCollision()
    {
        HideText();
    }
    #endregion
}
