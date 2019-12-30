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
    public List<EnemyController> enemies = new List<EnemyController>();
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


    //PRIVATE METHODS
    private void setupInternals() {
    }


    //PUBLIC METHODS
    public EnemyController getEnemy() {
        var randEnemy = Random.Range(0, enemies.Count);
        return enemies[randEnemy];
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