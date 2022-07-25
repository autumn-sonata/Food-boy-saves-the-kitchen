using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    /* Manages audio for the game.
     * Main menu and level selection screens: Seabed Transcription
     * Lvl 1-20: 8bitPiano
     * Level 20-43: AcGuitar
     */

    public static AudioManager control;
    private int previousSceneIndex; //to check whether to change soundtrack, default 0
    private bool changeTrack;

    [SerializeField] private int Level1BuildIndex;
    private int Level20BuildIndex;
    [SerializeField] private AudioSource BGM;
    [SerializeField] private List<AudioClip> Tracks;

    #region Unity specific functions

    private void Awake()
    {
        /* Make the music persistent throughout the game using a singleton,
         * which is the "control" variable.
         * 
         * Initialisation of variables.
         */

        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        } else if (control != this)
        {
            Destroy(gameObject);
        }

        Level20BuildIndex = Level1BuildIndex + 19;
        //previousSceneIndex initially starts at 0.
        int numScenes = SceneManager.sceneCountInBuildSettings;
        if (Level1BuildIndex > numScenes || Level20BuildIndex > numScenes ||
            Level1BuildIndex < 0 || Level20BuildIndex < 0)
            Debug.LogError("Check Level 0/20 build index is properly initialised.");
    }

    private void Update()
    {
        /* Updates the music soundtrack if the scene changes to the main menu or
         * levels respectively.
         */

        int currScene = SceneManager.GetActiveScene().buildIndex;
        if (currScene != previousSceneIndex)
        {
            changeTrack = true;
            if (currScene < Level1BuildIndex &&
                previousSceneIndex < Level1BuildIndex ||

                currScene >= Level20BuildIndex &&
                previousSceneIndex >= Level20BuildIndex ||

                Level1BuildIndex <= currScene && currScene < Level20BuildIndex &&
                Level1BuildIndex <= previousSceneIndex &&
                previousSceneIndex < Level20BuildIndex
                ) changeTrack = false;

            if (changeTrack)
            {
                //changes track
                BGM.Stop();
                if (currScene < Level1BuildIndex)
                {
                    BGM.clip = Tracks[0];
                }
                else if (currScene >= Level20BuildIndex)
                {
                    BGM.clip = Tracks[2];
                }
                else
                {
                    BGM.clip = Tracks[1];
                }
                BGM.Play();
            }
            previousSceneIndex = currScene;
        }
    }

    #endregion
}
