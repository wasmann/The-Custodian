using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleData : MonoBehaviour
{
    public static int BattleLevelCount { get { return 6; } } // just a random nubmer.. to be changed later
    public static int BattleLevelID;
    public static Dictionary<int,EnemyData> EnemyDataList;
    public static PlayerData playerData;
    public static Dictionary<Vector2, EnvironmentType> enviromentData;

    public static bool PlayingACard = false;//When the player plays a card, set Busy to true, when that card is activated, set this to false
    public static bool AbleToPalyCard = true;
    public static HashSet<Card> NewCard; //for duplication
    public static HashSet<Card> duplicated;

    public Deck deck;//?????
    

    public struct EnemyData
    {
        public Vector2 position;
        public Enemy enemy;//script
        public int maxHealth;
        public int currentHealth;
        //public int ID;
        public List<Card> handCard;
        public List<Card> discardPile;
        public List<Card> drawPile;
        public GameObject obj;
        public BuffAndDebuff.Buff buff;        
    }


    public enum EnvironmentType
    {
        Walkable,
        Obstacle,
        //exit or electric wall
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
        public BuffAndDebuff.Buff buff;
    }
    

    public static void BattleLevelInit(int battleLevelID)
    {
        NewCard = new HashSet<Card>();
        duplicated = new HashSet<Card>();
        BattleLevelID = battleLevelID;
        LoadEnvironmentData();
        LoadEnermyData();
        LoadPlayerData();       
    }
    public static void LoadEnvironmentData()
    {
        enviromentData = new Dictionary<Vector2, EnvironmentType>();
        Tilemap tilemap = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    if (tile.name.Contains("CRATE_1") || tile.name.Contains("RIVET") || tile.name.Contains("WARN")) //retrieve from level info
                    {
                        enviromentData.Add(new Vector2(x, y), EnvironmentType.Walkable);
                        Debug.Log("x " + x + " y " + y + "   walk" + tile.name);
                    }
                    else
                    {
                        enviromentData.Add(new Vector2(x, y), EnvironmentType.Obstacle);
                        Debug.Log("x " + x + " y " + y + "  nope" + tile.name);
                    }

                }
            }
        }
        //enviromentData = new Dictionary<Vector2, EnvironmentType>();
        //string path = "Assets/Scenes/SceneDoc/BattleLevel" + BattleLevelID + ".txt";
        //StreamReader reader = new StreamReader(path);
        //string line;
        //while ((line = reader.ReadLine()) != null)
        //{
        //    string[] split = line.Split(',');          
        //    EnvironmentType environmentType = (EnvironmentType)System.Enum.Parse(typeof(EnvironmentType), split[2]);
        //    enviromentData.Add(new Vector2(int.Parse(split[0]), int.Parse(split[1])), environmentType);
        //}
        //reader.Close();
        //Debug.Log(enviromentData.Count);
        //Debug.Log(enviromentData.ElementAt(0).Value);
    }

    public static void LoadEnermyData(){
        EnemyDataList = new Dictionary<int,EnemyData>();
        //use streamreader to load data.
        //Thefollowing code is hardcode for testing.
        switch (BattleLevelID)
        {
            case 0:
                LoadEnemyForTutorial();
                break;
            case 1:
                LoadEnemyForLevel1();
                break;
            case 2:
                LoadEnemyForLevel2();
                break; 
        }

    }
    public static void LoadPlayerData(){
        playerData.position = new Vector2(2,2);
        playerData.maxHealth = GameData.health;
        playerData.maxEnergy = GameData.Energy;
        playerData.currentHealth = playerData.maxHealth;
        playerData.currentEnergy = playerData.maxEnergy;
        playerData.drawPile = GameData.Deck;
        playerData.handCard= new List<Card>();
        playerData.discardPile = new List<Card>();
        StartingHandCards(4, playerData.handCard, playerData.drawPile,true);
        
    }

    public static void StartingHandCards(int num,List<Card> handcards,List<Card> drawPile,bool ShouldSetUI)
    {
        for(int i = 0; i < num; i++)
        {
           
            /*int randomNum = Random.Range(0, drawPile.Count);
            handcards.Add(drawPile[randomNum]);
            drawPile.RemoveAt(randomNum);*/

            handcards.Add(drawPile[0]);
            drawPile.RemoveAt(0);
        }
        if(ShouldSetUI)
            UI.SetOtherPilesInative();
    }

    public static bool CheckWinCondition()
    {
        if (EnemyDataList.Count == 0)
        {
            GameData.currentState = GameData.state.WorldMap;
            //SceneManager.LoadScene("WinningScreen");
            return true;
        }
            
        else
            return false;
    }
    public static bool CheckLoseCondition()
    {
        if (playerData.currentHealth <= 0)
        {
            GameData.currentState = GameData.state.WorldMap;
            //SceneManager.LoadScene("FailedScreen");
            return true;
        }
            
        else
            return false;
    }
    void Start()
    {
    }

    void Update()
    {
        CheckLoseCondition();
        CheckWinCondition();
    }

    static void LoadEnemyForTutorial()
    {
        //EnemyData tree = new EnemyData();  
        //tree.position=new Vector2(4,-1);
        //tree.obj = GameObject.Find("TreeEnemy");
        //tree.enemy= tree.obj.GetComponent<Tree_Enemy>();
        //tree.enemy.EnemyID = 1;
        //tree.currentHealth= tree.enemy.Health;
        //tree.maxHealth = tree.enemy.Health;
        //tree.drawPile = tree.enemy.CardsDeck;
        //tree.handCard = new List<Card>();
        //tree.discardPile = new List<Card>();
        //StartingHandCards(1, tree.handCard, tree.drawPile, false);
        //EnemyDataList.Add(1,tree);
        EnemyData sheep = new EnemyData();
        sheep.position = new Vector2(7, 4);
        sheep.obj = GameObject.Find("Sheep");
        sheep.enemy = sheep.obj.GetComponent<SheepEnemy>();
        sheep.enemy.EnemyID = 1;
        sheep.currentHealth = sheep.enemy.Health;
        sheep.maxHealth = sheep.enemy.Health;
        sheep.drawPile = sheep.enemy.CardsDeck;
        sheep.handCard = new List<Card>();
        sheep.discardPile = new List<Card>();
        StartingHandCards(3, sheep.handCard, sheep.drawPile, false);
        EnemyDataList.Add(1, sheep);
    }

    static void LoadEnemyForLevel1()
    {
        EnemyData enemy1 = new EnemyData();
        enemy1.position = new Vector2(18, 12);
        enemy1.obj = GameObject.Find("LabWorker");
        enemy1.enemy = enemy1.obj.GetComponent<LabWorkerEnemy>();
        enemy1.enemy.EnemyID = 1;
        enemy1.currentHealth = enemy1.enemy.Health;
        enemy1.maxHealth = enemy1.enemy.Health;
        enemy1.drawPile = enemy1.enemy.CardsDeck;
        enemy1.handCard = new List<Card>();
        enemy1.discardPile = new List<Card>();
        StartingHandCards(2, enemy1.handCard, enemy1.drawPile, false);
        EnemyDataList.Add(1, enemy1);

        var enemy = new EnemyData();
        enemy.position = new Vector2(16, 12);
        enemy.obj = GameObject.Find("Hound");
        enemy.enemy = enemy.obj.GetComponent<Hound>();
        enemy.enemy.EnemyID = 2;
        enemy.currentHealth = enemy.enemy.Health;
        enemy.maxHealth = enemy.enemy.Health;
        enemy.drawPile = enemy.enemy.CardsDeck;
        enemy.handCard = new List<Card>();
        enemy.discardPile = new List<Card>();
        StartingHandCards(3, enemy.handCard, enemy.drawPile, false);
        EnemyDataList.Add(2, enemy);
    }
    static void LoadEnemyForLevel2()
    {
        EnemyData sheep = new EnemyData();
        sheep.position = new Vector2(7, 4);
        sheep.obj = GameObject.Find("AlphaSoldier");
        sheep.enemy = sheep.obj.GetComponent<AlfaSolder>();
        sheep.enemy.EnemyID = 1;
        sheep.currentHealth = sheep.enemy.Health;
        sheep.maxHealth = sheep.enemy.Health;
        sheep.drawPile = sheep.enemy.CardsDeck;
        sheep.handCard = new List<Card>();
        sheep.discardPile = new List<Card>();
        StartingHandCards(2, sheep.handCard, sheep.drawPile, false);
        EnemyDataList.Add(1, sheep);
    }
}
