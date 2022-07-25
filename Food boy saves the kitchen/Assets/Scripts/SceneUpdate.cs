using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUpdate : MonoBehaviour
{
    public int Level;

    #region Button functions

    public void MainMenuButton()
    {
        //Load main menu
        SceneManager.LoadScene(0);
    }

    public void SettingsButton()
    {
        //Load settings page
        SceneManager.LoadScene(1);
    }
    public void LevelSelectionButton()
    {
        //Load level selection screen
        SceneManager.LoadScene(2);
    }
    public void LevelButton()
    {
        //Load level from level selection
        SceneManager.LoadScene(Level + 2);
    }
    public void NextLevelButton()
    {
        //Load next level after this current level.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}
