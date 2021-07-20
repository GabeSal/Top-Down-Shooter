using UnityEngine;

public class HandlePlayerImpact : MonoBehaviour
{
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;

    internal void SpawnBloodSplatterParticle(Vector2 point, Vector2 normal)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(normal));
        bloodSplatter.ReturnToPool(1f);
    }
}
