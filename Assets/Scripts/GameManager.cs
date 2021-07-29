using TMPro;
using System;
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
    private bool _inputsAllowed;
    private int _enemyCounter;
    #endregion

    #region Action Events
    public event Action OnGameOver; 
    #endregion

    #region Properties
    public static GameManager Instance { get; private set; }
    public bool PlayerIsDead { get => _playerIsDead; }
    public bool InputsAllowed { get => _inputsAllowed; }
    #endregion

    #region Standard Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        _playerIsDead = false;
        _inputsAllowed = true;
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

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!operation.isDone)
            yield return null;

        if (sceneName != "Main Menu")
        {
            ResetGameSettings();
            FindGameUIElements();
            SetGameOverClickEvents();
            _enemyCounter = GetAllActiveEnemiesInScene();
        }
    }

    private void ResetGameSettings()
    {
        _playerIsDead = false;
        _inputsAllowed = true;
    }

    /// <summary>
    /// Internal method that's called from the UIPlayerHealth object when the player's health reaches 0.
    /// </summary>
    private void GameManager_OnPlayerDied()
    {
        _playerIsDead = true;
        GameOver();
    }

    /// <summary>
    /// Finds all of the game objects in the loaded scene that have the EnemyStatus component and
    /// counts how many are active/enabled in the Unity hierarchy.
    /// </summary>
    /// <returns>Int value that represents how many active enemies are in the current scene.</returns>
    private int GetAllActiveEnemiesInScene()
    {
        var enemiesInScene = FindObjectsOfType<EnemyStatus>();
        int enemyCount = 0;
        foreach (var enemy in enemiesInScene)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                enemyCount++;
            }            
        }

        return enemyCount;
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

            _playerUIHealth.GetComponent<UIPlayerHealth>().OnPlayerDied += GameManager_OnPlayerDied;
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
    private void GameOver()
    {
        OnGameOver?.Invoke();
        _playerUIHealth.SetActive(false);
        _playerUIAmmo.SetActive(false);

        _gameOverUI.SetActive(true);

        var gameOverText = _gameOverUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (_playerIsDead)
            gameOverText.text = "You Died!";
        else
            gameOverText.text = "You Win!";
    }

    /// <summary>
    /// Internal method that's called from EnemyStatus that decreases the _enemyCounter value when the player kills
    /// an enemy in the scene.
    /// </summary>
    internal void EnemyDied()
    {
        _enemyCounter--;

        if(_enemyCounter == 0)
        {
            GameOver();
            _inputsAllowed = false;
        }
    }
    #endregion
}
