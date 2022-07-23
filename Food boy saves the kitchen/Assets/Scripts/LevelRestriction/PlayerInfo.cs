using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    /* Functionality for providing player progression 
     * and save states.
     */

    private static GameObject[] levelObj;
    private static LevelStatus[] levels = new LevelStatus[43];

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private void LoadLevelObj()
    {
        if (levelObj == null)
        {
            List<GameObject> res = new();
            //Initialise all levelObj gameObjects.
            foreach (int buildIndex in NextLevelManager.levelSelect)
            {
                GameObject[] objs =
                    SceneManager.GetSceneByBuildIndex(buildIndex).GetRootGameObjects();
                foreach (GameObject obj in objs)
                {
                    if (obj.CompareTag("Level")) res.Add(obj);
                }
            }

            //sort res array and give to levelObj.
            levelObj = res.OrderBy(o => o.GetComponent<SceneUpdateTile>().nextScene)
                .ToArray();
        }
    }

    private void SaveState()
    {
        //Call whenever clear a level.
        LevelRestriction.Save(this);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void LoadState()
    {
        //Call whenever scene changes.
        //Load data into PlayerInfo.
        PlayerData data = LevelRestriction.Load();
        levelObj = data.getLevelObj();
        levels = data.getLevelsUnlocked();
    }

    public LevelStatus[] getLevelsUnlocked()
    {
        return levels;
    }

    public GameObject[] getLevelObj()
    {
        return levelObj;
    }

    public void completedLvl(int completedLvlNum)
    {
        /* Call to finish a level. Unlocks subsequent levels as well.
         * API to interact with WinManager.
         * 
         * 1) Mark this level as completed
         * 2) Unlock new levels
         * 
         * Parameters
         * ----------
         * 1) int completedLvlNum: The level number to be completed.
         */
        if (levels[completedLvlNum - 1] != LevelStatus.Unlocked)
            Debug.LogError("Level " + completedLvlNum + " has not been unlocked yet!");
        levels[completedLvlNum - 1] = LevelStatus.Completed; //due to 0-indexing

        //Unlocks next levels.
        List<int> nextLvls = NextLevelManager.nextLvl[completedLvlNum];
        foreach (int lvl in nextLvls)
        {
            if (lvl == -1)
            {
                //Special lvl, needs to unlock next level select screen.
                GameObject next = GameObject.FindGameObjectWithTag("Next");
                if (!next) Debug.LogError("Error with finding Next arrow.");

                next.GetComponent<BoxCollider2D>().enabled = true;
                next.GetComponent<SpriteRenderer>().sprite =
                    next.GetComponent<DormantSprite>().GetActiveSprite();

            } else if (levels[lvl - 1] != LevelStatus.Locked)
            {
                Debug.LogError("Level " + lvl + " is not even locked!");
            } else
            {
                levels[lvl - 1] = LevelStatus.Unlocked;
                //enable collider2D and change sprite
                levelObj[lvl - 1].GetComponent<BoxCollider2D>().enabled = true;
                levelObj[lvl - 1].GetComponent<SpriteRenderer>().sprite =
                    levelObj[lvl - 1].GetComponent<DormantSprite>().GetActiveSprite();
            }
        }

        //Add save state.
        SaveState();
    }
}
