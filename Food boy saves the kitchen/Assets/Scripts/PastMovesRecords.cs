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
    private class MoveLogs
    {
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
            return turn;
        }

        public Vector3 getPrevPosition()
        {
            return prevPosition;
        }

        public List<bool> getTags()
        {
            return tags;
        }

        public bool getActive()
        {
            return isActive;
        }
    }

    private Stack<MoveLogs> moveLogs;
    private PastMovesManager moveManager; //higher in hierarchy
    private PastMovesSameTag sameTagManager; //lower in hierarchy. For tiles.
 
    // Start is called before the first frame update
    private void Awake()
    {
        moveLogs = new Stack<MoveLogs>();
        moveManager = GameObject.Find("Canvas").GetComponent<PastMovesManager>();
        sameTagManager = GetComponent<PastMovesSameTag>();
    }

    public void RecordMove()
    {
        /* Adds entry to moveLogs stack.
         */
        moveLogs.Push(new MoveLogs(turnNumber(), transform.position, 
            new List<bool>(GetComponent<Tags>().getTags().ToList()), gameObject.activeInHierarchy));

        if (isTile()) sameTagManager.RecordMove(turnNumber());
    }

    public void Undo()
    {
        /* Undo the move done by this object.
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

    private int turnNumber()
    {
        return moveManager.getTurn();
    } 

    private bool isTile()
    {
        return sameTagManager != null;
    }
}
