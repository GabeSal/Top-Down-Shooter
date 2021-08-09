using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private GameObject _sceneLoader;
    #endregion

    #region Private Fields
    private GameObject _playerUIHealth;
    private GameObject _playerUIAmmo;
    private GameObject _gameOverUI;
    private GameObject _mainMenuUI;
    private GameObject _pauseMenu;
    private GameObject _controlsUIOverlay;
    private Image _loadingBar;

    private bool _playerIsDead;
    private bool _inputsAllowed;
    private int _enemyCounter;
    #endregion

    #region Action Events
    public event Action OnGameOver;
    #endregion

    #region Properties
    public static bool GameIsPaused;
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

        InitializePauseMenu();
        InitializeLoadingScreen();
        ResetGameSettings();
        FindMainMenuUIElements();
    }
    #endregion

    #region Class Defined Methods

    /// <summary>
    /// Hides the loading screen and sets the fill amount of the loading bar image to 0.
    /// </summary>
    private void InitializeLoadingScreen()
    {
        _sceneLoader.SetActive(false);

        _loadingBar = _sceneLoader.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
        _loadingBar.fillAmount = 0f;
    }

    private void InitializePauseMenu()
    {
        _pauseMenu = GetComponent<PauseMenu>().pauseMenu;

        GetComponent<PauseMenu>().OnGamePaused += GameManager_OnGamePaused;
        GetComponent<PauseMenu>().OnGameResume += GameManager_OnGameResume;
    }

    private void GameManager_OnGamePaused()
    {
        GameIsPaused = true;
    }

    private void GameManager_OnGameResume()
    {
        GameIsPaused = false;
    }

    /// <summary>
    /// Loads the very first scene in the build ("Main Menu")
    /// </summary>
    public void ReturnToMenu()
    {
        if (Time.timeScale < 1f)
        {
            Time.timeScale = 1f;
            _pauseMenu.SetActive(false);
        }

        StartCoroutine(LoadSceneAsyncByName("Main Menu"));
    }

    /// <summary>
    /// Initiates the StartGame() coroutine.
    /// </summary>
    public void Begin()
    {
        StartCoroutine(LoadSceneAsyncByName("Tutorial Level"));
    }

    /// <summary>
    /// Sets the _controlsUIOverlay object to active.
    /// </summary>
    public void ShowControls()
    {
        _controlsUIOverlay.SetActive(true);
    }

    /// <summary>
    /// De-activates/Hides the _controlsUIOverlay object.
    /// </summary>
    public void HideControls()
    {
        _controlsUIOverlay.SetActive(false);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    public bool SceneIsPlayable()
    {
        return SceneManager.GetActiveScene().name != "Main Menu";
    }

    /// <summary>
    /// Coroutine that loads the next scene in the build asynchronously. When the next scene is loaded,
    /// this method will call the SpawnPlayer() method.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSceneAsyncByName(string sceneName)
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(_pauseMenu);
        UnsubscribeFromAllEvents();

        _sceneLoader.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _loadingBar.fillAmount = progress;
            yield return null;
        }

        _sceneLoader.SetActive(false);

        if (SceneIsPlayable())
        {
            ResetGameSettings();
            FindLevelUIElements();
            //FindCheckpoints();
            SubscribeToLevelEndPointEvents();
            SetGameOverClickEvents();
            if (sceneName == "Survival Level" || sceneName == "Tests")
            {
                _enemyCounter = GetAllActiveEnemiesInScene();
            }            
        }
        else
        {
            FindMainMenuUIElements();
            SetMainMenuClickEvents();
        }
    }

    /// <summary>
    /// Set the player to be alive/not dead and inputs are allowed.
    /// </summary>
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
    private void FindLevelUIElements()
    {
        if (_playerUIHealth == null && _playerUIAmmo == null && _gameOverUI == null)
        {
            _playerUIHealth = GetGameObjectFromTransformChildOf((int)EGui.playerHealth);
            _playerUIAmmo = GetGameObjectFromTransformChildOf((int)EGui.playerAmmo);
            _gameOverUI = GetGameObjectFromTransformChildOf((int)EGui.gameOver);

            _playerUIHealth.GetComponent<UIPlayerHealth>().OnPlayerDied += GameManager_OnPlayerDied;
            _gameOverUI.SetActive(false);
        }
    }

    /// <summary>
    /// Finds the endpoint game object and subscribes to the event in which to transition to the next level
    /// when the player interacts with it.
    /// </summary>
    private void SubscribeToLevelEndPointEvents()
    {
        var endpoint = FindObjectOfType<EndPoint>();
        if (endpoint != null)
        {
            endpoint.OnEndPointInteraction += GameManager_OnEndPointInteraction;
        }
        else
        {
            return;
        }        
    }

    private void UnsubscribeFromAllEvents()
    {
        var endPoint = FindObjectOfType<EndPoint>();
        //var checkpoints = FindObjectsOfType<Checkpoint>();

        if (endPoint != null)
        {
            endPoint.OnEndPointInteraction -= GameManager_OnEndPointInteraction;
        }

        //if (checkpoints.length != 0)
        //{
        //    foreach (checkpoint in checkpoints) 
        //    {
        //        checkpoint.OnCheckpointInteraction -= GameManager_OnCheckpointInteraction;
        //    }
        //}

        if (_playerUIHealth != null)
        {
            _playerUIHealth.GetComponent<UIPlayerHealth>().OnPlayerDied -= GameManager_OnPlayerDied;
        }
    }

    /// <summary>
    /// Calls the LoadSceneAsyncByName() method
    /// </summary>
    private void GameManager_OnEndPointInteraction(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncByName(sceneName));
    }

    /// <summary>
    /// Finds and stores the UI elements from the main menu scene into our private UI game object fields.
    /// </summary>
    private void FindMainMenuUIElements()
    {
        var canvases = FindObjectsOfType<Canvas>();

        foreach (var canvas in canvases)
        {
            if (canvas.name == "Main Menu")
                _mainMenuUI = canvas.transform.gameObject;
        }

        _controlsUIOverlay = _mainMenuUI.transform.GetChild(6).gameObject;
        _controlsUIOverlay.SetActive(false);
    }

    /// <summary>
    /// Uses the Unity defined FindObjectOfType() and GetChild() methods to find the appropriate child UI objects from the 
    /// Canvas in the scene.
    /// </summary>
    /// <param name="index">int value of the childs index nested in the Canvas object</param>
    /// <returns>GameObject that represents the UI element we wish to reference throughout this module.</returns>
    private static GameObject GetGameObjectFromTransformChildOf(int index)
    {
        var canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.name == "PlayerUI")
                return canvas.transform.GetChild(index).gameObject;
        }
        return null;
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
    /// Finds the main menu ui buttons and sets their respective click event.
    /// </summary>
    private void SetMainMenuClickEvents()
    {
        Button startButton = GetButtonFromMainMenuCanvas(2);
        Button controlsButton = GetButtonFromMainMenuCanvas(3);
        Button quitButton = GetButtonFromMainMenuCanvas(4);
        Button hideControlsButton = _controlsUIOverlay.transform.GetChild(2).GetComponent<Button>();

        bool allButtonsFound = startButton != null && controlsButton != null && quitButton != null && hideControlsButton != null;

        if (allButtonsFound)
        {
            startButton.onClick.AddListener(Begin);
            controlsButton.onClick.AddListener(ShowControls);
            quitButton.onClick.AddListener(ExitGame);
            hideControlsButton.onClick.AddListener(HideControls);
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
    /// Finds the Unity button object within the main menu canvas at a specified child index and accesses the Button component.
    /// </summary>
    /// <param name="childIndex">int value of the childs index nested in the Main Menu canvas object.</param>
    /// <returns>Button that represents the UI element we wish to delegate onClick events for the 
    /// MainMenu canvas overlay.</returns>
    private Button GetButtonFromMainMenuCanvas(int childIndex)
    {
        return _mainMenuUI.transform.GetChild(childIndex).GetComponent<Button>();
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
