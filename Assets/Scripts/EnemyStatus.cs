using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyStatus : MonoBehaviour
{
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;

    private void Start()
    {
        GetComponent<Health>().OnDied += EnemyStatus_OnDied;
    }

    private void EnemyStatus_OnDied()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnDied -= EnemyStatus_OnDied;
    }

    internal void SpawnBloodSplatterParticle(Vector2 point, Vector2 normal)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(normal));
        bloodSplatter.ReturnToPool(1f);
    }
}
