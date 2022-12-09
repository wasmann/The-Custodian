using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    public static int BattleLevelID;
    public static Dictionary<int,EnemyData> EnemyDataList;
    public static PlayerData playerData;

    public static Card CardReadyToPlay;
    //public n* m Matrix environmentData;

    public Deck deck;
    

    public struct EnemyData
    {
        public Vector2 position;
        public int maxHealth;
        public int currentHealth;
        public int ID;
        public List<Card> handCard;
        public HashSet<Card> discardPile;
        public HashSet<Card> drawPile;
        public Enemy enemy;
    }

    public struct PlayerData
    {
        public Vector2 position;
        public int maxHealth;
        public int currentHealth;
        public int maxEnergy;
        public int currentEnergy;
        public List<Card> handCard;
        public HashSet<Card> discardPile;
        public HashSet<Card> drawPile;
    }
    public HashSet<int> NewCard; 

    public static void BattleLevelInit(int battleLevelID)
    {
        BattleLevelID = battleLevelID;
        LoadEnvironmentData();
        LoadEnermyData();
        LoadPlayerData();       
    }
    public static void LoadEnvironmentData(){
        
    }
    public static void LoadEnermyData(){

    } 
    public static void LoadPlayerData(){
        playerData.maxHealth = GameData.health;
        playerData.maxEnergy = GameData.Energy;
        playerData.currentHealth = playerData.maxHealth;
        playerData.currentEnergy = playerData.maxEnergy;
        (playerData.drawPile,playerData.handCard)= Deck.StartingHandCards(4);
        playerData.discardPile = new HashSet<Card>();
    } 

    public bool CheckWinCondition()
    {
        if(EnemyDataList.Count==0)
            return true;
        else
            return false;
    }
    public bool CheckLoseCondition()
    {
        if (playerData.currentHealth == 0)
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
