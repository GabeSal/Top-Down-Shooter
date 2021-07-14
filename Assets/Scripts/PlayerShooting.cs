using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _weaponDamage = 1;

    private int _mouseLeftClick = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(_mouseLeftClick))
        {
            StartCoroutine(ShootWeapon());
        }
    }

    private IEnumerator ShootWeapon()
    {
        RaycastHit2D hitInfo2D = Physics2D.Raycast(_firePoint.position, GetMousePosition());

        Collider2D something = hitInfo2D.collider;

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
            _lineRenderer.SetPosition(1, _firePoint.position + _firePoint.up * 50f);
        }        

        yield return new WaitForSeconds(0.05f);

        _lineRenderer.enabled = false;
    }

    private bool IsEnemy(Collider2D game_object)
    {
        return game_object.GetComponent<Health>() != null;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 firedFromPosition = _firePoint.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (mousePosition - firedFromPosition).normalized;
    }
}
