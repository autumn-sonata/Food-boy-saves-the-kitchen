using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Coordinator : MonoBehaviour
{
    /* Coordinates all player movements to be in sync. Only allows
     * for next set of movements once all players have finished moving.
     * 
     * Also coordinates which food items are to be players among you tiles.
     */
    private const float MoveDelay = 0.2f;

    private HashSet<GameObject> players; //stores every single player on the level
    private Dictionary<string, int> playerTypes; //stores the food types of players on the level
    private bool checkedMove;

    private Timer timer;
    private PastMovesManager moveManager;
    private InputManager inputManager;
    private List<WinManager> winTiles; //All instances of winTiles.
    private List<YouManager> youTiles; //All instances of youTiles.

    private void Awake()
    {
        players = new HashSet<GameObject>();
        playerTypes = new Dictionary<string, int>();

        timer = GetComponent<Timer>();
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
        PlayerRoutine();

        if (hasMoved())
        {
            foreach (YouManager youTile in youTiles)
            {
                youTile.ChangeYouObject();
            }

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
            executeWinManagers(); //update Win Manager to previous state.

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
            if (!playerTypes.ContainsKey("Knife")) playerTypes.Add("Knife", 0);
            playerTypes["Knife"]++; //Moves the same object
        }
        else
        {
            if (!playerTypes.ContainsKey(playerType)) playerTypes.Add(playerType, 0);
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
            youTile.Initialise();
            players.UnionWith(youTile.playersAttached());
            if (youTile.hasFoodOnYouTile())
            {
                string tagOnYouTile = youTile.youFoodTag();
                incrementPlayer(tagOnYouTile);
            }
        }
    }

    public bool allMovementsComplete()
    {
        /* Checks whether the moves for all moving players are complete
         * by checking whether they are all at their destination.
         */
        if (players.Count() == 0 && playerTypes.Values.All(numPlayer => numPlayer == 0))
        {
            //No players, force stop music and undo/reset
            //Placeholder:
            Debug.LogError("No players detected! Game over.");
        }

        return !players.Any(food => !food.GetComponent<PlayerManager>().isAtDestination());
    }

    private void PlayerRoutine()
    {
        if (allMovementsComplete())
        {
            //Everyone has stopped movement, poll for new move through input.
            float horizontalMove = inputManager.KeyDirectionHorz();
            float verticalMove = inputManager.KeyDirectionVert();

            var list = new List<KeyValuePair<PlayerManager, Vector3>>();
            //Allow players that are not children to update their destination.
            foreach (GameObject player in players)
            {
                if (Mathf.Abs(horizontalMove) == 1f && timer.countdownFinished() && !player.GetComponent<Tags>().isInAnyTile())
                {
                    Vector2 direction = new Vector2(horizontalMove, 0f);
                    if (player.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (player.GetComponent<PlayerManager>(), new Vector3(horizontalMove, 0f, 0f)));
                }
                else if (Mathf.Abs(verticalMove) == 1f && timer.countdownFinished() && !player.GetComponent<Tags>().isInAnyTile())
                {
                    Vector2 direction = new Vector2(0f, verticalMove);
                    if (player.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (player.GetComponent<PlayerManager>(), new Vector3(0f, verticalMove, 0f)));
                }
            }

            foreach (var kvp in list)
            {
                PlayerManager player = kvp.Key;
                Vector3 direction = kvp.Value;
                if (!player.isChild())
                {
                    player.moveDestination(direction);
                }
            }
        } else
        {
            timer.startTimer(MoveDelay);
        }
    }
}
