using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PastMovesManager : MonoBehaviour
{
    public static PastMovesManager instance;
    public TMP_Text moveText;

    //int representing number of moves after start of the scene
    [HideInInspector] public int turn;
    [HideInInspector] public int timer;

    //Creates instance
    private void Awake()
    {
        instance = this;
    }

    void Start()    
    {
        moveText.text = "Moves: " + turn.ToString();
        timer = 0;
        turn = -1;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer--;
        }

        DoUndo();
        RestartLevel();
        moveText.text = "Moves: " + turn.ToString();
    }

    //increments moves and updates text
    public void AddTurn()
    {
        turn += 1;
    }



    private void DoUndo()
    {
        if (Input.GetKey("z") && timer == 0 && turn > 0)
        {
            //Note to Eric: hard coding this for simplicity for now, would u prefer I change this to find all gameobjects
            callUndoForObjectWithTag("Tomato");

            turn -= 1;
            timer = 70;
        }

    }

    //restarts level after "r" key
    private void RestartLevel()
    {
        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            turn = 0;
        }
    }

    private void callUndoForObjectWithTag(string Tag)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(Tag);
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy)
            {
                go.GetComponent<PlayerManager>().Undo();
            }
    }

}


