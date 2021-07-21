using System;
using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyShootingHandler : MonoBehaviour
{
    [SerializeField]
    private float _range = 8f;
    [SerializeField]
    private bool _canShootAndRun;
    [SerializeField]
    private float _timeUntilNextShot = 1.5f;
    [SerializeField] [Range(0, 1.5f)]
    private float _weaponSwayAmount;
    [SerializeField]
    private int _damage = 3;
    [SerializeField]
    private bool _isBurstFire;
    [SerializeField]
    private int _burstSize = 3;
    [SerializeField]
    private float _burstTimeUntilNextShot = 0.08f;


    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private PooledMonoBehaviour _bulletImpactParticle;
    [SerializeField]
    private LineRenderer _bulletTrail;

    private float _shootingTimer = 0;
    private AIPath _aiPath;
    private LayerMask _playerLayerMask;
    private Transform _target;

    public event Action OnFire = delegate { };

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _playerLayerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (_aiPath.reachedDestination || _canShootAndRun)
        {
            _shootingTimer += Time.deltaTime;

            if (_target != null)
                LookAtTarget();

            if (_shootingTimer >= _timeUntilNextShot && _isBurstFire == false)
                ShootPlayer();

            if (_shootingTimer >= _timeUntilNextShot && _isBurstFire)
            {
                StartCoroutine(BurstFire());
            }
        }
        else
        {
            _shootingTimer = 0;
        }
    }

    private IEnumerator BurstFire()
    {
        _shootingTimer = 0;

        for (int i = 0; i < _burstSize; i++)
        {
            ShootPlayer();
            yield return new WaitForSeconds(_burstTimeUntilNextShot);
        }
    }

    private void LookAtTarget()
    {
        Vector2 direction = (_target.position - this.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ShootPlayer()
    {
        _shootingTimer = 0;

        Collider2D player = Physics2D.OverlapCircle(transform.position, _range, _playerLayerMask);

        if (player != null)
        {
            _target = player.transform;
        }
        else
        {
            _target = null;
            return;
        }

        Vector2 shootingDirection = AimAtPlayer(player);

        RaycastHit2D target = Physics2D.Raycast(transform.position, shootingDirection, 100f, _layerMask);
        //Debug.DrawRay(transform.position, shootingDirection * _range, Color.red, 1.5f);

        if (target.collider != null)
        {
            //Debug.Log("Shoot player, they're within range!");
            StartCoroutine(DrawBulletTrail(target));

            if (target.collider.CompareTag("Player"))
            {
                target.collider.GetComponent<HandlePlayerImpact>().SpawnBloodSplatterParticle(target.point, target.normal);
                target.collider.GetComponent<Health>().TakeHit(_damage);
            }
            else
            {
                SpawnBulletRicochetParticle(target.point, target.normal);
            }

            OnFire();
        }
    }

    private IEnumerator DrawBulletTrail(RaycastHit2D target)
    {
        _bulletTrail.enabled = true;

        _bulletTrail.SetPosition(0, _firePoint.position);
        _bulletTrail.SetPosition(1, target.point);

        yield return new WaitForSeconds(0.05f);

        _bulletTrail.enabled = false;
    }

    private void SpawnBulletRicochetParticle(Vector2 point, Vector2 normal)
    {
        var ricochet = _bulletImpactParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(-normal));
        ricochet.ReturnToPool(1f);
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
