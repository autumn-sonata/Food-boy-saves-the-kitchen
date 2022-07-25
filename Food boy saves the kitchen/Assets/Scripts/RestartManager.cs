using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartManager : MonoBehaviour
{
    /* Provides functionality for restarting game progress,
     * as if a new game has been started.
     */

    [SerializeField]
    private GameObject RestartUI;

    #region Unity specified functions
    private void Start()
    {
        /* Make UI for asking player to reset progress inactive.
         */

        RestartUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /* Make UI for asking player to reset progress active.
         */

        RestartUI.SetActive(true);
    }

    #endregion

    #region Restart options

    public void DontRestart()
    {
        /* Player does not want to reset progress,
         * then make UI disappear.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        RestartUI.SetActive(false);
    }

    public void DoRestart()
    {
        /* Restart game progress.
         * Removes all save files in the game system.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        GameObject solo = GameObject.Find("Solo");
        if (solo)
        {
            solo.GetComponent<PlayerInfo>().ResetProgress();
        }
        else
        {
            Debug.LogError("Unable to find Audio and Save object, start" +
                "the game from Main Menu!");
        }
        RestartUI.SetActive(false);
    }

    #endregion
}
