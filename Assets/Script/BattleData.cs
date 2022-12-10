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
        public List<Card> discardPile;
        public List<Card> drawPile;
        public GameObject obj;
    }

    public struct PlayerData
    {
        public Vector2 position;
        public int maxHealth;
        public int currentHealth;
        public int maxEnergy;
        public int currentEnergy;
        public List<Card> handCard;
        public List<Card> discardPile;
        public List<Card> drawPile;
    }
    public static List<int> NewCard; //for duplication

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
        //use streamreader to load data. but for test ,just load tree
        EnemyData tree = new EnemyData();
        tree.obj= GameObject.Find("Tree");
        tree.position=new Vector2(9,9);
        tree.handCard = tree.obj.GetComponent<Tree_Enemy>().Deck;
        tree.discardPile = new List<Card>();


    } 
    public static void LoadPlayerData(){
        playerData.maxHealth = GameData.health;
        playerData.maxEnergy = GameData.Energy;
        playerData.currentHealth = playerData.maxHealth;
        playerData.currentEnergy = playerData.maxEnergy;
        playerData.drawPile= new List<Card>();
        playerData.handCard= new List<Card>();
        (playerData.drawPile,playerData.handCard)= Deck.StartingHandCards(4);
        playerData.discardPile = new List<Card>();
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
