using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //SINGLETON
    public static GameController instance;


    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] Camera CameraOrthographic = null;
    [SerializeField] Camera CameraPerspective = null;
    [SerializeField] GameObject SelectedPlayer = null; //This should change to character select that will pass on the selected playercontroller;

    [Header("Tweaking")]
    [SerializeField][Range(1f, 5f)] float EnterSpeed = 1f;


    //MEMBERS (PRIVATE)
    GameStates _gameState;
    PlayStates _playState;
    CombatStates _combatState;
    ExploreStates _exploreState;
    PlayerController _activePlayer;


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
            break;

            case PlayStates.Combat:
            break;

            case PlayStates.Exiting:
            break;
        }
    }

    void initialize() {
        var player = Instantiate(SelectedPlayer, Vector3.zero, Quaternion.Euler(0, 180, 0));
        _activePlayer = player.GetComponent<PlayerController>();

        CameraOrthographic.GetComponent<CameraController>().setTarget(player);
        CameraPerspective.GetComponent<CameraController>().setTarget(player);

        _playState = PlayStates.Entering;
    }

    void enter() {
        CameraPerspective.nearClipPlane -= Time.deltaTime * EnterSpeed;
        if(CameraPerspective.nearClipPlane <= 3.85f) {
            CameraPerspective.nearClipPlane = 3.85f;
            _playState = PlayStates.Exploring;
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


    //PUBLIC METHODS
}
