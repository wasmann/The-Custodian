using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    public int BattleLevelID;
    public Dictionary<int,EnermyData> EnermyDataList;
    public PlayerData playerData;
    //public n* m Matrix environmentData;

    public Deck deck;
    

    public struct EnermyData
    {
        public Vector2 position;
        public int health;
        public int ID;
        public List<Card> handCard;
        public List<Card> discardPile;
        public List<Card> drawPile;
        public Enermy enermy;
        //public bool Busy;// by default enermy can only let one card preparing at the same time
    }

    public struct PlayerData
    {
        public Vector2 position;
        public int health;
        public int energy;
        public List<Card> handCard;
        public List<Card> discardPile;
        public List<Card> drawPile;
    }
    public HashSet<int> NewCard; 

    public void LoadBattlelevel(int battleLevelID)
    {
        BattleLevelID = battleLevelID;
        LoadEnvironmentData();
        LoadEnermyData();
        LoadPlayerData();
        
    }
    public void LoadEnvironmentData(){

    }
    public void LoadEnermyData(){

    } 
    public void LoadPlayerData(){
        playerData.health = GameData.health;
        playerData.energy = GameData.Energy;
        playerData.health = GameData.health;
        (playerData.drawPile,playerData.handCard)=Deck.StartingHandCards(4,this);
        playerData.discardPile = new List<Card>();
    } 

    public bool CheckWinCondition()
    {
        if(EnermyDataList.Count==0)
            return true;
        else
            return false;
    }
    public bool CheckLoseCondition()
    {
        if (playerData.health == 0)
            return true;
        else
            return false;
    }
    void Start()
    {
    }

    void Update()
    {
        
    }
}
