using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //SINGLETON
    public static GameController instance;


    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] Camera CameraOrthographic = null;
    [SerializeField] Camera CameraPerspective = null;

    [Header("Pools")]
    [SerializeField] GameObject SelectedPlayer = null; //This should change to character select that will pass on the selected playercontroller;
    [SerializeField] GameObject Enemy = null; //Make seperate class that is able to create pools based on room, level and difficulty

    [Header("Tweaking")]
    [SerializeField][Range(1f, 5f)] float EnterSpeed = 1f;


    //MEMBERS (PRIVATE)
    GameStates _gameState;
    PlayStates _playState;
    CombatStates _combatState;
    ExploreStates _exploreState;
    
    PlayerController _activePlayer;
    List<EnemyController> _activeEnemies = new List<EnemyController>();
    

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


    //UNITY LIFECYCLE
    void Awake() {
        if(instance == null) {
            instance = this;
        }
        else if(instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
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
        initializeEnemies();
        initializeObjects();

        CameraOrthographic.GetComponent<CameraController>().setTarget(_activePlayer.gameObject);
        CameraPerspective.GetComponent<CameraController>().setTarget(_activePlayer.gameObject);

        _playState = PlayStates.Entering;
    }

    void initializePlayer() {
        var player = Instantiate(SelectedPlayer, Vector3.zero, Quaternion.Euler(0, 180, 0));
        _activePlayer = player.GetComponent<PlayerController>();
    }

    void initializeEnemies() {
        var enemy1 = Instantiate(Enemy, new Vector3(-2, 0, -2), Quaternion.Euler(0, 180, 0));
        var enemy2 = Instantiate(Enemy, new Vector3(-1, 0, 3), Quaternion.Euler(0, 0, 0));
        var enemy3 = Instantiate(Enemy, new Vector3(2, 0, 1), Quaternion.Euler(0, -90, 0));

        _activeEnemies.Add(enemy1.GetComponent<EnemyController>());
        _activeEnemies.Add(enemy2.GetComponent<EnemyController>());
        _activeEnemies.Add(enemy3.GetComponent<EnemyController>());
    }

    void initializeObjects() {

    }

    void enter() {
        CameraPerspective.nearClipPlane -= Time.deltaTime * EnterSpeed;
        if(CameraPerspective.nearClipPlane <= 3.8f) {
            CameraPerspective.nearClipPlane = 3.8f;
            _playState = PlayStates.Exploring;
            
            setNewRound();
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
                    //Check for combat here
                    
                    _activeEnemies.ForEach(e => setEnemyExploreAction(e));
                    
                    _exploreState = ExploreStates.EnemyMoving;
                }
            break;

            case ExploreStates.EnemyMoving:
                if(_activeEnemies.All(e => e.movementData.HasMadeAMoveThisTurn) && _activeEnemies.All(e => !e.movementData.IsMoving)) {
                    //Check for combat here

                    setNewRound();
                    _exploreState = ExploreStates.PlayerMoving;
                }
            break;
        }
    }

    void setNewRound() {
        _activePlayer.setNewRound();
        _activeEnemies.ForEach(e => e.setNewRound());
    }

    void setEnemyExploreAction(EnemyController e) {
        e.movementData.setDestination(Vector3.right);
    }


    //PUBLIC METHODS
}
