using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] string CardName;
    [SerializeField] Sprite CardImage = null; //Could also do this manually by just inserting the image
    [SerializeField][Multiline] string CardDescription;
    [SerializeField] CardTypes CardType;
    

    //MEMBERS (PRIVATE)
    private bool _isDragging = false;
    private Vector3 _positionBeforeDragging;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public string Name {
        get { return CardName; }
    }

    public string Description {
        get { return CardDescription; }
    }


    //UNITY LIFECYCLE
    void Awake() {

    }

    void Start() {
        
    }

    void Update() {
        
    }

    void OnMouseDown() {
        _positionBeforeDragging = transform.position;
        _isDragging = true;
    }

    void OnMouseUp() {
        _isDragging = false;
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
}
