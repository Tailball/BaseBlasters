using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemiesController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] GameObject EnemyHolder = null;


    //MEMBERS (PRIVATE)
    private List<EnemyController> _enemiesInRoom = new List<EnemyController>();


    //ACCESSORS - MUTATORS (PUBLIC)


    //UNITY LIFECYCLE
    void Awake() {
    }

    void Start() {
    }

    void Update() {
    }

    void FixedUpdate() {
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void instantiateEnemies() {
        //TODO: determine how many enemies and where
        
        var enemy = Instantiate(PoolController.instance.getEnemy(), new Vector3(-2, 0, 1), Quaternion.identity, EnemyHolder.transform);
        var enemyController = enemy.GetComponent<EnemyController>();
        _enemiesInRoom.Add(enemyController);

        var enemy2 = Instantiate(PoolController.instance.getEnemy(), new Vector3(7, 0, 3), Quaternion.identity, EnemyHolder.transform);
        var enemyController2 = enemy2.GetComponent<EnemyController>();
        _enemiesInRoom.Add(enemyController2);
    }

    public bool haveAllEnemiesMadeAMoveThisTurn() {
        return this._enemiesInRoom.All(e => e.movementData.hasMadeAMoveThisTurn);
    }

    public bool haveAllEnemiesStoppedMoving() {
        return this._enemiesInRoom.All(e => !e.movementData.isMoving);
    }

    public void setNewRound() {
        this._enemiesInRoom.ForEach(e => e.setNewRound());
    }

    public void setAllEnemiesExploreAction(Vector3 playerPos) {
        this._enemiesInRoom.ForEach(e => e.setExploreAction(playerPos));
    }

    public void killEnemy(EnemyController enemy) {
        _enemiesInRoom.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}