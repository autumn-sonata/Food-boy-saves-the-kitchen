using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartManager : MonoBehaviour
{
    /* Provides functionality for restarting game progress.
     */
    [SerializeField]
    private GameObject RestartUI;

    private void Start()
    {
        RestartUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RestartUI.SetActive(true);
    }

    public void DontRestart()
    {
        RestartUI.SetActive(false);
        //continue on
    }

    public void DoRestart()
    {
        //restart progress.
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
}
