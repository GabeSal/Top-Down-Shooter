using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    #region Public Fields
    public GameObject pauseMenu;
    #endregion

    #region Action Events
    public event Action OnGamePaused;
    public event Action OnGameResume;
    #endregion

    #region Standard Unity Methods

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown((KeyCode)PlayerControls.pauseGame) && GameManager.Instance.SceneIsPlayable())
        {
            if (GameManager.GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    #endregion

    #region Class Defined Methods
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        OnGameResume?.Invoke();
    }
    #endregion
}
