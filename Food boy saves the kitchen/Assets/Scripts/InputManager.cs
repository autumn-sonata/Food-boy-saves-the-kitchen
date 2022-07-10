using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    /*
     * InputManager manages the inputs of the player (WASD), undo, restart
     * and instructs the food manager to move all instances of food.
     */
    private static readonly KeyCode Restart = KeyCode.R; //restart key = "r"
    private static readonly KeyCode Undo = KeyCode.Z; //undo key = "z"
    private static readonly KeyCode Invisible = KeyCode.Tab; //see only tiles = "tab"

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
        return Input.GetKey(Restart);
    }

    public bool KeyPressUndo()
    {
        return Input.GetKey(Undo);
    }

    public bool KeyHoldTab()
    {
        return Input.GetKey(Invisible);
    }
}
