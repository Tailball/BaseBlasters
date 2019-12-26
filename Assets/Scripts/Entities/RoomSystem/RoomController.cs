using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnemiesController))]
[RequireComponent(typeof(TilemapController))]
public class RoomController : MonoBehaviour
{
    //UNITY LINKS


    //MEMBERS (PRIVATE)
    private EnemiesController _enemiesController;
    private TilemapController _tilemapController;


    //ACCESSORS - MUTATORS (PUBLIC)
    public EnemiesController enemies {
        get { return _enemiesController; }
    }

    public TilemapController tilemap {
        get { return _tilemapController; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _enemiesController = GetComponent<EnemiesController>();
        _tilemapController = GetComponent<TilemapController>();
    }

    void Start() {
    }

    void Update() {
    }

    void FixedUpdate() {
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void setNewRound() {
        _enemiesController.setNewRound();
    }
}
