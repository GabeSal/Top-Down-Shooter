using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    #region Properties
    public static PlayerSingleton Instance { get; private set; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        Instance = this;
    } 
    #endregion
}
