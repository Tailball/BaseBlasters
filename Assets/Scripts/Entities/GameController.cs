using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;


public class GameController : MonoBehaviour
{
    //SINGLETON
    public static GameController instance;


    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] GameObject MainUi = null;
    [SerializeField] Camera CameraOrthographic = null;
    [SerializeField] Camera CameraPerspective = null;
    [SerializeField] TMP_Text txtRoundNum = null;
    [SerializeField] GameObject RoomHolder = null;

    [Header("Instances")]
    [SerializeField] GameObject SelectedPlayer = null; //This should change to character select that will pass on the selected playercontroller;
    
    [Header("Tweaking")]
    [SerializeField][Range(1f, 5f)] float EnterSpeed = 1f;

    [Header("Physics Layers")]
    [SerializeField] LayerMask DefFloorLayer;
    [SerializeField] LayerMask DefWallLayer;


    //MEMBERS (PRIVATE)
    GameStates _gameState;
    PlayStates _playState;
    CombatStates _combatState;
    ExploreStates _exploreState;
    
    PlayerController _activePlayer;
    RoomController _activeRoom;
    List<RoomController> _roomsOnFloor = new List<RoomController>();

    int _round = 0;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public GameStates gameStates {
        get { return _gameState; }
    }

    public PlayStates playState {
        get { return _playState; }
    }

    public CombatStates combatState {
        get { return _combatState; }
    }

    public ExploreStates exploreState {
        get { return _exploreState; }
    }

    public LayerMask FloorLayer {
        get { return DefFloorLayer; }
    }

    public LayerMask WallLayer {
        get { return DefWallLayer; }
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
    }

    void Start() {
        MainUi.SetActive(false);

        this._gameState = GameStates.Ingame; 
        this._playState = PlayStates.Initializing;
        this._combatState = 0;
        this._exploreState = ExploreStates.PlayerMoving;
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
                //Todo
            break;

            case GameStates.Ingame:
                handlePlayState();
            break;
        }
    }


    //PRIVATE METHODS
    void handlePlayState() {
        switch(_playState) {
            case PlayStates.Initializing:
                initialize();
            break;

            case PlayStates.Entering:
                enter();
            break;

            case PlayStates.Exploring:
                getExploringInput();
                explore();
            break;

            case PlayStates.Combat:
            break;

            case PlayStates.Exiting:
            break;
        }
    }

    void initialize() {
        initializePlayer();
        initializePool();
        initializeFloor();

        CameraOrthographic.GetComponent<CameraController>().setTarget(_activePlayer.gameObject);
        CameraPerspective.GetComponent<CameraController>().setTarget(_activePlayer.gameObject);

        _playState = PlayStates.Entering;
    }

    void initializePlayer() {
        var player = Instantiate(SelectedPlayer, Vector3.zero, Quaternion.Euler(0, 180, 0));
        _activePlayer = player.GetComponent<PlayerController>();
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

    void enter() {
        CameraPerspective.nearClipPlane -= Time.deltaTime * EnterSpeed;
        if(CameraPerspective.nearClipPlane <= 3.8f) {
            CameraPerspective.nearClipPlane = 3.8f;
            _playState = PlayStates.Exploring;
            
            setNewRound();
            MainUi.SetActive(true);
        }
    }

    void getExploringInput() {
        if(_playState == PlayStates.Exploring) {
            if(Input.GetKeyDown(KeyCode.C)) {
                switchCamera();
            }
        }
    }

    void switchCamera() {
        CameraOrthographic.gameObject.SetActive(!CameraOrthographic.gameObject.activeInHierarchy);
        CameraPerspective.gameObject.SetActive(!CameraPerspective.gameObject.activeInHierarchy);
    }

    void explore() {
        switch(_exploreState) {
            case ExploreStates.PlayerMoving:
                var playerMover = _activePlayer.movementData;
                if(playerMover.HasMadeAMoveThisTurn && !playerMover.IsMoving) {
                    checkForCombat();
                    _activeRoom.enemies.setAllEnemiesExploreAction(_activePlayer.transform.position);
                    
                    _exploreState = ExploreStates.EnemyMoving;
                }
            break;

            case ExploreStates.EnemyMoving:
                var allEnemiesMadeAMoveThisTurn = _activeRoom.enemies.haveAllEnemiesMadeAMoveThisTurn();
                var allEnemiesDoneMoving = _activeRoom.enemies.haveAllEnemiesStoppedMoving();
                if(allEnemiesMadeAMoveThisTurn && allEnemiesDoneMoving) {
                    checkForCombat();
                    setNewRound();

                    _exploreState = ExploreStates.PlayerMoving;
                }
            break;
        }
    }

    void checkForCombat() {

    }

    void setNewRound() {
        _round++;

        _activePlayer.setNewRound();
        _activeRoom.setNewRound();

        txtRoundNum.text = _round.ToString();
    }


    //PUBLIC METHODS
}
