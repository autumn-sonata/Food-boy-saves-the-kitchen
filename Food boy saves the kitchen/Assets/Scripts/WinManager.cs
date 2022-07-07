using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    /* Checks for the winning condition on the board.
     * Fails the player if there is no other way to win aka a reset or undo must be made
     * to further progress.
     */
    private GameObject[,] winConfig; //current winning configuration of the level
    private List<GameObject> foodSameTagTopLeft; //stores all foods that are the same tag as topLeft that is not on the win tile.
    private Vector2 topLeftCenteredCoord;

    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;

    private LayerMask push;

    private void Awake()
    {
        /* Prepare for reading the original win configuration.
         */
        push = LayerMask.GetMask("Push");
        foodSameTagTopLeft = new List<GameObject>();
        int x = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.x);
        int y = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.y);
        winConfig = new GameObject[y, x];
        UpdateTileCorners();
        topLeftCenteredCoord = new Vector2(transform.position.x - x / 2f,
            transform.position.y + y / 2f) + new Vector2(0.5f, -0.5f);
        
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                Collider2D withinWinTile = Physics2D.OverlapPoint(topLeftCenteredCoord + new Vector2(j, i), push);
                if (withinWinTile)
                {
                    withinWinTile.GetComponent<Tags>().enableWinTileTag();
                    winConfig[i, j] = withinWinTile.gameObject;
                }
            }
        }

        Collider2D topLeft = Physics2D.OverlapPoint(topLeftCenteredCoord, push);
        if (topLeft)
        {
            GameObject[] topLeftFoods = GameObject.FindGameObjectsWithTag(topLeft.tag);
            foreach (GameObject food in topLeftFoods)
            {
                if (!food.GetComponent<Tags>().isInWinTile())
                {
                    foodSameTagTopLeft.Add(food);
                }
            }
        }
    }

    public void moveExecuted()
    {
        /* What to run when a move is being done.
         * 1) Get winning configuration on win tile.
         * 2) Search board for winning states.
         */
        updateWinCondition();
        checkIfWin();
    }

    private void updateWinCondition()
    {
        /* Checks whether win condition has been updated. 
         * If so, update winConfig, foodSameTagTopLeft and topLeftCenteredCoord. 
         */

        //Check topLeftCenteredCoord since coord of winTile might have changed.
        topLeftCenteredCoord = new Vector2(transform.position.x - winConfig.GetLength(1) / 2f, 
            transform.position.y + winConfig.GetLength(0) / 2f) + new Vector2(0.5f, -0.5f);

        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                if (winConfig[row, col])
                {
                    Vector2 currPosition = winConfig[row, col].transform.position;
                    if (currPosition != topLeftCenteredCoord + new Vector2(col, row))
                    {
                        //Only replace if found that there is a different food there.
                        GameObject updatedFood =
                            Physics2D.OverlapPoint(topLeftCenteredCoord + new Vector2(col, row), push).gameObject;
                        winConfig[row, col] = updatedFood;
                        if (row == 0 && col == 0)
                        {
                            updateFoodSameTagTopLeft(updatedFood);
                        }
                    }
                }
            }
        }
    }

    private void checkIfWin()
    {
        /* Checks if there are any winning configurations in the level.
         * Does this by checking foodSameTagTopLeft and whether the relative configuration
         * matches winConfig.
         */

        foreach (GameObject food in foodSameTagTopLeft)
        {
            if (matchWinConfig(food))
            {
                Debug.Log("You Win!");
            }
        }
    }

    private bool matchWinConfig(GameObject foodTopLeft)
    {
        /* Matches win configuration using the food name of each item.
         * Note that the win tile must be completely full before a winning condition 
         */
        Vector2 foodCoord = foodTopLeft.transform.position;
        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                Collider2D foodConfig = Physics2D.OverlapPoint(foodCoord + new Vector2(col, row), push);
                if (!winConfig[row, col] || !foodConfig ||
                    winConfig[row, col].tag != foodConfig.tag || 
                    winConfig[row, col].GetComponent<Tags>().isCut() != foodConfig.GetComponent<Tags>().isCut() ||
                    winConfig[row, col].GetComponent<Tags>().isHot() != foodConfig.GetComponent<Tags>().isHot() ||
                    winConfig[row, col].GetComponent<Tags>().isCold() != foodConfig.GetComponent<Tags>().isCold() ||
                    winConfig[row, col].GetComponent<Tags>().isCooked() != foodConfig.GetComponent<Tags>().isCooked())
                {
                    //Configuration is wrong
                    return false;
                }
            }
        }

        return true;
    }

    private void updateFoodSameTagTopLeft(GameObject topLeft)
    {
        /* Update the foodSameTagTopLeft list to new top left food item since it has changed.
         */
        foodSameTagTopLeft.Clear();

        foreach (GameObject food in GameObject.FindGameObjectsWithTag(topLeft.tag))
        {
            if (!food.GetComponent<Tags>().isInWinTile())
            {
                foodSameTagTopLeft.Add(food);
            }
        }
    }

    private void UpdateTileCorners()
    {
        topLeftCorner = new Vector2(transform.position.x - winConfig.GetLength(1) / 2f,
            transform.position.y + winConfig.GetLength(0) / 2f);
        bottomRightCorner = new Vector2(transform.position.x + winConfig.GetLength(1) / 2f,
            transform.position.y - winConfig.GetLength(0) / 2f);
    }

    public void ExitTrigger()
    {
        /* Checks if any winConfig elements exited the win tile.
         * If so, disable the win tile tag. Re-enable heavy and hot/cold
         * as necessary.
         * 
         * Current winConfig is outdated during execution of this function.
         */
        UpdateTileCorners();
        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                if (winConfig[row, col])
                {
                    Vector2 currPosition = winConfig[row, col].transform.position;
                    if (currPosition.x < topLeftCorner.x || bottomRightCorner.x < currPosition.x ||
                        currPosition.y > topLeftCorner.y || bottomRightCorner.y < currPosition.y)
                    {
                        //out of win tile.
                        winConfig[row, col].GetComponent<Tags>().disableWinTileTag();
                    }
                }
            }
        }
    }

    public void EnterTrigger()
    {
        /* Checks if any new object is now in the win tile. 
         * If so, enable the win tile tag and remove children.
         * Disable heavy and hot/cold as necessary.
         */
        Collider2D[] winTileObj = 
            Physics2D.OverlapAreaAll(topLeftCorner + new Vector2(0.5f, -0.5f), 
            bottomRightCorner + new Vector2(-0.5f, 0.5f), push);

        foreach (Collider2D collider in winTileObj)
        {
            collider.GetComponent<Tags>().enableWinTileTag();
            collider.GetComponent<DetachChildren>().detachAllChildren();
        }
    }
}
