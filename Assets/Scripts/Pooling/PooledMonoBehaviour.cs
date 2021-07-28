using System;
using System.Collections;
using UnityEngine;

public class PooledMonoBehaviour : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private int _initialPoolSize = 10;
    #endregion

    #region Action Events
    public event Action<PooledMonoBehaviour> OnReturnToPool;
    #endregion

    #region Properties
    public int InitialPoolSize { get { return _initialPoolSize; } }
    #endregion

    #region Standard Unity Methods
    protected virtual void OnDisable()
    {
        if (OnReturnToPool != null)
            OnReturnToPool(this);
    }
    #endregion

    #region Class Defined Methods
    public T Get<T>(bool enable = true) where T : PooledMonoBehaviour
    {
        var pool = Pool.GetPool(this);
        var pooledObject = pool.Get<T>();

        if (enable)
        {
            pooledObject.gameObject.SetActive(true);
        }

        return pooledObject;
    }
    public T Get<T>(Vector3 position, Quaternion rotation) where T : PooledMonoBehaviour
    {
        var pooledObject = Get<T>();

        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;

        return pooledObject;
    }

    public void ReturnToPool(float delay = 0f)
    {
        StartCoroutine(ReturnToPoolAfterSeconds(delay));
    }

    private IEnumerator ReturnToPoolAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    } 
    #endregion
}
