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

    private void Awake()    
    {
        moveText.text = prefix + turn.ToString();
        turn = -1;
        canUndo = true;
        pastRecords = new List<PastMovesRecords>();
    }

    private void Start()
    {
        GameObject[] movableObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in movableObjects)
        {
            if (obj.GetComponent<PastMovesRecords>()) 
                pastRecords.Add(obj.GetComponent<PastMovesRecords>());
        }
    }

    public int getTurn()
    {
        return turn;
    }

    public void RecordThisMove()
    {
        /* A turn has passed.
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
         */
        moveText.text = prefix + turn.ToString();
    }

    public void DoUndo()
    {
        if (canUndo && turn > 0)
        {
            canUndo = false;
            turn--;
            callUndoForObjectWithTag();
            changeMoveText();
            StartCoroutine(undoCooldownTimer());
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void callUndoForObjectWithTag()
    {
        /* Handles undo of all player objects.
         * https://answers.unity.com/questions/454590/c-how-to-check-if-a-gameobject-contains-a-certain.html
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
         */
        yield return new WaitForSeconds(UndoCooldownDuration);
        canUndo = true;
    }
}


