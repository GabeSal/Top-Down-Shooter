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

    //public float meshResolution;
    //public int edgeResolveIterations;
    //public float edgeDistanceThreshold;

    //public MeshFilter viewMeshFilter;
    //private Mesh _viewMesh;

    private void Start()
    {
        //_viewMesh = new Mesh();
        //_viewMesh.name = "View Mesh";
        //viewMeshFilter.mesh = _viewMesh;

        StartCoroutine("FindTargetWithDelay", 0.2f);
    }

    //private void LateUpdate()
    //{
    //    DrawFieldOfView();
    //}

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
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    //private void DrawFieldOfView()
    //{
    //    int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
    //    float stepAngleSize = viewAngle / stepCount;

    //    List<Vector3> viewPoints = new List<Vector3>();
    //    ViewCastInfo oldViewCast = new ViewCastInfo();

    //    for (int i = 0; i <= stepCount; i++)
    //    {
    //        float angle = -transform.eulerAngles.z - (viewAngle / 2f) + (stepAngleSize * i);
    //        ViewCastInfo newViewCast = ViewCast(angle);

    //        if (i > 0)
    //        {
    //            bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

    //            if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
    //            {
    //                EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
    //                if (edge.pointA != Vector3.zero)
    //                    viewPoints.Add(edge.pointA);

    //                if (edge.pointB != Vector3.zero)
    //                    viewPoints.Add(edge.pointB);

    //            }
    //        }

    //        viewPoints.Add(newViewCast.point);
    //        oldViewCast = newViewCast;

    //        //Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(angle, true) * viewRadius, Color.white);
    //    }

    //    int vertexCount = viewPoints.Count + 1;
    //    Vector3[] vertices = new Vector3[vertexCount];
    //    int[] triangles = new int[(vertexCount - 2) * 3];

    //    vertices[0] = Vector3.zero;
    //    for (int i = 0; i < (vertexCount - 1); i++)
    //    {
    //        vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

    //        if (i < (vertexCount - 2))
    //        {
    //            triangles[i * 3] = 0;
    //            triangles[i * 3 + 1] = i + 1;
    //            triangles[i * 3 + 2] = i + 2;
    //        }            
    //    }

    //    _viewMesh.Clear();
    //    _viewMesh.vertices = vertices;
    //    _viewMesh.triangles = triangles;
    //    _viewMesh.RecalculateNormals();
    //}

    //private ViewCastInfo ViewCast(float globalAngle)
    //{
    //    Vector3 direction = DirectionFromAngle(globalAngle, true);
    //    RaycastHit2D hit2D = Physics2D.Raycast(transform.position, direction, viewRadius, _obstacleMask);

    //    if (hit2D.collider != null)
    //    {
    //        return new ViewCastInfo(true, hit2D.point, hit2D.distance, globalAngle);
    //    }
    //    else
    //    {
    //        return new ViewCastInfo(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
    //    }
    //}

    //private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    //{
    //    float minAngle = minViewCast.angle;
    //    float maxAngle = maxViewCast.angle;

    //    Vector3 minPoint = Vector3.zero;
    //    Vector3 maxPoint = Vector3.zero;

    //    for (int i = 0; i < edgeResolveIterations; i++)
    //    {
    //        float currentAngle = (minAngle + maxAngle) / 2f;
    //        ViewCastInfo newViewCast = ViewCast(currentAngle);

    //        bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
    //        if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
    //        {
    //            minAngle = currentAngle;
    //            minPoint = newViewCast.point;
    //        }
    //        else
    //        {
    //            maxAngle = currentAngle;
    //            maxPoint = newViewCast.point;
    //        }
    //    }

    //    return new EdgeInfo(minPoint, maxPoint);
    //}

    //public struct ViewCastInfo
    //{
    //    public bool hit;
    //    public Vector3 point;
    //    public float distance;
    //    public float angle;

    //    public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
    //    {
    //        hit = _hit;
    //        point = _point;
    //        distance = _distance;
    //        angle = _angle;
    //    }
    //}

    //public struct EdgeInfo
    //{
    //    public Vector3 pointA;
    //    public Vector3 pointB;

    //    public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
    //    {
    //        pointA = _pointA;
    //        pointB = _pointB;
    //    }
    //}
}
