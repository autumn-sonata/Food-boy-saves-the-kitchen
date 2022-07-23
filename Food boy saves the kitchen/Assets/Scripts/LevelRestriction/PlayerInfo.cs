using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    /* Functionality for providing player progression 
     * and save states.
     */

    [SerializeField]
    private GameObject[] levelObj;
    private LevelStatus[] levels = new LevelStatus[43];

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
        /* Call to finish a level.
         * 
         * Parameters
         * ----------
         * 1) int completedLvlNum: The level number to be completed.
         */
        if (levels[completedLvlNum - 1] != LevelStatus.Unlocked)
            Debug.LogError("Level " + completedLvlNum + " has not been unlocked yet!");
        levels[completedLvlNum - 1] = LevelStatus.Completed; //due to 0-indexing
    }
}
