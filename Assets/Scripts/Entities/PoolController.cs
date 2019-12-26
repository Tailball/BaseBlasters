using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//FIXME: class needs full work done
public class PoolController : MonoBehaviour
{
    //SINGLETON PATTERN
    public static PoolController instance;


    //UNITY LINKS
    public EnemyController testEnemy = null;
    public RoomController testRoom = null;
    public List<CardController> cards = new List<CardController>();


    //MEMBERS (PRIVATE)


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
    }


    //PUBLIC METHODS
    public EnemyController getEnemy() {
        return testEnemy;
    }

    public RoomController getRoom() {
        return testRoom;
    }

    public CardController getCard() {
        if(cards.Count > 0) {
            var c = cards.First();
            cards.RemoveAt(0);

            return c;
        }
        
        return null;
    }
}