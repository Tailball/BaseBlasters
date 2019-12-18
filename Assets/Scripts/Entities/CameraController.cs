using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    //UNITY LINKS
    

    //MEMBERS (PRIVATE)
    Vector3 _offset = Vector3.zero;
    GameObject _target = null;


    //ACCESSORS - MUTATORS (PUBLIC)


    //UNITY LIFECYCLE
    void Awake() {
    }

    void Start() {
    }

    void Update() {
        followTarget();
    }

    void FixedUpdate() {
    }


    //PRIVATE METHODS
    void followTarget() {
        if(_target == null) return;

        transform.position = _target.transform.position + _offset;
    }

    void calculateOffset() {
        if(_target == null) return;

        _offset = transform.position - _target.transform.position;
    }


    //PUBLIC METHODS
    public void setTarget(GameObject newTarget) {
        _target = newTarget;
        calculateOffset();
    }
}