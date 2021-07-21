using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField]
    private Image _healthFillBar;

    private void Start()
    {
        var player = FindObjectOfType<PlayerMovement>().GetComponent<Health>();
        player.OnHealthChanged += UIPlayerHealth_OnHealthChanged;
    }

    private void UIPlayerHealth_OnHealthChanged(int currentHealth, int maxHealth)
    {
        float healthPercentage = (float)currentHealth / (float)maxHealth;
        _healthFillBar.fillAmount = healthPercentage;

        if (currentHealth <= 0)
        {
            RemoveEvent();
        }
    }

    private void RemoveEvent()
    {
        var player = FindObjectOfType<PlayerMovement>().GetComponent<Health>();
        player.OnHealthChanged -= UIPlayerHealth_OnHealthChanged;
    }
}
