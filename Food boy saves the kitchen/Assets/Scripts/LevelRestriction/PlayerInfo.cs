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

    public void unlockedLvl(int unlockedLvlNum)
    {
        /* Call to unlock a level.
         * 
         * Parameters
         * ----------
         * 1) int unlockedLvlNum: The level number to be unlocked.
         */
        if (levels[unlockedLvlNum - 1] != LevelStatus.Locked)
            Debug.LogError("Level " + unlockedLvlNum + " is not even locked!");
        levels[unlockedLvlNum - 1] = LevelStatus.Unlocked;
    }
}
