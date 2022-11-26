using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    int health;
    List<Card> Deck;
    int Energy;
    int worldLevelID;//the level ID on the world map
    public int MaxCardsInHand;

    public state currentState;

    public enum state // In battle activate drag and drop to play a card, in worldmap activate onmousepointenter to zoom in a card for detailed Information
    {
        Battle,
        WorldMap
    }

    //static List<Hardware> hardwares;


}
