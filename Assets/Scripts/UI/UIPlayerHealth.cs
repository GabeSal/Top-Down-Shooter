using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Image _healthFillBar;
    #endregion

    #region Action Events
    public event Action OnPlayerDied;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        var player = FindObjectOfType<PlayerMovement>().GetComponent<Health>();
        player.OnHealthChanged += UIPlayerHealth_OnHealthChanged;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Invoked when the OnHealthChanged() event is called. Sets the fill amount of the _healthFillBar sprite
    /// to the quotient of the currentHealth and maxHealth value casted as a float. If currentHealth reaches 0
    /// or below, we then call RemoveEvent().
    /// </summary>
    /// <param name="currentHealth">int value passed from the players _currentHealth value.</param>
    /// <param name="maxHealth">int value passed from the players _maxHealth value.</param>
    private void UIPlayerHealth_OnHealthChanged(int currentHealth, int maxHealth)
    {
        float healthPercentage = (float)currentHealth / (float)maxHealth;
        _healthFillBar.fillAmount = healthPercentage;

        if (currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
            RemoveHealthEvent();
        }
    }

    /// <summary>
    /// This method de-registers the UIPlayerHealth_OnHealthChanged() method associated with the OnHealthChanged() event.
    /// </summary>
    private void RemoveHealthEvent()
    {
        var player = FindObjectOfType<PlayerMovement>().GetComponent<Health>();
        player.OnHealthChanged -= UIPlayerHealth_OnHealthChanged;
    } 
    #endregion
}
