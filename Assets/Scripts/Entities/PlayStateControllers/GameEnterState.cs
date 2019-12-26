using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameEnterState : MonoBehaviour
{
    //UNITY LINKS
    [Header("Tweaking")]
    [SerializeField][Range(1f, 5f)] float EnterSpeed = 3.25f;


    //MEMBERS (PRIVATE)
    private Camera _camera;


    //ACCESSORS - MUTATORS (PUBLIC)


    //UNITY LIFECYCLE
    void Awake() {
    }


    //PRIVATE METHODS
    void execEnter() {
        _camera.nearClipPlane -= Time.deltaTime * EnterSpeed;

        if(_camera.nearClipPlane <= 3.8f) {
            _camera.nearClipPlane = 3.8f;
            GameController.instance.changePlaystateToExploring();
        }
    }


    //PUBLIC METHODS
    public void exec() {
        execEnter();
    }

    public void setCamera(Camera cam) {
        _camera = cam;
    }
}