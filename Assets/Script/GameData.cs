using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static int health=10;
    public static List<Card> Deck;

    public static int Energy=5;
    public static int worldLevelID;//the level ID on the world map
    public static int MaxCardsInHand;

    public static state currentState;

    public enum state // In battle activate drag and drop to play a card, in worldmap activate onmousepointenter to zoom in a card for detailed Information
    {
        Battle,
        WorldMap
    }

    //static List<Hardware> hardwares;
    private void Awake()
    {

        Deck = new List<Card>();
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/RunUp")).GetComponent<RunUp>());
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/Walk")).GetComponent<Walk>());
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/RunDown")).GetComponent<RunDown>());
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/RunLeft")).GetComponent<RunLeft>());
    }

}
