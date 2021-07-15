using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _recoilForce = 2f;

    public float RecoilForce { get; private set; }

    private void Start()
    {
        RecoilForce = _recoilForce;
    }
}
