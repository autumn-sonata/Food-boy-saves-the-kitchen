using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastMovesRecords : MonoBehaviour
{
    /* Stores all the past records of this object.
     * Past moves are to be used by the undo function.
     */

    #region MoveLogs class initialisation

    private class MoveLogs
    {
        /* Stores all relevant information for undoing an action
         */

        private readonly int turn;
        private readonly Vector3 prevPosition;
        private readonly List<bool> tags;
        private readonly bool isActive;

        public MoveLogs(int turn, Vector3 prevPosition, List<bool> tags, bool isActive)
        {
            this.turn = turn;
            this.prevPosition = prevPosition;
            this.tags = tags;
            this.isActive = isActive;
        }

        public int getTurn()
        {
            /* Get the turn number of this move log.
             */

            return turn;
        }

        public Vector3 getPrevPosition()
        {
            /* Gets position recorded in this move log.
             */

            return prevPosition;
        }

        public List<bool> getTags()
        {
            /* Gets tags of gameObject at this turn number.
             */

            return tags;
        }

        public bool getActive()
        {
            /* Gets whether the gameObject has been destroyed
             * or not at this turn number.
             */

            return isActive;
        }
    }

    #endregion

    private Stack<MoveLogs> moveLogs;
    private PastMovesManager moveManager; //higher in hierarchy
    private PastMovesSameTag sameTagManager; //lower in hierarchy. For tiles.

    #region Unity specific functions

    private void Awake()
    {
        /* Initialises the move log stack and references to the move manager.
         */

        moveLogs = new Stack<MoveLogs>();
        moveManager = GameObject.Find("Canvas").GetComponent<PastMovesManager>();
        sameTagManager = GetComponent<PastMovesSameTag>();
    }

    #endregion

    #region Record

    public void RecordMove()
    {
        /* Adds entry to moveLogs stack.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        moveLogs.Push(new MoveLogs(turnNumber(), transform.position, 
            new List<bool>(GetComponent<Tags>().getTags().ToList()), gameObject.activeInHierarchy));

        if (isTile()) sameTagManager.RecordMove(turnNumber());
    }

    #endregion

    #region Undo

    public void Undo()
    {
        /* Undo the move done by this object back to previous
         * turn number.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (GetComponent<DetachChildren>())
            GetComponent<DetachChildren>().detachAllChildren();
        if (moveLogs.Count > 0)
        {
            MoveLogs top = moveLogs.Pop();
            while (top.getTurn() > turnNumber()) top = moveLogs.Pop();
            moveLogs.Push(top);

            //Update to previous iteration
            if (top.getActive())
            {

                gameObject.SetActive(true); 
                transform.position = top.getPrevPosition();
                if (GetComponent<PlayerManager>())
                    GetComponent<PlayerManager>().updateDestinationToCurrPosition();
                GetComponent<Tags>().setTags(top.getTags().ToArray());
                if (isTile()) sameTagManager.Undo(turnNumber());
            }
        }
    }

    #endregion

    #region Miscellaneous
    private int turnNumber()
    {
        /* Gets turn number of current turn.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * int: turn number of current turn.
         */

        return moveManager.getTurn();
    } 

    private bool isTile()
    {
        /* Checks if this object is a tile or not.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if this gameObject is a tile, False
         *   otherwise.
         */

        return sameTagManager != null;
    }

    #endregion
}
