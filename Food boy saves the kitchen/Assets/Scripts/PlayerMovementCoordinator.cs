using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementCoordinator : MonoBehaviour
{
    /* Coordinates all player movements to be in sync. Only allows
     * for next set of movements once all players have finished moving.
     * 
     * Also coordinates which food items are to be players among you tiles.
     */
    private HashSet<GameObject> players; //stores every single player on the level
    private Dictionary<string, int> playerTypes; //stores the food types of players on the level
    private bool checkedMove;

    private PastMovesManager moveManager;
    private InputManager inputManager;
    private List<WinManager> winTiles; //All instances of winTiles.
    private List<YouManager> youTiles; //All instances of youTiles.

    private void Awake()
    {
        players = new HashSet<GameObject>();
        playerTypes = new Dictionary<string, int>();

        moveManager = GameObject.Find("Canvas").GetComponent<PastMovesManager>();
        inputManager = GetComponent<InputManager>();
        winTiles = GameObject.FindGameObjectsWithTag("Win")
            .Select(winTile => winTile.GetComponent<WinManager>()).ToList();
        youTiles = GameObject.FindGameObjectsWithTag("You")
            .Select(youTile => youTile.GetComponent<YouManager>()).ToList();
        checkedMove = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get all players that can be moved.
        UpdatePlayerAttrInfo();
    }

    private void Update()
    {
        /* Central call to all other managers.
         */
        foreach (YouManager youTile in youTiles)
        {
            youTile.ChangeYouObject();
        }

        if (hasMoved())
        {
            //WinManager updates
            executeWinManagers();

            //YouManager updates
            foreach (YouManager youTile in youTiles)
            {
                youTile.moveExecuted();
            }

            //ask moveManager to make all PastMovesRecords to record their moves.
            moveManager.RecordThisMove();
            checkedMove = true;
        }

        //Undo and restart
        if (inputManager.KeyPressUndo())
        {
            moveManager.DoUndo();
            executeWinManagers(); //update Win Manager.

            players.Clear();
            playerTypes.Clear();
            UpdatePlayerAttrInfo();
        }
        if (inputManager.KeyPressRestart()) moveManager.RestartLevel();
    }

    public void decrementPlayer(string playerType)
    {
        /* Decrements value of playerType by 1 in playerTypes hashmap.
         */
        if (playerType.Contains("Knife"))
        {
            playerTypes["Knife"]--; //Moves the same object
        }
        else
        {
            playerTypes[playerType]--;
        }
    }

    public void incrementPlayer(string playerType)
    {
        /* Increments value of playerType by 1 in playerTypes hashmap.
         */
        if (playerType.Contains("Knife"))
        {
            playerTypes["Knife"]++; //Moves the same object
        }
        else
        {
            playerTypes[playerType]++;
        }
    }

    public bool isPlayer(string playerType)
    {
        /* Checks whether the playerType is still a valid player
         * (due to other you tiles having the object on it)
         */
        if (playerType.Contains("Knife"))
            return playerTypes["Knife"] > 0;
        return playerTypes[playerType] > 0;
    }

    public void addAndRemovePlayers(List<GameObject> foods)
    {
        /* To call to add and remove items from players that are no longer players.
         */
        players.RemoveWhere(food => !food.GetComponent<Tags>().isPlayer());
        players.UnionWith(foods); //player tag already enabled
    }

    private bool hasMoved()
    {
        /* Ask if a move is done.
         */
        bool movementsComplete = allMovementsComplete();
        if (!movementsComplete) checkedMove = false;
        return movementsComplete && !checkedMove;
    }

    private void executeWinManagers()
    {
        foreach (WinManager winTile in winTiles)
        {
            winTile.moveExecuted();
        }
    }

    private void UpdatePlayerAttrInfo()
    {
        /* Updates players hashset and playerTypes hashmap.
         */
        foreach (YouManager youTile in youTiles)
        {
            players.UnionWith(youTile.playersAttached());
            string tagOnYouTile = youTile.youFoodTag();
            if (playerTypes.ContainsKey(tagOnYouTile)) incrementPlayer(tagOnYouTile);
            else playerTypes.Add(tagOnYouTile, 1);
        }
    }

    private bool allMovementsComplete()
    {
        /* Checks whether the moves for all moving players are complete
         * by checking whether they are all at their destination.
         */
        if (players.Count() == 0)
        {
            //No players, force stop music and undo/reset
            //Placeholder:
            Debug.LogError("No players detected!");
        }
        return !players.Any(food => !food.GetComponent<PlayerManager>().isAtDestination());
    }
}
