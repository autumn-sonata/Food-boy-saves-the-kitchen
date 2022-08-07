using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    #region Unity specific functions
    void Start()
    {
        /* Initialisation of pauseMenu's UI
         */

        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        /* Pauses and resumes the game based on whether
         * the pause menu has been brought up by the 
         * user/player.
         */

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    #endregion

    #region Pausing
    private void Pause()
    {
        /* Pauses the game for the time being.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }
    public void Resume()
    {
        /* Resumes the game when the escape key is not 
         * held.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    public bool isPauseOpen()
    {
        /* Checks whether the pause menu is 
         * active on the screen of the user/player.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if the game is paused, false otherwise.
         */

        return GameIsPaused;
    }

    #endregion

    #region Button functionality

    public void LoadMenu()
    {
        /* Loads the Main menu of the game.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        SceneManager.LoadScene(0);
    }

    public void LoadLevelSelect()
    {
        /* Loads the first level select 
         * screen of the game.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        /* Quits the game.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        Application.Quit();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        /* Sets the game to fullscreen when the fullscreen
         * checkbox is marked.
         * 
         * Parameters
         * ----------
         * 1) isFullscreen: True if the checkbox is marked, 
         *   false otherwise.
         * 
         * Return
         * ------
         * 
         */

        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float volume)
    {
        /* Adjusts the volume of the music in the game based on 
         * the slider within the settings menu.
         * 
         * Parameters
         * ----------
         * 1) volume: Range from 0 - 1, determine how loud the music
         *   should be.
         * 
         * Return
         * ------
         * 
         */

        GameObject solo = GameObject.Find("Solo");
        if (solo)
        {
            solo.GetComponent<AudioSource>().volume = volume;
        } else
        {
            Debug.LogError("Start game from the Main menu!");
        }
    }

    #endregion
}
