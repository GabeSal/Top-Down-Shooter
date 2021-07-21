using System;
using UnityEngine;

public class HandlePlayerImpact : MonoBehaviour
{
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;

    private Health _playerHealth;

    private void Awake()
    {
        _playerHealth = GetComponent<Health>();
        _playerHealth.OnDied += PlayerHealth_OnDied;
    }

    private void PlayerHealth_OnDied()
    {
        this.gameObject.SetActive(false);
    }

    internal void SpawnBloodSplatterParticle(Vector2 point, Vector2 normal)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(normal));
        bloodSplatter.ReturnToPool(1f);
    }

    internal void MeleeAttack(Vector3 originOfForce)
    {
        GetComponent<Rigidbody2D>().AddForce(originOfForce * 25f, ForceMode2D.Impulse);
    }

    internal void Explosion(Vector3 forceVector)
    {
        GetComponent<Rigidbody2D>().AddForce(forceVector * 100f, ForceMode2D.Impulse);
    }

    private void OnDisable()
    {
        _playerHealth.OnDied -= PlayerHealth_OnDied;
    }
}
