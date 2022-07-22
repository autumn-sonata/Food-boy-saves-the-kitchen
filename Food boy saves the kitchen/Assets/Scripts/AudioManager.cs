using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    /* Manages audio for the game.
     * Main menu and level selection screens: Track 0
     * Lvl 1-20: Track 1
     * Level 20-??: Track 2
     */
    public static AudioManager control;
    private int previousSceneIndex; //to check whether to change soundtrack
    private bool changeTrack;
    [SerializeField] private int Level0BuildIndex;
    private int Level20BuildIndex;
    [SerializeField] private AudioSource BGM;
    [SerializeField] private List<AudioClip> Tracks;

    private void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        } else if (control != this)
        {
            Destroy(gameObject);
        }

        Level20BuildIndex = Level0BuildIndex + 20;
        //previousSceneIndex initially starts at 0.
        int numScenes = SceneManager.sceneCountInBuildSettings;
        if (Level0BuildIndex > numScenes || Level20BuildIndex > numScenes ||
            Level0BuildIndex < 0 || Level20BuildIndex < 0)
            Debug.LogError("Check Level 0/20 build index is properly initialised.");
    }

    private void Update()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;
        if (currScene != previousSceneIndex)
        {
            changeTrack = true;
            if (currScene < Level0BuildIndex &&
                previousSceneIndex < Level0BuildIndex ||

                currScene >= Level20BuildIndex &&
                previousSceneIndex >= Level20BuildIndex ||

                Level0BuildIndex <= currScene && currScene < Level20BuildIndex &&
                Level0BuildIndex <= previousSceneIndex &&
                previousSceneIndex < Level20BuildIndex
                ) changeTrack = false;

            if (changeTrack)
            {
                //changes track
                BGM.Stop();
                if (currScene < Level0BuildIndex)
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
}
