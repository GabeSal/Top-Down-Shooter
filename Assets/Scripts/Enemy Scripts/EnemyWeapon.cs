using UnityEngine;

public class EnemyWeapon : WeaponBase
{
    #region Public Fields
    [Header("Enemy Weapon Settings")]
    [Range(0, 1.5f)]
    public float weaponSwayAmount;
    [Tooltip("Determines if the enemy weapon is fired in bursts rather than semi or fully automatic.")]
    public bool isBurstFire;
    [Range(2, 5)]
    public int shotsPerBurst;
    [Range(0.05f, 0.1f)]
    public float timeUntilNextBurstShot;
    [Tooltip("Determines if the enemy can shoot the player while chasing them.")]
    public bool canShootAndRun;

    [Header("Enemy Weapon Prefabs")]
    public LineRenderer bulletTrail;
    #endregion

    #region Properties
    public int Damage { get => _weaponDamage; }
    public float Range { get => _weaponRange; }
    public float TimeUntilNextShot { get => _fireDelay; }
    public LayerMask CollisionLayers { get => _collisionLayers; }
    public PooledMonoBehaviour ImpactParticle { get => _projectileImpactParticle; }
    #endregion
}
