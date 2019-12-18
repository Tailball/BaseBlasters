using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBaseController
{
    //UNITY LINKS


    //MEMBERS (PRIVATE)
    

    //ACCESSORS - MUTATORS (PUBLIC)


    //UNITY LIFECYCLE
    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        getInput();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }


    //PRIVATE METHODS
    void getInput() {  
        if(_isMoving) return;
        if(GameController.instance.playState != PlayStates.Exploring) return;
        if(GameController.instance.exploreState != ExploreStates.PlayerMoving) return;

        var xAxis = Input.GetAxisRaw("Horizontal");
        var yAxis = Input.GetAxisRaw("Vertical");

        if(xAxis != 0 || yAxis != 0) {
            setDestination(new Vector3(xAxis, 0, yAxis));
        }
    }


    //PUBLIC METHODS
}
