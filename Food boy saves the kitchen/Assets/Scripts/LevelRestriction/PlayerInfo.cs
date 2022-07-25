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

    private void Awake()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        InitialiseLevelProgress();

        if (!LevelRestriction.saveFileExists())
            SaveState();
    }

    private void Update()
    {
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

    private void UnlockNextLvlSet()
    {
        //Special lvl, needs to unlock next level select screen.
        GameObject next = GameObject.FindGameObjectWithTag("Next");
        if (!next) Debug.LogError("Error with finding Next arrow.");

        next.GetComponent<BoxCollider2D>().enabled = true;
        next.GetComponent<DormantSprite>().ChangeToActiveSprite();
    }

    private void InitialiseLevelProgress()
    {
        /* Initialises level progress.
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
         */
        InitialiseLevelProgress();
        SaveState();
    }

    #region SaveLoadMethods

    private void SaveState()
    {
        //Call whenever clear a level.
        LevelRestriction.Save(this);
    }

    private void LoadState()
    {
        //Call only if scene is one of the level selects.
        PlayerData data = LevelRestriction.Load();

        levels = data.getLevelsUnlocked();
        colliders = data.getColliders();
        levelSprite = data.getLevelSprite();
    }

    #endregion

    #region PlayerData methods
    public LevelStatus[] getLevelsUnlocked()
    {
        return levels;
    }

    public bool[] getColliders()
    {
        return colliders;
    }

    public bool[] getLevelSprites()
    {
        return levelSprite;
    }

    public bool[] getNextArrow()
    {
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
