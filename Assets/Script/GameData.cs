using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static int health;
    public static List<Card> Deck;
    public static int Energy;
    public static int worldLevelID;//the level ID on the world map
    public static int MaxCardsInHand;

    public static state currentState;

    public enum state // In battle activate drag and drop to play a card, in worldmap activate onmousepointenter to zoom in a card for detailed Information
    {
        Battle,
        WorldMap
    }

    //static List<Hardware> hardwares;


}
