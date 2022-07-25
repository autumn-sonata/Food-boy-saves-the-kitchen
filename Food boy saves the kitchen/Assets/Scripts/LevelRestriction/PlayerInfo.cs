using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    /* Functionality for providing player progression 
     * and load/save states.
     * 
     * Limits the number of levels that the player has 
     * access to so that the player will not be overwhelmed
     * with the different kinds of game mechanics.
     */

    private static readonly int numLvls = 43;
    [SerializeField]
    private LevelStatus[] levels;
    [SerializeField]
    private bool[] colliders;
    [SerializeField]
    private bool[] levelSprite;
    [SerializeField]
    private bool[] nextArrow;
    private static readonly KeyValuePair<int, int> levelSelIndex = new(2, 5);
    private int sceneIndex;

    #region Unity specific functions

    private void Awake()
    {
        /* Initialise current progress to be completely fresh.
         * Aka new game.
         */

        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        InitialiseLevelProgress();

        if (!LevelRestriction.saveFileExists())
            SaveState();
    }

    private void Update()
    {
        /* Driver function for reading the level status information and
         * translating it into actually unlocking levels in Unity.
         */

        int currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currSceneIndex != sceneIndex)
        {
            sceneIndex = currSceneIndex;

            //runs only when scene changes.
            if (currSceneIndex >= levelSelIndex.Key &&
                currSceneIndex <= levelSelIndex.Value)
            {
                LoadState();
                if (currSceneIndex - 2 < nextArrow.Count() && nextArrow[currSceneIndex - 2])
                    UnlockNextLvlSet();
                //Update based on load
                GameObject[] GOlvls = SceneManager.GetActiveScene().GetRootGameObjects();
                GOlvls = GOlvls.ToList()
                    .FindAll(obj => obj.CompareTag("Level"))
                    .OrderBy(obj => obj.GetComponent<SceneUpdateTile>().nextScene)
                    .ToArray();

                int startLvlCurrScene = NextLevelManager.specialUnlockedLvls[currSceneIndex - 2];

                for (int i = 0; i < GOlvls.Length; i++)
                {
                    //each lvl, update based on colliders and levelsprite.
                    GOlvls[i].GetComponent<BoxCollider2D>().enabled = colliders[i + startLvlCurrScene - 1];
                    if (levelSprite[i + startLvlCurrScene - 1])
                        GOlvls[i].GetComponent<DormantSprite>().ChangeToActiveSprite();
                }
            }
        }
    }

    #endregion

    #region Helper functions
    private void UnlockNextLvlSet()
    {
        /* Function for special levels. When this function is run,
         * the next level select screen will be open and accessible
         * to the player.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        GameObject next = GameObject.FindGameObjectWithTag("Next");
        if (!next) Debug.LogError("Error with finding Next arrow.");

        next.GetComponent<BoxCollider2D>().enabled = true;
        next.GetComponent<DormantSprite>().ChangeToActiveSprite();
    }

    private void InitialiseLevelProgress()
    {
        /* Initialises level progress, as if the game has been started
         * anew.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        levels = new LevelStatus[numLvls];
        colliders = new bool[numLvls];
        levelSprite = new bool[numLvls];
        nextArrow = new bool[3];
        foreach (int specialLvl in NextLevelManager.specialUnlockedLvls)
        {
            levels[specialLvl - 1] = LevelStatus.Unlocked;
            colliders[specialLvl - 1] = true;
            levelSprite[specialLvl - 1] = true;
        }
    }

    public void ResetProgress()
    {
        /* Resets the level progression.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        InitialiseLevelProgress();
        SaveState();
    }

    #endregion

    #region SaveLoadMethods

    private void SaveState()
    {
        /* Function to be called whenever 
         * the player clears a level, to update the information
         * in the system.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        LevelRestriction.Save(this);
    }

    private void LoadState()
    {
        /* Function to be called whenever the level
         * information needs to be retrieved from 
         * the binary file.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        PlayerData data = LevelRestriction.Load();

        levels = data.getLevelsUnlocked();
        colliders = data.getColliders();
        levelSprite = data.getLevelSprite();
    }

    #endregion

    #region PlayerData methods
    public LevelStatus[] getLevelsUnlocked()
    {
        /* Method to store level status information 
         * into PlayerData.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * LevelStatus[]: The status of each level in the game.
         */
        return levels;
    }

    public bool[] getColliders()
    {
        /* Method to store current collider information
         * into PlayerData.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool[]: Whether the colliders have been enabled
         *   or disabled.
         */

        return colliders;
    }

    public bool[] getLevelSprites()
    {
        /* Method to store current level sprite information
         * into PlayerData.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool[]: Whether the numbers for the levels has been
         *   lighted up or not.
         */

        return levelSprite;
    }

    public bool[] getNextArrow()
    {
        /* Method to store information on whether the next
         * set of levels has been unlocked or not.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool[]: Whether the next set of scenes has been unlocked
         *   or not.
         */

        return nextArrow;
    }

    #endregion

    #region WinManager methods
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
         * 
         * Return
         * ------
         * 
         */

        if (levels[completedLvlNum - 1] == LevelStatus.Locked)
            Debug.LogError("Level " + completedLvlNum + " has not been unlocked yet!");
        levels[completedLvlNum - 1] = LevelStatus.Completed;

        //Unlocks next levels.
        List<int> nextLvls = NextLevelManager.nextLvl[completedLvlNum];
        foreach (int lvl in nextLvls)
        {
            if (lvl == -1)
            {
                //hit next section.
                for (int i = 0; i < nextArrow.Count(); i++)
                {
                    if (!nextArrow[i])
                    {
                        nextArrow[i] = true;
                        break;
                    }
                }
            }
            else if (levels[lvl - 1] != LevelStatus.Locked)
            {
                Debug.LogError("Level " + lvl + " is not even locked!");
            }
            else
            {
                levels[lvl - 1] = LevelStatus.Unlocked;
                //enable collider2D and change sprite
                colliders[lvl - 1] = true;
                levelSprite[lvl - 1] = true;
            }
        }

        //Add save state.
        SaveState();
    }
    #endregion
}
