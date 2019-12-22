using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//FIXME: class needs full work done
public class PoolController : MonoBehaviour
{
    //SINGLETON PATTERN
    public static PoolController instance;


    //UNITY LINKS
    public GameObject testEnemy = null;
    public GameObject testRoom = null;

    //MEMBERS (PRIVATE)
    private CardPool _cardpool;
    private EnemyPool _enemypool;
    private RoomPool _roompool;


    //ACCESSORS - MUTATORS (PUBLIC)


    //UNITY LIFECYCLE
    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            setupInternals();
        } 
        else if(instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
    }

    void Update() {
    }

    void FixedUpdate() {
    }


    //PRIVATE METHODS
    private void setupInternals() {
        _cardpool = new CardPool();
        _enemypool = new EnemyPool();
        _roompool = new RoomPool();
    }


    //PUBLIC METHODS
    public GameObject getEnemy() {
        return testEnemy;
    }

    public GameObject getRoom() {
        return testRoom;
    }
}