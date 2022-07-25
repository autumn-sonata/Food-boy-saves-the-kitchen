using System;
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

    private HashSet<GameObject> players; //stores every single player on the level. Managed by YouManager
    private Dictionary<string, int> playerTypes; //stores the food types of players on the level
    private List<GameObject> invisibleCache; //stores items that are invisible when holding tab
    private List<GameObject> tmpPlayers; //stores players for conveyer belt to act as players
    private bool checkedMove; //checks if a move has been made on the board (with movement)
    private bool isInitialise; //checks if this is the first time a script is run on the scene.
    private bool hasUndone; 
    private bool playerCanMove; //checks if the player is able to move (after conveyer belt moves).
    private bool winFound; //checks if the user/player has beat the level.
    private bool winUpdateAftUndo; 

    private Timer playerTimer; //timer to count cooldown on player movement.
    private PastMovesManager moveManager; //manager to store past moves (for undo)
    private InputManager inputManager; //manager to receive player inputs from keyboard.
    private List<SpriteManager> sprites; //sprites of all the different movable objects in the level.
    private List<WinManager> winTiles; //All instances of winTiles.
    private List<YouManager> youTiles; //All instances of youTiles.
    private List<ColdManager> coldTiles; //All instances of coldTiles.
    private List<HotManager> hotTiles; //All instances of hotTiles.
    private List<HeavyManager> heavyTiles; //All instances of heavyTiles.
    private List<TileManager> tiles; //All tiles excluding winTiles.
    private List<ConveyerBeltManager> conveyerBelts; //All conveyer belts.

    #region Unity specific functions

    private void Awake()
    {
        /* Initialisation of players and playerTypes, 
         * and all other variables. Since this is the central script for running the game,
         * so this function will look for all the tiles, and save them for future use.
         */

        players = new();
        playerTypes = new();
        tmpPlayers = new();
        invisibleCache = new();

        playerTimer = GetComponent<Timer>();
        moveManager = GameObject.Find("Canvas").GetComponent<PastMovesManager>();
        inputManager = GetComponent<InputManager>();
        sprites = GetAllSprites();
        winTiles = FindObjectsOfType<WinManager>().ToList();
        youTiles = FindObjectsOfType<YouManager>().ToList();
        coldTiles = FindObjectsOfType<ColdManager>().ToList();
        hotTiles = FindObjectsOfType<HotManager>().ToList();
        heavyTiles = FindObjectsOfType<HeavyManager>().ToList();
        tiles = new List<TileManager>();
        tiles = tiles.Concat(youTiles).Concat(coldTiles).Concat(hotTiles)
            .Concat(heavyTiles).ToList();
        conveyerBelts = FindObjectsOfType<ConveyerBeltManager>().ToList();
        checkedMove = false;
        isInitialise = true;
        hasUndone = false;
        playerCanMove = true;
        winFound = false;
        winUpdateAftUndo = false;
        InitialiseAllAttributes(); //initialise cooked and cut
    }

    void Start()
    {
        /* Updates players hashset and playerTypes hashmap.
         */

        foreach (TileManager tile in tiles)
        {
            tile.Initialise();
        }

        foreach (TileManager youTile in youTiles)
        {
            players.UnionWith(youTile.playersAttached());
            if (youTile.hasFoodOnYouTile())
            {
                incrementPlayer(youTile.colFoodTag());
            }
        }
    }

    private void Update()
    {
        /* Central call to all other managers. This function is the driver for the
         * logic of the entire game.
         */

        if (winUpdateAftUndo)
        {
            //next frame right after, sprite has moved, overlappoint can pinpoint.
            executeWinManagers(); //update Win Manager to previous state.
            winUpdateAftUndo = false;
        }
        if (inputManager.KeyHoldTab() && !invisibleCache.Any())
        {
            /* Make all movable objects that are currently visible
             * to be temporarily invisible as long as tab key is held.
             */
            int pushLayer = LayerMask.NameToLayer("Push");
            invisibleCache = FindObjectsOfType<GameObject>().ToList()
                .FindAll(obj => obj.layer == pushLayer);
            invisibleCache.ForEach(obj => obj.SetActive(false));
        } else if (!inputManager.KeyHoldTab())
        {
            if (invisibleCache.Any())
            {
                invisibleCache.ForEach(obj => obj.SetActive(true));
                invisibleCache.Clear();
            }

            PlayerRoutine();
            LevelTagPlayerUpdate(); //updates all the tags on the board.
            ConveyerBeltRoutine();
            LevelTagBeltUpdate();
        }
    }

    #endregion

    #region Attribute initialisation
    private void InitialiseAllAttributes()
    {
        /* Initialise cut and cooked if stated as so.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */
        foreach (AttributeInitialisation attr in FindObjectsOfType<AttributeInitialisation>())
        {
            attr.Initialise();
        }
    }

    #endregion

    #region Driver code for update
    private void LevelTagPlayerUpdate()
    {
        /* Driver code for updating the positions and sprites
         * based on user/player input.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        CheckInactive();
        CheckUndo();

        if (hasMoved())
        {
            TagRoutine();
        }
        SpriteUpdate();
    }

    private void LevelTagBeltUpdate()
    {
        /* Driver code for updating the positions and sprites
         * based on conveyer input.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        CheckInactive();
        CheckUndo();

        if (hasMoved())
        {
            TagRoutine();
            //ask moveManager to make all PastMovesRecords to record their moves.
            moveManager.RecordThisMove();
            checkedMove = true;
            isInitialise = false;
        }
        SpriteUpdate();
        checkWinConfig(); //check static position cut 
        UndoRestartRoutine();
    }

    private void TagRoutine()
    {
        /* Updates the tiles and tags for their different properties
         * and knowledge of who is standing on which tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (GameObject player in players)
        {
            player.GetComponent<DetachChildren>().detachAllChildren();
        }

        foreach (TileManager tile in tiles)
        {
            tile.TriggerTile(); //update collider
        }

        /* Dictionary parameters -> [Tag name: [isCold?, isHot?]]
         * 
         * The hotCold hashmap represents the objects on hot and cold 
         * tiles and whether  
         */
        Dictionary<string, bool[]> hotCold = new();

        UpdateHotColdPlayers(hotCold);

        if (!isInitialise)
        {
            foreach (TileManager tile in tiles)
            {
                /* Enable hot/cold and heavy again for food 
                 * items going out of tile
                 */
                tile.ActivateDormantHotCold(hotCold);
            }

            //You, cold and hot tile tag + updates in case change of objects
            foreach (TileManager tile in tiles)
            {
                tile.OldColUpdate(); //run all old col updates before new ones.
            }

            foreach (WinManager winTile in winTiles)
            {
                winTile.ExitTrigger(hotCold, heavyTiles);
            }

            //Tag updates for new objects that stepped into different tiles
            foreach (WinManager winTile in winTiles)
            {
                winTile.EnterTrigger();
            }

            foreach (TileManager tile in tiles)
            {
                tile.NewColUpdate();
            }

            //Deactivate objects if both hot and cold.
            foreach (TileManager tile in tiles)
            {
                tile.DeactivateDormantHotCold(hotCold);
            }
        }

        HeavyActivation();
        SpriteUpdate(); //deactivates gameobject if tags are appropriate.

        //Win tile updates
        executeWinManagers();
    }

    #endregion

    #region players array modifications

    public void decrementPlayer(string playerType)
    {
        /* Decrements value of playerType by 1 in playerTypes hashmap.
         * 
         * Parameters
         * ----------
         * 1) playerType: string containing the tag name of the object stepping
         *   out of a tile.
         * 
         * Return
         * ------
         * 
         */

        playerTypes[playerType]--;
    }

    public void incrementPlayer(string playerType)
    {
        /* Increments value of playerType by 1 in playerTypes hashmap.
         * 
         * Parameters
         * ----------
         * 1) playerType: string containing the tag name of the object stepping
         *   into a tile.
         * 
         * Return
         * ------
         * 
         */

        if (!playerTypes.ContainsKey(playerType)) playerTypes.Add(playerType, 0);
            playerTypes[playerType]++;
    }

    public bool isPlayer(string playerType)
    {
        /* Checks whether the playerType is still a valid player
         * (due to other you tiles having the object on it)
         * 
         * Parameters
         * ----------
         * 1) playerType: string containing the tag name of an object.
         * 
         * Return
         * ------
         * bool: checks whether that object is supposed to be a player based
         *   on the tag it has.
         */

        return playerTypes[playerType] > 0;
    }

    public void addAndRemovePlayers(List<GameObject> foods)
    {
        /* To call to add and remove items from players that are no longer players.
         * 
         * Parameters
         * ----------
         * 1) foods: food items that has been updated based on objects moving onto/
         *   out of tiles.
         * 
         * Return
         * ------
         * 
         */

        players.RemoveWhere(food => !food.GetComponent<Tags>().isPlayer());
        players.UnionWith(foods); //player tag already enabled
    }

    #endregion

    #region win tile modifications
    public void WinFound()
    {
        /* Disable any movements of any player.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        winFound = true;
    }

    private void executeWinManagers()
    {
        /* Updates the win tiles of their current winning
         * configurations and checks for any winning states
         * on the board.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (WinManager winTile in winTiles)
        {
            winTile.moveExecuted();
        }
    }

    private void checkWinConfig()
    {
        /* All win tiles on the level will check for
         * possible win states on the board.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (WinManager winTile in winTiles)
        {
            winTile.checkIfWin();
        }
    }

    #endregion

    #region sprite updates
    private List<SpriteManager> GetAllSprites()
    {
        /* Gets all the sprites present in the scene.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * List<SpriteManager>: List of all SpriteManager scripts present
         *   in the scene.
         */

        SpriteManager[] allObjects = FindObjectsOfType<SpriteManager>();
        return allObjects.ToList();
    }

    private void SpriteUpdate()
    {
        /* Update sprites to match tags.
         * However, if gameObject isInactive, then first: 
         * 1) Disable gameObjects
         * 2) Update sprites list.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        List<SpriteManager> toDisable = sprites.FindAll(sprite =>
            sprite.GetComponent<Tags>().isInactive());
        toDisable.ForEach(sprite => sprite.gameObject.SetActive(false));

        foreach (SpriteManager sprite in sprites)
        {
            sprite.UpdateSprites();
        }
    }

    #endregion

    #region player and conveyer belt movement
    private bool allMovementsComplete()
    {
        /* Checks whether the moves for all moving players are complete
         * by checking whether they are all at their destination.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if all movements of objects to their respective
         *   destinations are complete, false otherwise.
         */

        if (players.Count() == 0 && playerTypes.Values.All(numPlayer => numPlayer == 0))
        {
            //No players, force stop music and undo/reset
            //Placeholder:
            Debug.LogError("No players detected! Game over.");
        }

        return !players.Any(food => !food.GetComponent<PlayerManager>().isAtDestination());
    }

    private bool hasMoved()
    {
        /* Ask if a move is done.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if a move is done on the board, false otherwise.
         */

        bool movementsComplete = allMovementsComplete();
        if (!movementsComplete) checkedMove = false;
        return allMovementsComplete() && !checkedMove;
    }

    private void PlayerRoutine()
    {
        /* Driver code for carrying out player movement based
         * on user/player keyboard input. 
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (allMovementsComplete() && playerCanMove)
        {
            if (tmpPlayers.Any())
            {
                tmpPlayers.ForEach(obj => {
                    obj.GetComponent<Tags>().disablePlayerTag();
                    obj.GetComponent<DetachChildren>().detachAllChildren();
                });
                players.RemoveWhere(player => !player.GetComponent<Tags>().isPlayer());
            }
            //Everyone has stopped movement, poll for new move through input.
            float horizontalMove = inputManager.KeyDirectionHorz();
            float verticalMove = inputManager.KeyDirectionVert();

            var list = new List<KeyValuePair<PlayerManager, Vector3>>();
            //Allow players that are not children to update their destination.
            foreach (GameObject player in players)
            { 
                if (Mathf.Abs(horizontalMove) == 1f && playerTimer.countdownFinished() && 
                    !player.GetComponent<Tags>().isInAnyTile() && !winFound)
                {
                    Vector2 direction = new(horizontalMove, 0f);
                    playerCanMove = false;
                    if (player.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (player.GetComponent<PlayerManager>(), new Vector3(horizontalMove, 0f, 0f)));
                }
                else if (Mathf.Abs(verticalMove) == 1f && playerTimer.countdownFinished() && 
                    !player.GetComponent<Tags>().isInAnyTile() && !winFound)
                {
                    Vector2 direction = new(0f, verticalMove);
                    playerCanMove = false;
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
                if (!player.isChild() && player.GetComponent<Tags>().isPlayer())
                {
                    player.moveDestination(direction);
                }
            }
        } else if (!allMovementsComplete())
        {
            playerTimer.startTimer(MoveDelay);
        }
    }

    private void ConveyerBeltRoutine()
    {
        /* Mimics player movement on the conveyer belt.
         * Applies same logic as moving an object as a player.
         * 
         * Uses "players" from playerConveyerBelt instead.
         * Only runs when there is a player update.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (allMovementsComplete() && !playerCanMove && !isInitialise)
        {
            var list = new List<KeyValuePair<PlayerManager, Vector3>>();
            tmpPlayers.Clear();
            //Allow players that are not children to update their destination.
            foreach (ConveyerBeltManager belt in conveyerBelts)
            {
                float horizontalMove = belt.getPushDirectionHorz();
                float verticalMove = belt.getPushDirectionVert();
                Collider2D objOnTop = belt.getObjOnTop();
                if (Mathf.Abs(horizontalMove) == 1f && !winFound)
                {
                    Vector2 direction = new(horizontalMove, 0f);
                    if (objOnTop && objOnTop.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (objOnTop.GetComponent<PlayerManager>(), new Vector3(horizontalMove, 0f, 0f)));
                }
                else if (Mathf.Abs(verticalMove) == 1f && !winFound)
                {
                    Vector2 direction = new(0f, verticalMove);
                    if (objOnTop && objOnTop.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (objOnTop.GetComponent<PlayerManager>(), new Vector3(0f, verticalMove, 0f)));
                }
            }

            foreach (var kvp in list)
            {
                PlayerManager player = kvp.Key;
                Vector3 direction = kvp.Value;
                //Unconditionally move this object as it is on a conveyer belt.
                player.moveDestination(direction);
                player.GetComponent<Tags>().enablePlayerTag();
            }

            tmpPlayers = list.ConvertAll(obj => obj.Key.gameObject).FindAll(obj => !players.Contains(obj));
            players.UnionWith(tmpPlayers);
            playerCanMove = true;
        }
    }

    #endregion

    #region undo and restart 

    private void CheckInactive()
    {
        /* Updates players array on whether the objects are supposed to be destroyed
         * or not. Objects are destroyed if they are both hot and cold, or when
         * hot and cold objects collide with each other.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        players.RemoveWhere(player => player.GetComponent<Tags>().isInactive());
    }

    private void CheckUndo()
    {
        /* Re-initialises the tiles of their objects standing on top of them after
         * and undo has been executed.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (hasUndone)
        {
            hasUndone = false;
            //update tiles
            foreach (TileManager tile in tiles)
            {
                tile.Initialise();
            }
        }
    }

    #endregion

    #region movable object attributes

    private void UpdateHotColdPlayers(Dictionary<string, bool[]> hotCold)
    {
        /* Initialises the hotCold hashmap.
         * Updates players based on hotCold.
         * 
         * Parameters
         * ----------
         * 1) hotCold: Determines whether the food type is supposed to be hot or cold.
         * 
         * Return
         * ------
         * 
         */

        foreach (TileManager hotTile in hotTiles)
        {
            if (!String.IsNullOrEmpty(hotTile.colFoodTag()))
                AddOrUpdateDict(hotCold, hotTile.colFoodTag(), new bool[] { false, true });
        }

        foreach (TileManager coldTile in coldTiles)
        {
            if (!String.IsNullOrEmpty(coldTile.colFoodTag()))
                AddOrUpdateDict(hotCold, coldTile.colFoodTag(), new bool[] { true, false });
        }

        List<GameObject> removeFromPlayer = new List<GameObject>();
        //Update players if there are destroys due to same food item being on hot and cold tile.
        foreach (GameObject player in players)
        {
            if (hotCold.ContainsKey(player.tag) && hotCold[player.tag].All(qn => qn))
            {
                //Both hot and cold. Remove from player from players.
                removeFromPlayer.Add(player);
            }
        }

        foreach (GameObject player in removeFromPlayer)
        {
            players.Remove(player);
        }
    }

    private void HeavyActivation()
    {
        /* After update of other tags, update heavy tags.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        Tags[] objWithTag = FindObjectsOfType<Tags>();
        Dictionary<string, bool> heavyTags = new();
        foreach (TileManager tile in heavyTiles)
        {
            if (!String.IsNullOrEmpty(tile.colFoodTag()))
                heavyTags.Add(tile.colFoodTag(), true);
        }

        foreach (Tags obj in objWithTag)
        {
            if (heavyTags.ContainsKey(obj.tag) && !obj.isPlayer() && !obj.isInAnyTile()) 
            {
                obj.enableHeavy();
            } else
            {
                obj.disableHeavy();
            }
        }
    }

    #endregion

    #region undo and restart

    private void UndoRestartRoutine()
    {
        /* Specifies execution of undo and restart,
         * namely "Z" and "R" for the user/player.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (inputManager.KeyPressUndo() && !winFound)
        {
            hasUndone = true;
            moveManager.DoUndo();
            winUpdateAftUndo = true;
            SpriteUpdate();
            //update players and playerTypes to previous state
            players.Clear();
            playerTypes.Clear();
            foreach (TileManager youTile in youTiles)
            {
                players.UnionWith(youTile.playersAttached());
                if (youTile.hasFoodOnYouTile())
                {
                    incrementPlayer(youTile.colFoodTag());
                }
            }
        }
        if (inputManager.KeyPressRestart() && !winFound) moveManager.RestartLevel();
    }

    #endregion

    #region miscellaneous

    private static void AddOrUpdateDict(Dictionary<string, bool[]> dict, string key, bool[] value)
    {
        /* Updates the dictionary (hot cold) with the object type, and whether it has hot and/or
         * cold.
         * 
         * Parameters
         * ----------
         * 1) dict: Dictionary in which the information is being updated to
         * 2) key: tags that the movable objects will have
         * 3) value: boolean array of size 2 that will determine whether the tag
         *   is supposed to have hot and/or cold associated with it.
         * 
         * Return
         * ------
         * 
         */

        if (dict.ContainsKey(key))
        {
            bool[] lst = dict[key];
            if (lst.Length != value.Length)
            {
                Debug.LogError
                    ("Dictionary bool[] and provided value array length is not the same.");
            }

            for (int i = 0; i < lst.Length; i++)
            {
                if (!lst[i])
                    dict[key][i] = value[i];
            }
        } else
        {
            dict.Add(key, value);
        }
    }

    private void DebugTags(string name)
    {
        /* For debugging tags. 
         * 
         * Parameters
         * ----------
         * 1) name: name of object in scene.
         * 
         * Return
         * ------
         * 
         */
        GameObject.Find(name).GetComponent<Tags>().printTags();
    }

    #endregion
}
