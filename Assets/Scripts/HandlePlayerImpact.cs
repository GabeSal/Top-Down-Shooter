using UnityEngine;

public class HandlePlayerImpact : MonoBehaviour
{
    internal void EnemyBlast(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * 150, ForceMode2D.Impulse);
    }
}
