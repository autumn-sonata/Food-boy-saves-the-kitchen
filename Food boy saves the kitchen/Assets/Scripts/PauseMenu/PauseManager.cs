using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropDown;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> options = new();

        int currResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution reso = resolutions[i];
            options.Add(reso.width + " x " + reso.height);

            if (reso.width == Screen.currentResolution.width &&
                reso.height == Screen.currentResolution.height)
            {
                currResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float volume)
    {
        GameObject.Find("Solo").GetComponent<AudioSource>().volume = volume;
    }

    public bool isPauseOpen()
    {
        return GameIsPaused;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
