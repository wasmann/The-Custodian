using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enermy : MonoBehaviour
{
    //every enermy is a prefab and has its own script named EnermyName and this one is a superclass
    string name;
    int health;
    List<Card> Deck;
    BattleData battleData;

    public abstract void EnermyChooseACardToPlay(BattleData.EnermyData data); // BattleLevelDriver.NewcardPlayed(Card.InfoForActivate info)

}
