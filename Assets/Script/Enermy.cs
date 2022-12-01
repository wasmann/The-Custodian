using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //every enermy is a prefab and has its own script named EnermyName and this one is a superclass
    int ID;
    string name;
    int health;
    List<Card> Deck;


    public abstract void EnermyChooseACardToPlay(BattleData data); // BattleLevelDriver.NewcardPlayed(Card.InfoForActivate info)

}
