﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CombatResolver))]
[RequireComponent(typeof(CharacterMover))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //UNITY LINKS


    //MEMBERS (PRIVATE)
    CharacterMover _mover = null;
    CombatResolver _resolver = null;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CharacterMover movementData {
        get { return _mover ;}
    }

    public CombatResolver combatData {
        get { return _resolver; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _mover = GetComponent<CharacterMover>();
        _resolver = GetComponent<CombatResolver>();
    }

    void Update() {
        getInput();
    }


    //PRIVATE METHODS
    void getInput() {  
        if(_mover.isMoving) return;
        if(GameController.instance.playState != PlayStates.Exploring) return;
        if(GameController.instance.passedExploreState != ExploreStates.PlayerMoving) return;

        var xAxis = Input.GetAxisRaw("Horizontal");
        var yAxis = Input.GetAxisRaw("Vertical");

        if(xAxis != 0 || yAxis != 0) {
            var dir = new Vector3(xAxis, 0, yAxis);
            var desiredLocation = transform.position + dir;
            
            if(_mover.isValidLocation(desiredLocation)) {
                _mover.setMovementDirection(dir);
            }
        }
    }


    //PUBLIC METHODS
    public void setNewRound() {
        _mover.setNewRound();
    }
}
