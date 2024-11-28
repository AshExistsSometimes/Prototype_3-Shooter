using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{

    public PlayerStats player;
    public PlayerController playerController;

    public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;

    public bool IsGamePaused = false;

    // Start is called before the first frame update
    void Start()
    {
        IsGamePaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGamePaused || player.PlayerIsDead)
        {
            playerController.CanMoveAndRotate = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else if (!IsGamePaused || !player.PlayerIsDead)
        {
            playerController.CanMoveAndRotate = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }

        GamePauseLogic();
    }

    public void ResumeGame()
    {
        OnGameResume.Invoke();
        IsGamePaused = false;
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void GamePauseLogic()
    {
        if (IsGamePaused && Input.GetKeyDown(KeyCode.Escape))
        {
            IsGamePaused = false;
            OnGameResume.Invoke();
        }
        else if (!IsGamePaused && Input.GetKeyDown(KeyCode.Escape))
        {
            OnGamePause.Invoke();
            IsGamePaused = true;
        }
    }


}
