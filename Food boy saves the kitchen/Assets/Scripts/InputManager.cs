using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    /*
     * InputManager manages the inputs of the player (WASD), undo, restart
     * and instructs the food manager to move all instances of food.
     */
    private static KeyCode restart = KeyCode.R; //restart key = "r"
    private static KeyCode undo = KeyCode.Z; //undo key = "z"

    private int horz;
    private int vert;

    // Update is called once per frame
    void Update()
    {
        horz = (int)Input.GetAxisRaw("Horizontal");
        vert = (int)Input.GetAxisRaw("Vertical");
    }

    public int KeyDirectionHorz()
    {
        return horz;
    }

    public int KeyDirectionVert()
    {
        return vert;
    }

    public bool KeyPressRestart()
    {
        return Input.GetKey(restart);
    }

    public bool KeyPressUndo()
    {
        return Input.GetKey(undo);
    }
}
