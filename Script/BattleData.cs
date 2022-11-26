using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    int BattleLevelID;
    List<EnermyData> EnermyDataList;
    PlayerData playerData;
    n* m Matrix environmentData;

    public Deck deck;
    

    public struct EnermyData
    {
        Vector2 position;
        int Health;
        int ID;
        List<Card> Handcard;
        List<Card> DiscardPile;
        List<Card> DrawPile;
        Enermy enermy;
    }

    struct PlayerData
    {
        Vector2 position;
        int health;
        int Energy;
        List<Card> Handcard;
        List<Card> DiscardPile;
        List<Card> DrawPile;
    }



    public void LoadBattlelevel(int battleLevelID)
    {
        BattleLevelID = battleLevelID;
        LoadEnvironmentData();
        LoadEnermyData();
        LoadPlayerData();
        deck.StartingHandCards(4);// MaxCardsInHand
    }
    public void LoadEnvironmentData(){}
    public void LoadEnermyData(){ }
    public void LoadPlayerData(){ } 

    public bool CheckWinCondition()
    {
        return false;
    }
    public bool CheckLoseCondition()
    {
        return false;
    }
    void Start()
    {
    }

    void Update()
    {
        
    }
}
