using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    public void NextLevel()
    {
        /* Loads the next level once a level has been completed.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LevelSelect()
    {
        /* Figure out which level select scene the level select button will return
         * to.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        bool hasScene = false;

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
        /* Quit the game.
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
}
