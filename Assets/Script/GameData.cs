using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameData : MonoBehaviour
{
    public static int health=10;
    public static List<Card> Deck = new List<Card>();

    public static int Energy=5;
    public static int worldLevelID;//the level ID on the world map
    public static int MaxCardsInHand;

    public static state currentState;

    public static float tickspeed = 1;
    public static int killed;
    public static int duplicated;
    public static int accessible = 2;
    public static bool upgraded;
    public static bool enteredEventLevel = false;
    public enum state // In battle activate drag and drop to play a card, in worldmap activate onmousepointenter to zoom in a card for detailed Information
    {
        Battle,
        WorldMap
    }

    //static List<Hardware> hardwares;
    private void Awake()
    {

        /*Deck = new List<Card>();
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/RunUp"), GameObject.Find("CardBank").transform).GetComponent<RunUp>());
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/Walk"), GameObject.Find("CardBank").transform).GetComponent<Walk>());
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/RunDown"), GameObject.Find("CardBank").transform).GetComponent<RunDown>());
        Deck.Add(GameObject.Instantiate(GameObject.Find("CardBank/RunLeft"), GameObject.Find("CardBank").transform).GetComponent<RunLeft>());*/

        /*PlayerPrefs.DeleteAll();
        SaveCard(1, "RunUp");
        SaveCard(2, "Walk");
        SaveCard(3, "RunDown");
        SaveCard(4, "RunLeft");
        SaveCard(5, "RunRight");
        SaveCard(6, "Headbutt");*/
        LoadCard();
    }

    public static void SaveCard(int key, string value)
    {
        PlayerPrefs.SetString(key.ToString(),value);
        PlayerPrefs.Save();
        SaveCardNumber(key);
    }

    public static void SaveCardNumber(int num)
    {
        PlayerPrefs.SetInt("number", num);
        PlayerPrefs.Save();
    }

    public static int GetCardNumber()
    {
        return PlayerPrefs.GetInt("number");
    }
    public static void LoadCard()
    {
        Deck = new List<Card>();
        int num = PlayerPrefs.GetInt("number");
        for(int i = 1; i < num+1; ++i)
        {
            string name = PlayerPrefs.GetString(i.ToString(),"deleted");
            if(name == "deleted")
            {
                continue;
            }
            Type t = Type.GetType(name);
            Deck.Add((Card)Instantiate( GameObject.Find("CardBank/" + name), GameObject.Find("CardBank").transform).GetComponent(t));
        }
    }

    public static void DeleteCard(string name)
    {
        for(int i = 1; i <= GetCardNumber(); ++i)
        {
            if(PlayerPrefs.GetString(i.ToString()) == name)
            {
                PlayerPrefs.DeleteKey( i.ToString());
                LoadCard();
                break;
            }
        }
        PlayerPrefs.Save();
    }
}
