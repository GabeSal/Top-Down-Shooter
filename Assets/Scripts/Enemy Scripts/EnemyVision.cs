using System;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    #region Action Events
    public event Action OnPlayerInVision;
    public event Action OnPlayerOutOfVision;
    #endregion

    #region Standard Unity Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPlayerInVision?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            OnPlayerOutOfVision?.Invoke();
    }
    #endregion
}
