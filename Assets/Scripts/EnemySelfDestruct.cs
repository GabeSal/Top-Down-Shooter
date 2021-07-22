using System;
using System.Collections;
using UnityEngine;

public class EnemySelfDestruct : MonoBehaviour
{
    #region Serialized Fields
    [Header("Explosion Settings")]
    [SerializeField]
    [Range(5, 20)]
    private int _damage;
    [SerializeField]
    [Range(5, 12)]
    private float _blastRadius;
    [SerializeField]
    [Range(1.5f, 5f)]
    private float _countdownOnset;
    [SerializeField]
    [Range(5, 10)]
    private float _shakeAmount;
    [Header("Particle")]
    [SerializeField]
    private PooledMonoBehaviour _explosionParticle;
    #endregion

    #region Private Fields
    private bool _isShaking;
    private LayerMask _layerMask;
    #endregion

    #region Action Events
    public event Action OnExplosion;
    #endregion

    #region Standard Unity Methods
    private void Start()
    {
        _layerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (_isShaking)
        {
            Vector3 newPosition = GetRandomNewPosition();
            transform.position = newPosition;
        }
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Produces a new random position using the insideUnitSphere property from the Unity Engines Random class.
    /// </summary>
    /// <returns>Vector3 that is used to replace the previous position of the enemy.</returns>
    private Vector3 GetRandomNewPosition()
    {
        var position = transform.position + UnityEngine.Random.insideUnitSphere * (Time.deltaTime * _shakeAmount);
        position.z = transform.position.z;

        return position;
    }

    /// <summary>
    /// Disables the sprite renderer component to simulate the enemy "blowing itself up," then proceeds
    /// to generate an overlap circle to check if the player is within the explosion range. If the player
    /// is within range, they will take damage and handle the impact accordingly. An explosion particle
    /// will be pooled in the scene and play the particle system before returning back into the pool. Finally,
    /// the OnExplosion() event will invoke any components listening to it.
    /// </summary>
    private void SelfDestruct()
    {
        // Hide the sprite when exploding
        this.GetComponent<SpriteRenderer>().enabled = false;

        Collider2D playerCollision = Physics2D.OverlapCircle(transform.position, _blastRadius, _layerMask.value);

        if (playerCollision != null)
        {
            playerCollision.GetComponent<Health>().TakeHit(_damage);

            var direction = (playerCollision.transform.position - transform.position).normalized;
            playerCollision.GetComponent<HandlePlayerImpact>().Explosion(direction);
        }

        var explosion = _explosionParticle.Get<PooledMonoBehaviour>(transform.position, Quaternion.identity);
        explosion.ReturnToPool(_explosionParticle.GetComponent<ParticleSystem>().main.duration);
        OnExplosion?.Invoke();
    }

    /// <summary>
    /// Coroutine that sets the _isShaking flag to true to produce the shaking effect before calling 
    /// the SelfDestruct() method and "killing itself".
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShakeEnemy()
    {
        Vector3 originalPosition = transform.position;

        if (_isShaking == false)
            _isShaking = true;

        yield return new WaitForSeconds(_countdownOnset);

        _isShaking = false;
        transform.position = originalPosition;

        SelfDestruct();
        float explosionLifetime = 6f;

        yield return new WaitForSeconds(explosionLifetime);
        this.GetComponent<Health>().TakeHit(9999999);
    }
    #endregion

    #region Internal Methods
    /// <summary>
    /// Starts the ShakeEnemy() coroutine.
    /// </summary>
    internal void BeginCountdown()
    {
        StartCoroutine(ShakeEnemy());
    } 
    #endregion
}
