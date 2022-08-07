using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PastMovesManager : MonoBehaviour
{
    private static string prefix = "Moves: ";
    private const float UndoCooldownDuration = 0.3f;

    public TMP_Text moveText;

    private int turn; //int representing number of moves after start of the scene
    private bool canUndo; //cooldown before another key from undo/restart can be activated
    private List<PastMovesRecords> pastRecords; //remembers all components of PastMovesRecords.

    #region Unity specific functions

    private void Awake()    
    {
        /* Initialises turn text and pastrecords list.
         */

        moveText.text = prefix + turn.ToString();
        turn = -1;
        canUndo = true;
        pastRecords = new List<PastMovesRecords>();
    }

    private void Start()
    {
        /* Gets all instances of PastMoveRecords (which means that 
         * these items will revert to previous state once undo button
         * has been pressed).
         */

        GameObject[] movableObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in movableObjects)
        {
            if (obj.GetComponent<PastMovesRecords>()) 
                pastRecords.Add(obj.GetComponent<PastMovesRecords>());
        }
    }

    #endregion

    #region Record

    public int getTurn()
    {
        /* Gets turn number of current movement.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * int: turn number of current turn
         */

        return turn;
    }

    public void RecordThisMove()
    {
        /* A turn has passed. Record the move
         * down so that undo may use it at a later time.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        turn++;
        foreach (PastMovesRecords record in pastRecords)
        {
            record.RecordMove();
        }
        changeMoveText();
    }

    private void changeMoveText()
    {
        /* Changes the Moves count on screen.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        moveText.text = prefix + turn.ToString();
    }

    #endregion

    #region Restart
    public void RestartLevel()
    {
        /* Restarts the level by loading the exact same scene anew.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Undo

    public void DoUndo()
    {
        /* The "Z" key has been pressed by the player, 
         * execute the undo function for all objects which
         * have a past move history.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (canUndo && turn > 0)
        {
            canUndo = false;
            turn--;
            callUndoForObjectWithTag();
            changeMoveText();
            StartCoroutine(undoCooldownTimer());
        }
    }

    private void callUndoForObjectWithTag()
    {
        /* Handles undo of all player objects.
         * https://answers.unity.com/questions/454590/c-how-to-check-if-a-gameobject-contains-a-certain.html
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (PastMovesRecords component in pastRecords)
        {
            component.Undo();
        }
    }

    private IEnumerator undoCooldownTimer()
    {
        /* Waits for undoCooldownDuration seconds before being able to undo again
         * https://foxxthom.medium.com/making-a-simple-and-efficient-cool-down-playerTimer-in-unity-137efcbb8dce
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * IEnumerator: Coroutine return.
         */
        yield return new WaitForSeconds(UndoCooldownDuration);
        canUndo = true;
    }

    #endregion
}


