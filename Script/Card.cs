using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    //every card is a prefab and has its own script named card_cardname and this one is a superclass
    string name;
    Rarity rarity;
    public int speed;
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
        public List<Vector2> TargetOrDirection;
        public int ID_WhoPlayedTheCard;// 0 for player
        public Card card;
    }

    public abstract void Playerplay();// shownotion, wait for choose a dir or a target, 





    public abstract void Acitvate(InfoForActivate info);

    //Drag and drop function to play the card

}
