using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    /* Functionality for providing player progression 
     * and save states.
     */

    [SerializeField]
    private static GameObject[] levelObj;
    private static LevelStatus[] levels = new LevelStatus[43];

    public void SaveState()
    {
        LevelRestriction.Save(this);
    }

    public void LoadState()
    {
        PlayerData data = LevelRestriction.Load();

        levels = data.getLevelsUnlocked();
    }

    public LevelStatus[] getLevelsUnlocked()
    {
        return levels;
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

                //enable collider2D and change sprite
                levelObj[lvl - 1].GetComponent<BoxCollider2D>().enabled = true;
                levelObj[lvl - 1].GetComponent<SpriteRenderer>().sprite =
                    levelObj[lvl - 1].GetComponent<DormantSprite>().GetActiveSprite();

            } else if (levels[lvl - 1] != LevelStatus.Locked)
            {
                Debug.LogError("Level " + lvl + " is not even locked!");
            } else
            {
                levels[lvl - 1] = LevelStatus.Unlocked;
            }
        }
    }
}
