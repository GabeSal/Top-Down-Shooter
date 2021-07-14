using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Health>().OnTookHit += EnemyStatus_OnTookHit;
        GetComponent<Health>().OnDied += EnemyStatus_OnDied;
    }

    private void EnemyStatus_OnTookHit()
    {
        //RecoilFromHit();
    }

    private void RecoilFromHit()
    {
        
    }

    private void EnemyStatus_OnDied()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnTookHit -= EnemyStatus_OnTookHit;
        GetComponent<Health>().OnDied -= EnemyStatus_OnDied;
    }
}
