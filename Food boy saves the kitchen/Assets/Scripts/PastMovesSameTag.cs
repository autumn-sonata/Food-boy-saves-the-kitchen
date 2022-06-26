using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastMovesSameTag : MonoBehaviour
{
    /* Records the player changes in foodSameTag[] of YouManager.
     * To be attached to all You Tile game objects.
     */

    private class MoveLogs
    {
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
            return turn;
        }

        public List<GameObject> getFoodSameTag()
        {
            return foodSameTag;
        }

        public Collider2D getCol()
        {
            return col;
        }

        public Collider2D getOldCol()
        {
            return oldCol;
        }
    }

    private Stack<MoveLogs> moveLogs;
    private YouManager youTile;

    // Start is called before the first frame update
    private void Awake()
    {
        moveLogs = new Stack<MoveLogs>();
        youTile = GetComponent<YouManager>();
    }

    public void RecordMove(int turn)
    {
        /* Adds entry to moveLogs stack.
         */

        moveLogs.Push(new MoveLogs(turn, new List<GameObject>(youTile.getFoodSameTag()), 
            youTile.getCol(), youTile.getOldCol()));
    }

    public void Undo(int turn)
    {
        /* Undo the move done by this object.
         */
        if (moveLogs.Count > 0)
        {
            MoveLogs top = moveLogs.Pop();
            while (top.getTurn() > turn) top = moveLogs.Pop();
            moveLogs.Push(top);

            //Update to previous iteration
            youTile.Undo(top.getFoodSameTag(), top.getCol(), top.getOldCol());
        }
    }
}
