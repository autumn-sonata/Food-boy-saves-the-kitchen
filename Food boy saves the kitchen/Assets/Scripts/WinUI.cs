using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LevelSelect()
    {
        bool hasScene = false;
        //Figure out which level select scene to load into.
        int currLvlNum = SceneManager.GetActiveScene().buildIndex - 5;
        if (currLvlNum < 0)
        {
            hasScene = true;
            SceneManager.LoadScene(2); //tutorial lvl
        }
        List<int> firstLvls = NextLevelManager.specialUnlockedLvls;
        for (int i = 0; i < firstLvls.Count; i++)
        {
            int firstLvl = firstLvls[i];
            if (firstLvl > currLvlNum && !hasScene)
            {
                hasScene = true;
                SceneManager.LoadScene(i + 1);
            }
        }

        //last level select scene
        if (!hasScene) SceneManager.LoadScene(firstLvls.Count + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
