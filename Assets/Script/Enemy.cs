using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //every enermy is a prefab and has its own script named EnermyName and this one is a superclass
    public string EnemyName;
    public int Health;
    public List<Card> Deck;
    public int EnemyID;//Not the id in BattleData

    public abstract Card.InfoForActivate EnermyChooseACardToPlay(BattleData dat,int ID); 

}
