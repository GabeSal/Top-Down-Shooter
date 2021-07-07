using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private LayerMask _targetMask;
    [SerializeField]
    private LayerMask _obstacleMask;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine("FindTargetWithDelay", 0.2f);
    }

    private IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }

        float angleInRad = angleInDegrees * Mathf.Deg2Rad;

        return new Vector3(Mathf.Sin(angleInRad), Mathf.Cos(angleInRad), 0f);
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        // Adds target in view radius to a 2d collision array
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, _targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, directionToTarget) < (viewAngle / 2))
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                // If line draw from object is not interrupted by an obstruction, then add the target to list of visible targets
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    visibleTargets.Add(target);
            }
        }
    }
}
