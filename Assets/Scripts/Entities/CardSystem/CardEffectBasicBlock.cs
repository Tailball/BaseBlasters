using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffectBasicBlock : MonoBehaviour,
    ICardEffect
{
    //UNITY LINKS
    [Header("Tweaking")]
    [SerializeField] int defense = 1;


    //MEMBERS (PRIVATE)


    //ACCESSORS - MUTATORS (PUBLIC)
    public int executionOrder { 
        get { return 2; } 
    }

    public string statusText { 
        get { return $"Defense {defense}"; }
    }


    //UNITY LIFECYCLE


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void execEffect(PlayerController player, EnemyController enemy, CardTypes cardtype) {
        if(cardtype == CardTypes.Combat) {
            player.combatData.addDefense(defense);
        }

        else if(cardtype == CardTypes.Enemy) {
            enemy.combatData.addDefense(defense);
        }
    }
}