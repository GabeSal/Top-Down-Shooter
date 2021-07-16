using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyStatus : MonoBehaviour
{
    [SerializeField]
    private float _recoilForce = 2f;
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;

    private Rigidbody2D _rigidBody2D;

    private void Start()
    {
        GetComponent<Health>().OnDied += EnemyStatus_OnDied;
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void EnemyStatus_OnDied()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnDied -= EnemyStatus_OnDied;
    }

    internal void RecoilFromHit(Vector2 recoilDirection)
    {
        _rigidBody2D.AddForce(recoilDirection * _recoilForce, ForceMode2D.Impulse);
    }

    internal void SpawnBloodSplatterParticle(Vector2 point, Vector2 normal)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(-normal));
        bloodSplatter.ReturnToPool(1f);
    }
}
