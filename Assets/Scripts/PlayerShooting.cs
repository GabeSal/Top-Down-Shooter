using UnityEngine;
using System.Collections;
using System;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _weaponDamage = 1;

    private int _mouseLeftClick = 0;
    private float _weaponRecoilForce;

    private void Start()
    {
        _weaponRecoilForce = GetComponentInChildren<Weapon>().RecoilForce;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(_mouseLeftClick))
        {
            StartCoroutine(ShootWeapon());
        }
    }

    private IEnumerator ShootWeapon()
    {
        RaycastHit2D hitInfo2D = Physics2D.Raycast(_firePoint.position, GetMouseDirection());
        Collider2D something = hitInfo2D.collider;

        WeaponRecoil(-GetMouseDirection());

        _lineRenderer.enabled = true;

        if (something != null)
        {
            //Debug.Log("Something was hit at point: " + hitInfo2D.point);
            //Debug.Log("Normal of the hit point is: " + hitInfo2D.normal);

            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, hitInfo2D.point);

            if (IsEnemy(something))
            {
                something.GetComponent<Health>().TakeHit(_weaponDamage);
                something.GetComponent<EnemyStatus>().RecoilFromHit(-hitInfo2D.normal);
            }
        }
        else
        {
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, GetMousePosition());

            //Debug.Log("Nothing was detected.");
        }

        yield return new WaitForSeconds(0.05f);

        _lineRenderer.enabled = false;
    }

    private void WeaponRecoil(Vector3 recoilDirection)
    {
        GetComponent<Rigidbody2D>().AddForce(recoilDirection * _weaponRecoilForce, ForceMode2D.Impulse);
    }

    private bool IsEnemy(Collider2D game_object)
    {
        return game_object.GetComponent<Health>() != null;
    }

    private Vector3 GetMouseDirection()
    {
        return (GetMousePosition() - _firePoint.position).normalized;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        return mousePosition;
    }
}
