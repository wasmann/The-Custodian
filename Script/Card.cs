using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    //every card is a prefab and has its own script named card_cardname and this one is a superclass
    string name;
    Rarity rarity;
    int speed;
    public BattleData battleData;
    string range;//describe the range
    GameObject notation;
    
    enum Rarity
    {
        basic,
        common,
        rare,
        epic,
        legendary,
    }

    public struct InfoForActivate
    {
        List<Vector2> TargetOrDirection;
        int ID_WhoPlayedTheCard;// 0 for player
        Card card;
    }

    public abstract void Playerplay();// shownotion, wait for choose a dir or a target, 





    protected abstract void Acitvate(InfoForActivate info);

    //Drag and drop function to play the card

}
