using System;
using UnityEngine;

public class HandlePlayerImpact : MonoBehaviour
{
    internal void EnemyBlast(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * 150, ForceMode2D.Impulse);
    }

    internal void MeleeHit(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * 25, ForceMode2D.Impulse);
    }
}
