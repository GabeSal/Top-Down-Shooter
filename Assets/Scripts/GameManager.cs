using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields
    #endregion

    #region Private Fields
    private GameObject _playerUIHealth;
    private GameObject _playerUIAmmo;
    private GameObject _gameOverUI;

    private bool _playerIsDead;
    #endregion

    #region Properties
    public static GameManager Instance { get; private set; }
    public bool PlayerIsDead { get => _playerIsDead; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        Instance = this;
        _playerIsDead = false;
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Initiates the StartGame() coroutine.
    /// </summary>
    public void Begin()
    {
        StartCoroutine(LoadSceneAsyncByName("Tests"));
    }

    /// <summary>
    /// Coroutine that loads the next scene in the build asynchronously. When the next scene is loaded,
    /// this method will call the SpawnPlayer() method.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSceneAsyncByName(string sceneName)
    {
        DontDestroyOnLoad(this);
        _playerIsDead = false;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!operation.isDone)
            yield return null;

        if (sceneName != "Main Menu")
        {
            FindGameUIElements();
            SetGameOverClickEvents();
        }        
    }

    /// <summary>
    /// Finds and stores the UI elements from the playable game scene into our private UI game object fields.
    /// </summary>
    private void FindGameUIElements()
    {
        if (_playerUIHealth == null && _playerUIAmmo == null && _gameOverUI == null)
        {
            _playerUIHealth = GetGameObjectFromTransformChildIndex(0);
            _playerUIAmmo = GetGameObjectFromTransformChildIndex(1);
            _gameOverUI = GetGameObjectFromTransformChildIndex(2);

            _gameOverUI.SetActive(false);
        }
    }

    /// <summary>
    /// Uses the Unity defined FindObjectOfType() and GetChild() methods to find the appropriate child UI objects from the 
    /// Canvas in the scene.
    /// </summary>
    /// <param name="index">int value of the childs index nested in the Canvas object</param>
    /// <returns>GameObject that represents the UI element we wish to reference throughout this module.</returns>
    private static GameObject GetGameObjectFromTransformChildIndex(int index)
    {
        return FindObjectOfType<Canvas>().transform.GetChild(index).gameObject;
    }

    /// <summary>
    /// Finds the game over ui buttons and sets their respective click event.
    /// </summary>
    public void SetGameOverClickEvents()
    {
        Button retryButton = GetButtonFromGameOverUIChildren(1);
        Button menuButton = GetButtonFromGameOverUIChildren(2);

        if (retryButton != null && menuButton != null)
        {
            retryButton.onClick.AddListener(RestartLevel);
            menuButton.onClick.AddListener(ReturnToMenu);
        }
    }

    /// <summary>
    /// Finds the Unity button object at a specified child index and accesses the Button component.
    /// </summary>
    /// <param name="childIndex">int value of the childs index nested in the GameOverUI object</param>
    /// <returns>Button that represents the UI element we wish to delegate onClick events for the GameOverUI overlay.</returns>
    private Button GetButtonFromGameOverUIChildren(int childIndex)
    {
        return _gameOverUI.transform.GetChild(childIndex).GetComponent<Button>();
    }

    /// <summary>
    /// Reloads the currently played scene and does not destroy the pools for the pooled objects in the scene
    /// (i.e. bullet ricochets, blood splatters, etc.)
    /// </summary>
    private void RestartLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsyncByName(currentScene));
    }

    /// <summary>
    /// Loads the very first scene in the build ("Main Menu")
    /// </summary>
    private void ReturnToMenu()
    {
        StartCoroutine(LoadSceneAsyncByName("Main Menu"));
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
