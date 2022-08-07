using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastMovesSameTag : MonoBehaviour
{
    /* Records the player changes in foodSameTag[] of TileManager.
     * To be attached to all You Tile game objects.
     */

    private class MoveLogs
    {
        /* MoveLogs class to store information to revert back to 
         * for tiles. 
         */

        private int turn;
        private List<GameObject> foodSameTag;
        private Collider2D col;
        private Collider2D oldCol;

        public MoveLogs(int turn, List<GameObject> foodSameTag, Collider2D col, Collider2D oldCol)
        {
            this.turn = turn;
            this.foodSameTag = foodSameTag;
            this.col = col;
            this.oldCol = oldCol;
        }

        public int getTurn()
        {
            /* Gets recorded turn.
             */

            return turn;
        }

        public List<GameObject> getFoodSameTag()
        {
            /* Gets recorded food items of the same tag
             * that would be affected by tag changes of 
             * current collider.
             */

            return foodSameTag;
        }

        public Collider2D getCol()
        {
            /* Get the collider of the gameObject on the 
             * tile.
             */

            return col;
        }

        public Collider2D getOldCol()
        {
            /* Get the previous collider of the gameObject
             * on the tile.
             */

            return oldCol;
        }
    }

    private Stack<MoveLogs> moveLogs;
    private TileManager tile;

    #region Unity specific functions

    private void Awake()
    {
        moveLogs = new Stack<MoveLogs>();
        tile = SelectTileType();
    }

    #endregion

    #region Record

    public void RecordMove(int turn)
    {
        /* Adds entry to moveLogs stack.
         * 
         * Parameters
         * ----------
         * turn: turn number to register the current state
         *   of the gameObject.
         * 
         * Return
         * ------
         * 
         */

        moveLogs.Push(new MoveLogs(turn, new List<GameObject>(tile.getFoodSameTag()), 
            tile.getCol(), tile.getOldCol()));
    }

    #endregion

    #region Undo

    public void Undo(int turn)
    {
        /* Undo the move done by this object.
         * 
         * Parameters
         * ----------
         * turn: turn number to register the current state
         *   of the gameObject.
         * 
         * Return
         * ------
         * 
         */

        if (moveLogs.Count > 0)
        {
            MoveLogs top = moveLogs.Pop();
            while (top.getTurn() > turn) top = moveLogs.Pop();
            moveLogs.Push(top);

            //Update to previous iteration
            tile.Undo(top.getFoodSameTag(), top.getCol(), top.getOldCol());
        }
    }

    #endregion

    #region Miscellaneous

    private TileManager SelectTileType()
    {
        /* Choose between the different tile types.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * TileManager: Selects the correct tile type
         *   of current gameObject this script is attached to.
         */

        if (GetComponent<YouManager>())
        {
            return GetComponent<YouManager>();
        } else if (GetComponent<HotManager>()){
            return GetComponent<HotManager>();
        } else if (GetComponent<ColdManager>())
        {
            return GetComponent<ColdManager>();
        } else if (GetComponent<HeavyManager>())
        {
            return GetComponent<HeavyManager>();
        }
        Debug.LogError("Manager not found in PastMovesSameTag.");
        return null;
    }

    #endregion
}
