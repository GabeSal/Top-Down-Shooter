using System;
using UnityEngine;

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
    [SerializeField]
    [Tooltip("Used to specify the origin of which to draw the raycast for the weapon.")]
    protected Transform _firePoint;

    [Header("Weapon Projectile Prefabs")]
    [SerializeField]
    protected PooledMonoBehaviour _projectile;
    [SerializeField]
    protected PooledMonoBehaviour _projectileImpactParticle;

    [SerializeField]
    [Tooltip("Assign the key to be pressed to select the weapon component defined in the editor.")]
    protected KeyCode _weaponHotKey;

    #endregion

    #region Protected Fields
    protected float _fireTimer;
    protected PlayerShooting _playerShooting;
    protected WeaponAmmo _weaponAmmo;
    #endregion

    #region Properties
    public KeyCode WeaponHotKey { get => _weaponHotKey; }
    #endregion

    #region Standard Unity Methods
    protected virtual void Awake()
    {
        _weaponAmmo = GetComponent<WeaponAmmo>();
        _playerShooting = GetComponentInParent<PlayerShooting>();
    }
    #endregion
}
