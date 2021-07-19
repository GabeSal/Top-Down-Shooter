using System;
using System.Collections;
using UnityEngine;

public class EnemySelfDestruct : MonoBehaviour
{
    [SerializeField]
    private int _damage = 5;
    [SerializeField]
    private float _blastRadius = 4f;
    [SerializeField]
    private float _countdownOnset = 3f;
    [SerializeField]
    private float _shakeAmount = 2.5f;
    [SerializeField]
    private PooledMonoBehaviour _explosionParticle;

    private bool _isShaking;
    private LayerMask _layerMask;

    public event Action OnExplosion = delegate { };

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Player");
    }

    private void LateUpdate()
    {
        if (_isShaking)
        {
            Vector3 newPosition = transform.position + UnityEngine.Random.insideUnitSphere * (Time.deltaTime * _shakeAmount);
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
    }

    private IEnumerator ShakeEnemy()
    {
        Vector3 originalPosition = transform.position;

        if (_isShaking == false)
            _isShaking = true;

        yield return new WaitForSeconds(_countdownOnset);

        _isShaking = false;
        transform.position = originalPosition;

        SelfDestruct();
        float explosionLifetime = 5f;

        yield return new WaitForSeconds(explosionLifetime);
        this.GetComponent<Health>().TakeHit(9999999);
    }

    internal void BeginCountdown()
    {
        StartCoroutine(ShakeEnemy());
    }
    
    private void SelfDestruct()
    {
        // Hide the sprite when exploding
        this.GetComponent<SpriteRenderer>().enabled = false;

        Collider2D playerCollision = Physics2D.OverlapCircle(transform.position, _blastRadius, _layerMask.value);

        if (playerCollision != null)
        {
            Vector2 direction = -GetDirectionToPlayer(playerCollision);

            playerCollision.GetComponent<Health>().TakeHit(_damage);
            playerCollision.GetComponent<HandlePlayerImpact>().EnemyBlast(direction);
        }

        var explosion = _explosionParticle.Get<PooledMonoBehaviour>(transform.position, Quaternion.identity);
        explosion.ReturnToPool(_explosionParticle.GetComponent<ParticleSystem>().main.duration);
        OnExplosion();
    }

    private Vector2 GetDirectionToPlayer(Collider2D player)
    {
        var playerPosition = player.transform.position;
        return (this.transform.position - playerPosition).normalized;
    }
}
