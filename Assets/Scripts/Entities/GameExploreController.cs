using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameExploreController : MonoBehaviour
{
    //UNITY LINKS
    [SerializeField] Camera CameraOrthographic = null;
    [SerializeField] Camera CameraPerspective = null;
    [SerializeField] TMP_Text txtRoundNum = null;


    //MEMBERS (PRIVATE)
    ExploreStates _state;
    int _round = 0;

    EnemyController _activeEnemy;


    //ACCESSORS - MUTATORS (PUBLIC)
    public ExploreStates state {
        get { return _state; }
    }

    public Camera cameraPerspective {
        get { return CameraPerspective; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _state = ExploreStates.PlayerMoving;
    }


    //PRIVATE METHODS
    void getExploringInput() {
        if(GameController.instance.gameState != GameStates.Ingame) return;

        if(Input.GetKeyDown(KeyCode.C)) {
            switchCamera();
        }
    }

    void switchCamera() {
        CameraOrthographic.gameObject.SetActive(!CameraOrthographic.gameObject.activeInHierarchy);
        CameraPerspective.gameObject.SetActive(!CameraPerspective.gameObject.activeInHierarchy);
    }

    void execExplore() {
        var gc = GameController.instance;

        var activePlayer = gc.activePlayer;
        var enemies = gc.activeRoom.enemies;

        switch(_state) {
            case ExploreStates.PlayerMoving:
                var playerMover = activePlayer.movementData;

                if(playerMover.hasMadeAMoveThisTurn && !playerMover.isMoving) {
                    var combat = checkForCombat();

                    if(!combat) {
                        enemies.setAllEnemiesExploreAction(activePlayer.transform.position);
                        _state = ExploreStates.EnemyMoving;
                    }
                    else {
                        setNewExploreRound();
                        _state = ExploreStates.PlayerMoving;
                    }
                }
            break;

            case ExploreStates.EnemyMoving:
                var allEnemiesMadeAMoveThisTurn = enemies.haveAllEnemiesMadeAMoveThisTurn();
                var allEnemiesDoneMoving = enemies.haveAllEnemiesStoppedMoving();

                if(allEnemiesMadeAMoveThisTurn && allEnemiesDoneMoving) {
                    setNewExploreRound();
                    var combat = checkForCombat();

                    if(!combat) {
                        _state = ExploreStates.PlayerMoving;
                    }
                    else {
                        enemies.setAllEnemiesExploreAction(activePlayer.transform.position);
                        _state = ExploreStates.EnemyMoving;
                    }
                }
            break;
        }
    }

    bool checkForCombat() {
        var gc = GameController.instance;

        var colliders = Physics.OverlapSphere(gc.activePlayer.transform.position, .2f, gc.npcLayer);
        
        if(colliders.Length > 0) {
            _activeEnemy = colliders[0].GetComponent<EnemyController>();
            gc.changePlaystateToCombat(_activeEnemy);

            return true;
        }

        return false;
    }


    //PUBLIC METHODS
    public void exec() {
        getExploringInput();
        execExplore();
    }

    public void setExploringCameraTarget(GameObject target) {
        CameraOrthographic.GetComponent<CameraController>().setTarget(target);
        CameraPerspective.GetComponent<CameraController>().setTarget(target);
    }

    public void setNewExploreRound() {
        var gc = GameController.instance;

        _round++;

        gc.activePlayer.setNewRound();
        gc.activeRoom.setNewRound();

        txtRoundNum.text = _round.ToString();
    }
}