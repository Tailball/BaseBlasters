using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMover : MonoBehaviour
{
    //UNITY LINKS
    [Header("Tweaking")]
    [SerializeField] float MovementSpeed = 1f;


    //MEMBERS (PRIVATE)
    Rigidbody _rb = null;
    Animator _anim = null;
    
    Vector3 _destination;
    float _rotDestinationY;
    bool _madeAMoveThisTurn;
    bool _isMoving;


    //ACCESSORS - MUTATORS (PUBLIC)
    public Vector3 Position {
        get { return new Vector3(transform.position.x, 0, transform.position.z); }
    }

    public bool HasMadeAMoveThisTurn {
        get { return _madeAMoveThisTurn; }
    }

    public bool IsMoving {
        get { return _isMoving; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Start() {
        _destination = transform.position;
        _madeAMoveThisTurn = false;
        _isMoving = false;
    }

    void Update() {
    }

    void FixedUpdate() {
        move();
        rotate();
    }


    //PRIVATE METHODS
    void move() {
        if(!_isMoving) return;

        var intermediatePosition = Vector3.MoveTowards(transform.position, _destination, Time.fixedDeltaTime * this.MovementSpeed);
        _rb.MovePosition(intermediatePosition);

        var diffX = Mathf.Abs(transform.position.x - _destination.x);
        var diffY = Mathf.Abs(transform.position.z - _destination.z);
        if(diffX <= 0.01f && diffY <= 0.01f) {
            _rb.MovePosition(_destination);
            _isMoving = false;
        }
    }

    void rotate() {
        if(!_isMoving) return;

        _rb.MoveRotation(Quaternion.Euler(0, _rotDestinationY, 0));
    }


    //PUBLIC METHODS
    public void setMovementDirection(Vector3 action) {
        var currRotationAngle = transform.rotation.eulerAngles.y;

        if(action.x != 0) {
            _destination = new Vector3(transform.position.x + action.x, transform.position.y, transform.position.z);
            _rotDestinationY = action.x > 0 ? 90 : 270; 
        }
        else if(action.z != 0) {
            _destination = new Vector3(transform.position.x, transform.position.y, transform.position.z + action.z);
            _rotDestinationY = action.z > 0 ? 0 : 180;
        }

        _anim.SetTrigger("Move");
        _madeAMoveThisTurn = true;
        _isMoving = true;
    }

    public void setNewRound() {
        _madeAMoveThisTurn = false;
        _isMoving = false;
    }

    public bool isValidLocation(Vector3 loc) {
        var floorUnits = Physics.OverlapSphere(loc, .2f, GameController.instance.FloorLayer);
        var wallUnits = Physics.OverlapSphere(loc, .2f, GameController.instance.WallLayer);

        if(floorUnits.Length > 0 && wallUnits.Length <= 0) return true;

        return false;
    }
}