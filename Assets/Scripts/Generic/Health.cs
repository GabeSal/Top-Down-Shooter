using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private int _maxHealth;
    #endregion

    #region Private Fields
    private int _currentHealth;
    #endregion

    #region Action Events
    public event Action OnTookHit;
    public event Action OnDied;
    public event Action<int, int> OnHealthChanged;
    #endregion

    #region Standard Unity Methods
    private void OnEnable()
    {
        _currentHealth = _maxHealth;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Checks to see if the _currentHealth of the game object is greater than 0 before proceeding to call the
    /// ModifyHealth() method and invoke the appropriate event depending on the _currentHealth remaining.
    /// </summary>
    /// <param name="damage">Int value that is passed from the damage field 
    /// of the corresponding class calling this function.</param>
    public void TakeHit(int damage)
    {
        if (_currentHealth <= 0)
            return;

        ModifyHealth(-damage);

        if (_currentHealth > 0)
            OnTookHit?.Invoke();
        else
            OnDied?.Invoke();
    }

    /// <summary>
    /// Public method that's called from the HealingItem object when the player interacts with the item to regain
    /// their lost health.
    /// </summary>
    /// <param name="healAmount">Int value that's passed from the healing item to modify the health.</param>
    public void Heal(int healAmount)
    {
        ModifyHealth(healAmount);
    }

    /// <summary>
    /// Can either increase or decrease the _currentHealth amount depending on the sign of the passed int
    /// parameter, amount. After adding or subtracting from _currentHealth, the OnHealthChanged() event is
    /// invoked to update the Player Health UI elements.
    /// </summary>
    /// <param name="amount">Int value that is added to the _currentHealth.</param>
    private void ModifyHealth(int amount)
    {
        _currentHealth += amount;

        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;
        
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
    #endregion
}
