using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameExploreController))]
[RequireComponent(typeof(GameCombatController))]
[RequireComponent(typeof(PoolController))]
[RequireComponent(typeof(DeckController))]
public class GameController : MonoBehaviour
{
    //SINGLETON
    public static GameController instance;


    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] GameObject ExploreUI = null;
    [SerializeField] GameObject CombatUI = null;
    [SerializeField] GameObject RoomHolder = null;

    [Header("Instances")]
    [SerializeField] GameObject SelectedPlayer = null; //This should change to character select that will pass on the selected playercontroller;
    
    [Header("Tweaking")]
    [SerializeField][Range(1f, 5f)] float EnterSpeed = 1f;

    [Header("Physics Layers")]
    [SerializeField] LayerMask DefFloorLayer;
    [SerializeField] LayerMask DefWallLayer;
    [SerializeField] LayerMask DefNPCLayer;


    //MEMBERS (PRIVATE)
    GameStates _gameState;
    PlayStates _playState;
    
    GameExploreController _exploreController = null;
    GameCombatController _combatController = null;
        
    List<RoomController> _roomsOnFloor = new List<RoomController>();
    RoomController _activeRoom;
    PlayerController _activePlayer;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public GameStates gameState {
        get { return _gameState; }
    }

    public PlayStates playState {
        get { return _playState; }
    }

    public ExploreStates passedExploreState {
        get { return _exploreController.state; }
    }

    public CombatStates passedCombatState {
        get { return _combatController.state; }
    }

    public LayerMask floorLayer {
        get { return DefFloorLayer; }
    }

    public LayerMask wallLayer {
        get { return DefWallLayer; }
    }

    public LayerMask npcLayer {
        get { return DefNPCLayer; }
    }

    public PlayerController activePlayer {
        get { return _activePlayer; }
    }

    public RoomController activeRoom {
        get { return _activeRoom; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this) {
            Destroy(gameObject);
        }

        _exploreController = GetComponent<GameExploreController>();
        _combatController = GetComponent<GameCombatController>();
    }

    void Start() {
        this._gameState = GameStates.ContentGeneration; 
        changePlaystateToInitializing();
    }

    void Update() {
        switch(_gameState) {
            //Maybe these aren't necessary if we have a dedicated menu controller and dedicated ingame controller?
            case GameStates.MainMenu:
            case GameStates.AboutMenu:
            case GameStates.SettingsMenu:
            case GameStates.Credits:
                //Todo
            break;

            case GameStates.ContentGeneration:
                handleContentGeneration();
            break;

            case GameStates.Ingame:
                handlePlayState();
            break;
        }
    }


    //PRIVATE METHODS
    void handleContentGeneration() {
        //TODO
        _gameState = GameStates.Ingame;
    }

    void handlePlayState() {
        switch(_playState) {
            case PlayStates.Initializing:
                execInitialize();
            break;

            case PlayStates.Entering:
                execEnter();
            break;

            case PlayStates.Exploring:
                _exploreController.exec();
            break;

            case PlayStates.Combat:
                _combatController.exec();
            break;

            case PlayStates.Exiting:
            break;

            case PlayStates.GameOver:
            break;
        }
    }

    void execInitialize() {
        initializePool();
        initializeFloor();
        initializePlayer();
        initializeDeck();

        _exploreController.setExploringCameraTarget(_activePlayer.gameObject);
        changePlaystateToEntering();
    }

    void initializePool() {
    }

    void initializeFloor() {
        var testRoom = Instantiate(PoolController.instance.getRoom(), Vector3.zero, Quaternion.identity, RoomHolder.transform);
        var roomController = testRoom.GetComponent<RoomController>();
        _roomsOnFloor.Add(roomController);
        _activeRoom = roomController;
        _activeRoom.enemies.instantiateEnemies();
    }

    void initializePlayer() {
        var player = Instantiate(SelectedPlayer, Vector3.zero, Quaternion.Euler(0, 180, 0));
        _activePlayer = player.GetComponent<PlayerController>();
    }

    void initializeDeck() {
        DeckController.instance.setStartSizeAndFillPool(6);
    }

    void execEnter() {
        var cam = _exploreController.cameraPerspective;
        cam.nearClipPlane -= Time.deltaTime * EnterSpeed;

        if(cam.nearClipPlane <= 3.8f) {
            cam.nearClipPlane = 3.8f;
            
            _exploreController.setNewExploreRound();

            ExploreUI.SetActive(true);
            CombatUI.SetActive(false);

            changePlaystateToExploring();
        }
    }


    //PUBLIC METHODS
    public void changePlaystateToInitializing() {
        _playState = PlayStates.Initializing;
    }

    public void changePlaystateToEntering() {
        _playState = PlayStates.Entering;
    }

    public void changePlaystateToCombat(EnemyController withEnemy) {
        _combatController.enabled = false;
        _exploreController.enabled = true;
        
        ExploreUI.SetActive(false);
        CombatUI.SetActive(true);

        _combatController.setActiveEnemy(withEnemy);
        _playState = PlayStates.Combat;
    }

    public void changePlaystateToExploring() {
        _combatController.enabled = false;
        _exploreController.enabled = true;
        
        ExploreUI.SetActive(true);
        CombatUI.SetActive(false);

        _playState = PlayStates.Exploring;
    }

    public void changePlaystateToGameOver() {
        Debug.Log("GAME OVER");
        _playState = PlayStates.GameOver;
    }

    public void acceptPlayerMove() {
        _combatController.acceptPlayerMove();
    }
}