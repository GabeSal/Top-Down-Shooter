using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour
{
    #region Private Fields
    private static Dictionary<PooledMonoBehaviour, Pool> _pools = new Dictionary<PooledMonoBehaviour, Pool>();

    private Queue<PooledMonoBehaviour> _objects = new Queue<PooledMonoBehaviour>();

    private PooledMonoBehaviour _prefab;
    private PooledMonoBehaviour _pooledObject;
    #endregion

    #region Standard Unity Methods
    private void OnDestroy()
    {
        _pooledObject.OnReturnToPool -= AddObjectToAvailableQueue;
        _pools.Clear();
    }
    #endregion

    #region Class Defined Methods
    public static Pool GetPool(PooledMonoBehaviour prefab)
    {
        if (_pools.ContainsKey(prefab))
            return _pools[prefab];

        var poolGameObject = new GameObject("Pool - " + prefab.name);
        var pool = poolGameObject.AddComponent<Pool>();
        pool._prefab = prefab;

        _pools.Add(prefab, pool);
        return pool;
    }

    public T Get<T>() where T : PooledMonoBehaviour
    {
        if (_objects.Count == 0)
        {
            GrowPool();
        }

        var pooledObject = _objects.Dequeue();
        return pooledObject as T;
    }
    private void GrowPool()
    {
        for (int i = 0; i < _prefab.InitialPoolSize; i++)
        {
            _pooledObject = Instantiate(_prefab);
            _pooledObject.gameObject.name += " " + i;

            _pooledObject.OnReturnToPool += AddObjectToAvailableQueue;

            _pooledObject.transform.SetParent(this.transform);
            _pooledObject.gameObject.SetActive(false);
        }
    }

    private void AddObjectToAvailableQueue(PooledMonoBehaviour pooledObject)
    {
        pooledObject.transform.SetParent(this.transform);
        _objects.Enqueue(pooledObject);
    } 
    #endregion
}
