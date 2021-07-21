using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth;

    private int _currentHealth;

    public event Action OnTookHit = delegate { };
    public event Action OnDied = delegate { };
    public event Action<int, int> OnHealthChanged = delegate { };

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeHit(int damage)
    {
        if (_currentHealth <= 0)
            return;

        ModifyHealth(-damage);

        if (_currentHealth > 0)
            OnTookHit();
        else
            OnDied();
    }

    private void ModifyHealth(int amount)
    {
        _currentHealth += amount;
        OnHealthChanged(_currentHealth, _maxHealth);
    }
}
