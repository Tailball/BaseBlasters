using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Tweaking")]
    [SerializeField] float MovementSpeed = 1f;
    

    //MEMBERS (PRIVATE)
    Rigidbody _rb = null;
    Animator _anim = null;
    
    Vector3 _destination;
    float _rotDestinationY;
    bool _isMoving;


    //ACCESSORS - MUTATORS (PUBLIC)
    public Vector3 Position {
        get { return new Vector3(transform.position.x, 0, transform.position.z); }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Start() {
        _destination = transform.position;
        _isMoving = false;
    }

    void Update() {
        getInput();
    }

    void FixedUpdate() {
        rotate();
        move();
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

    void setDestination(Vector3 action) {
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
        _isMoving = true;
    }

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
}
