using UnityEngine;

public class EnemyDropOnDeath : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private PooledMonoBehaviour _itemDrop;
    [SerializeField]
    private int _amountToDrop;
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        GetComponent<Health>().OnDied += EnemyDropOnDeath_OnDied;
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnDied -= EnemyDropOnDeath_OnDied;
    }

    private void OnDestroy()
    {
        GetComponent<Health>().OnDied -= EnemyDropOnDeath_OnDied;
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Method that's invoked when the enemies OnDied() event is called. Pools the item drop at the position
    /// where the enemy died.
    /// </summary>
    private void EnemyDropOnDeath_OnDied()
    {
        float dropChance = Random.Range(0f, 100f);

        for (int i = 0; i < _amountToDrop; i++)
        {
            if (dropChance >= 45f)
                _itemDrop.Get<PooledMonoBehaviour>(transform.position + GetRandomVectorOffset(), Quaternion.identity);
            else
                return;
        }
    }

    /// <summary>
    /// Generates a random vector3 coordinate to offset the spawn of the item drop object.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomVectorOffset()
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(-1.5f, 1.5f);

        return new Vector3(randomX, randomY, 0f);
    }
    #endregion
}
