using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private GameObject _playerUIHealth;
    [SerializeField]
    private GameObject _playerUIAmmo;
    [SerializeField]
    private GameObject _gameOverUI;
    #endregion

    #region Private Fields
    private bool _playerIsDead; 
    #endregion

    #region Properties
    public static GameManager Instance { get; private set; }

    public Transform playerPrefab;
    public float playerSpawnDelay = 3f;
    public bool PlayerIsDead { get => _playerIsDead; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        Instance = this;
        _playerIsDead = false;
        _gameOverUI.SetActive(false);
    }
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Coroutine that loads the next scene in the build asynchronously. When the next scene is loaded,
    /// this method will call the SpawnPlayer() method.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        while (operation.isDone == false)
            yield return null;

        //PlayerSingleton.Instance.SpawnPlayer();
    }

    /// <summary>
    /// Initiates the StartGame() coroutine.
    /// </summary>
    public void Begin()
    {
        StartCoroutine(StartGame());
    }

    public void RestartLevel()
    {
        var pools = FindObjectsOfType<Pool>();
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        foreach (var pool in pools)
        {
            DontDestroyOnLoad(pool);
        }        
        SceneManager.LoadScene(currentScene);
    }

    /// <summary>
    /// Disables the player UI elements on the canvas object.
    /// </summary>
    public void PlayerDied()
    {
        _playerIsDead = true;
        _playerUIHealth.SetActive(false);
        _playerUIAmmo.SetActive(false);

        _gameOverUI.SetActive(true);
    }
    #endregion
}
