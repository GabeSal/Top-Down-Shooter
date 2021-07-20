using UnityEngine;

public class EnemyCloseQuartersAttacks : MonoBehaviour
{
    [SerializeField]
    private float _attackRange = 3.5f;
    [SerializeField]
    private float _timeUntilNextAttack = 1f;
    [SerializeField]
    private int _damage = 1;

    private float _attackTimer = 0;
    private LayerMask _layerMask;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (GetPlayerDistance() <= _attackRange)
        {
            _attackTimer += Time.deltaTime;

            if (_attackTimer > _timeUntilNextAttack)
            {
                AttackPlayer();
            }
        }
        else
        {
            _attackTimer = 0;
        }
    }

    private void AttackPlayer()
    {
        _attackTimer = 0;

        Collider2D player = CheckForPlayerCollision();

        if (player != null)
        {
            player.GetComponent<Health>().TakeHit(_damage);
        }
    }

    private float GetPlayerDistance()
    {
        Collider2D player = CheckForPlayerCollision();

        if (player != null)
            return (transform.position - player.transform.position).magnitude;
        else
            return _attackRange + 1f;
    }

    private Collider2D CheckForPlayerCollision()
    {
        return Physics2D.OverlapCircle(transform.position, _attackRange, _layerMask);
    }
}
