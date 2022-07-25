using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    /* InputManager manages the inputs of the player (WASD), undo, restart
     * and instructs the food manager to move all instances of food.
     */

    private static readonly KeyCode Restart = KeyCode.R; //restart key = "r"
    private static readonly KeyCode Undo = KeyCode.Z; //undo key = "z"
    private static readonly KeyCode Invisible = KeyCode.Tab; //see only tiles = "tab"

    private int horz;
    private int vert;

    #region Unity specific functions

    void Update()
    {
        /* Get the user input for the up/down/left/right keys
         * or WASD keys.
         */

        horz = (int)Input.GetAxisRaw("Horizontal");
        vert = (int)Input.GetAxisRaw("Vertical");
    }

    #endregion

    #region Key inputs by user/player

    public int KeyDirectionHorz()
    {
        /* Gets whether a left/right key is being pressed.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * int: 1 if left/right key is pressed, 0 otherwise.
         */

        return horz;
    }

    public int KeyDirectionVert()
    {
        /* Gets whether a up/down key is being pressed.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * int: 1 if up/down key is pressed, 0 otherwise.
         */

        return vert;
    }

    public bool KeyPressRestart()
    {
        /* Gets whether the R key is pressed
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if R key is pressed, False otherwise
         */

        return Input.GetKey(Restart);
    }

    public bool KeyPressUndo()
    {
        /* Gets whether the Z key is pressed
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if Z key is pressed, False otherwise
         */

        return Input.GetKey(Undo);
    }

    public bool KeyHoldTab()
    {
        /* Gets whether the TAB key is pressed
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if TAB key is pressed, False otherwise
         */
        return Input.GetKey(Invisible);
    }

    #endregion
}
