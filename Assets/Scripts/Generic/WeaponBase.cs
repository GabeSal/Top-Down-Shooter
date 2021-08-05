using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBase : MonoBehaviour
{
    #region Serialized Fields
    [Header("Weapon Settings")]
    [SerializeField]
    [Range(1, 15)]
    protected int _weaponDamage;
    [SerializeField]
    [Range(40f, 1000f)]
    protected float _weaponRange;
    [SerializeField]
    [Range(0.04f, 2f)]
    protected float _fireDelay;
    [SerializeField]
    protected LayerMask _collisionLayers;

    [Header("Weapon Projectile Prefabs")]
    [SerializeField]
    protected PooledMonoBehaviour _projectile;
    [SerializeField]
    protected PooledMonoBehaviour _projectileImpactParticle;
    #endregion

    #region Protected Fields
    protected float _fireTimer;
    #endregion

    #region Public Field
    public Image weaponUIImage;
    #endregion
}
