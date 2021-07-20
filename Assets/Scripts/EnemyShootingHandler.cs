using UnityEngine;
using Pathfinding;
using System;

public class EnemyShootingHandler : MonoBehaviour
{
    [SerializeField]
    private float _range = 50f;
    [SerializeField]
    private float _timeUntilNextShot = 1.5f;
    [SerializeField] [Range(0, 1.5f)]
    private float _weaponSwayAmount;
    [SerializeField]
    private int _damage = 3;
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private PooledMonoBehaviour _bulletImpactParticle;

    private float _shootingTimer = 0;
    private AIPath _aiPath;
    private LayerMask _playerLayerMask;

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _playerLayerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {

        if (_aiPath.reachedDestination == true)
        {
            _shootingTimer += Time.deltaTime;
            if (_shootingTimer >= _timeUntilNextShot)
            {
                ShootPlayer();
            }
        }
        else
        {
            _shootingTimer = 0;
        }
    }

    private void ShootPlayer()
    {
        _shootingTimer = 0;

        Collider2D player = CheckIfPlayerInRange();

        if (player != null)
        {
            //Debug.Log("Shoot player, they're within range!");
            Vector2 shootingDirection = AimAtPlayer(player);

            RaycastHit2D target = Physics2D.Raycast(transform.position, shootingDirection, _range, _layerMask);
            Debug.DrawRay(transform.position, shootingDirection * 20f, Color.red, 1.5f);

            if (target.collider.CompareTag("Player"))
            {
                target.collider.GetComponent<HandlePlayerImpact>().SpawnBloodSplatterParticle(target.point, target.normal);
                target.collider.GetComponent<Health>().TakeHit(_damage);
            }
            else
            {
                SpawnBulletRicochetParticle(target.point, target.normal);
            }
        }
    }

    private void SpawnBulletRicochetParticle(Vector2 point, Vector2 normal)
    {
        var ricochet = _bulletImpactParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(-normal));
        ricochet.ReturnToPool(1f);
    }

    private Collider2D CheckIfPlayerInRange()
    {
        return Physics2D.OverlapCircle(transform.position, 10f, _playerLayerMask);
    }

    private Vector2 AimAtPlayer(Collider2D player)
    {
        Vector3 weaponSwayOffset = new Vector3(GetValueFromWeaponSway(), GetValueFromWeaponSway(), 0f);

        var aimDirection = (player.transform.position - this.transform.position +
            weaponSwayOffset).normalized;

        return aimDirection;
    }

    private float GetValueFromWeaponSway()
    {
        return UnityEngine.Random.Range(-_weaponSwayAmount, _weaponSwayAmount);
    }
}
