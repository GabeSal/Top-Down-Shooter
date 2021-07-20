using UnityEngine;

public class HandlePlayerImpact : MonoBehaviour
{
    [SerializeField]
    private PooledMonoBehaviour _bloodSplatterParticle;

    internal void EnemyBlast(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * 150, ForceMode2D.Impulse);
    }

    internal void MeleeHit(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * 25, ForceMode2D.Impulse);
    }

    internal void SpawnBloodSplatterParticle(Vector2 point, Vector2 normal)
    {
        var bloodSplatter = _bloodSplatterParticle.Get<PooledMonoBehaviour>(point, Quaternion.LookRotation(normal));
        bloodSplatter.ReturnToPool(1f);

        GetComponent<Rigidbody2D>().AddForce(-normal * 10, ForceMode2D.Impulse);
    }
}
