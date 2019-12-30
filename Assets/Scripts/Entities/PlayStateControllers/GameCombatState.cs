using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameCombatState : MonoBehaviour
{
    //UNITY LINKS


    //MEMBERS (PRIVATE)
    CombatStates _state;
    EnemyController _activeEnemy;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CombatStates state {
        get { return _state; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _state = CombatStates.DrawingCards;
    }


    //PRIVATE METHODS
    void execCombat() {
        switch(_state) {
            case CombatStates.DrawingCards:
                DeckController.instance.drawCardsToHand(3);
                GameController.instance.setStatsUI(GameController.instance.activePlayer, _activeEnemy);
                _state = CombatStates.EnemyTurn;
            break;

            case CombatStates.NewRound:
                execNewRound();
            break;

            case CombatStates.EnemyTurn:
                execEnemyTurn();
            break;

            case CombatStates.EnemyTurnCoroutine:
            break;

            case CombatStates.PlayerTurn:
                //Wait for cards (func playerMadeCardMove())
                //Will then automatically go to resolve
            break;

            case CombatStates.Resolve:
                execResolve();
            break;

            case CombatStates.ResolveCoroutine:
            break;

            case CombatStates.EndCombat:
                execEndCombat();
            break;        

            case CombatStates.EndCombatCoroutine:
            break;
        }
    }

    void execNewRound() {
        var dInst = DeckController.instance;
        dInst.moveAllDropPointsToDiscard();

        if(dInst.amountOfCardsInDeck <= 0 && dInst.amountOfCardsInHand <= 0) {
            Debug.Log("New Round -> Game Over");
            GameController.instance.changePlaystateToGameOver();
        }
        else {
            dInst.drawCardsToHand(1);
            _state = CombatStates.EnemyTurn;
        }
    }

    void execEnemyTurn() {
        StartCoroutine(runEnemyTurn());
        _state = CombatStates.EnemyTurnCoroutine;
    }

    IEnumerator runEnemyTurn() {
        yield return new WaitForSeconds(.5f);

        _activeEnemy.playCard();

        yield return new WaitForSeconds(.65f);
        
        _state = CombatStates.PlayerTurn;
    }

    void execResolve() {
        StartCoroutine(runResolveCombat());
        _state = CombatStates.ResolveCoroutine;
    }

    IEnumerator runResolveCombat() {
        yield return new WaitForSeconds(.65f);

        var gc = GameController.instance;
        var activePlayer = gc.activePlayer;
        var dInst = DeckController.instance;

        dInst.playerDrop.use(activePlayer, _activeEnemy);
        dInst.enemyDrop.use(activePlayer, _activeEnemy);
        
        var playerHp = activePlayer.combatData.resolveAndReportHP(activePlayer.name);
        var enemyHp = _activeEnemy.combatData.resolveAndReportHP(_activeEnemy.name);

        GameController.instance.setStatsUI(activePlayer, _activeEnemy);

        yield return new WaitForSeconds(.5f);
        
        if(playerHp <= 0) {
            Destroy(activePlayer);
            gc.changePlaystateToGameOver();
        }
        else if(enemyHp <= 0) {
            _state = CombatStates.EndCombat;
        }
        else {
            _state = CombatStates.NewRound;
        }
    }

    void execEndCombat() {
        StartCoroutine(runEndCombat());
        _state = CombatStates.EndCombatCoroutine;
    }

    IEnumerator runEndCombat() {
        yield return new WaitForSeconds(.5f);

        var dInst = DeckController.instance;
        dInst.moveAllDropPointsToDiscard();
        dInst.moveHandToDeck();

        var gc = GameController.instance;
        var activePlayer = gc.activePlayer;
        activePlayer.movementData.jumpBack(true);
        gc.activeRoom.enemies.killEnemy(_activeEnemy);

        yield return new WaitForSeconds(.3f);

        _state = CombatStates.DrawingCards;
        GameController.instance.changePlaystateToExploring();
    }


    //PUBLIC METHODS
    public void exec() {
        execCombat();
    }

    public void setActiveEnemy(EnemyController activeEnemy) {
        _activeEnemy = activeEnemy;
    }

    public void acceptPlayerMove() {
        _state = CombatStates.Resolve;
    }
}