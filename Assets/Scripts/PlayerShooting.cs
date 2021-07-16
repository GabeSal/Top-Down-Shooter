using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private Transform _firePoint;

    private float _weaponRecoilForce;
    private Weapon _weapon;

    private void Start()
    {
        _weapon = GetComponentInChildren<Weapon>();
        _weaponRecoilForce = _weapon.RecoilForce;

        _weapon.OnFire += Weapon_OnFire;
    }

    private void Weapon_OnFire()
    {
        WeaponRecoil(-GetMouseDirection());
    }

    private void WeaponRecoil(Vector3 recoilDirection)
    {
        GetComponent<Rigidbody2D>().AddForce(recoilDirection * _weaponRecoilForce, ForceMode2D.Impulse);
    }

    private void OnDisable()
    {
        _weapon.OnFire -= Weapon_OnFire;
    }

    public Vector3 GetMouseDirection()
    {
        return (GetMousePosition() - _firePoint.position).normalized;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        return mousePosition;
    }
}
