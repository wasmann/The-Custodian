using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleData : MonoBehaviour
{
    public static int BattleLevelCount { get { return 6; } } // just a random nubmer.. to be changed later
    public static int BattleLevelID;
    public static Dictionary<int,EnemyData> EnemyDataList;
    public static PlayerData playerData;

    public static bool PlayingACard = false;//When the player plays a card, set Busy to true, when that card is activated, set this to false
    public static bool AbleToPalyCard = true;
    public static List<Card> NewCard; //for duplication

    public Deck deck;//?????
    public static Tilemap map;
    

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
        public Enemy enemy;//script
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
    

    public static void BattleLevelInit(int battleLevelID)
    {
        map = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
        NewCard = new List<Card>();
        BattleLevelID = battleLevelID;
        LoadEnvironmentData();
        LoadEnermyData();
        LoadPlayerData();       
    }
    public static void LoadEnvironmentData(){
        
    }
    public static void LoadEnermyData(){
        EnemyDataList = new Dictionary<int,EnemyData>();
        //use streamreader to load data. but for test ,just load tree
        EnemyData tree = new EnemyData();
        tree.obj= GameObject.Find("Tree");
        tree.position=new Vector2(9,9);
        tree.handCard = tree.obj.GetComponent<Tree_Enemy>().Deck;
        tree.currentHealth=tree.obj.GetComponent<Tree_Enemy>().Health;
        tree.discardPile = new List<Card>();
        tree.enemy=tree.obj.GetComponent<Enemy>();
        tree.enemy.EnemyID = 1;
        //EnemyDataList.Add(1,tree);

    } 
    public static void LoadPlayerData(){
        playerData.position = BattleData.map.CellToWorld(new Vector3Int(0, 0, 0));
        playerData.maxHealth = GameData.health;
        playerData.maxEnergy = GameData.Energy;
        playerData.currentHealth = playerData.maxHealth;
        playerData.currentEnergy = playerData.maxEnergy;
        playerData.drawPile = GameData.Deck;
        playerData.handCard= new List<Card>();
        playerData.discardPile = new List<Card>();
        StartingHandCards(3);
        
    }

    public static void StartingHandCards(int num)
    {
        for(int i = 0; i < num; i++)
        {
            int randomNum = Random.Range(0, playerData.drawPile.Count);
            playerData.handCard.Add(playerData.drawPile[randomNum]);
            playerData.drawPile.RemoveAt(randomNum);      
        }
        UI.SetOtherPilesInative();
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
