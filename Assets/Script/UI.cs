using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class UI : MonoBehaviour
{
    //custodian
    private static GameObject custodian;
    static Slider health;
    static Slider energy;
    //static int RAM;

    //enemy
    private static Dictionary<int, GameObject> Enemies = new Dictionary<int, GameObject>();

    //ui components
    static GameObject timeline;
    const int TIME_LINE_LENGTH = 10;
    const float CARD_HEIGHT = 1;
    const float CARD_WIDTH = 1;
    const float TIME_LINE_HEIGHT = 0.5f;
    const float TIME_LINE_SPEED = 1.0f;

    //static string PREFAB_PATH = "Prefab/Card/";

    //card
    public static List<Vector3> HandCardPos;
    public static Vector3[] TimeLinePos_Enemy = new Vector3[10];
    public static Vector3[] TimeLinePos_Player = new Vector3[10];

    static List<GameObject> timelineObj;
    static GameObject NotationList;

    //pause
    static GameObject pauseButton;
    public static bool isPaused = false;

    //duplication
    static GameObject duplicationPanel;

    static Vector3 mouseWorldPos;
    
    public static void LoadBattleBegin()
    {
        timeline = GameObject.Find("TimeLine");
        custodian = GameObject.Find("Custodian");
        NotationList= GameObject.Find("Grid/NotationList");
        health = GameObject.Find("Armor").GetComponent<Slider>();
        energy = GameObject.Find("Energy").GetComponent<Slider>();

        health.maxValue = BattleData.playerData.maxHealth;
        energy.maxValue = BattleData.playerData.maxEnergy;

        timelineObj = new List<GameObject>();

        pauseButton = GameObject.Find("PauseButton");
        duplicationPanel = GameObject.Find("DuplicationPanel");
        duplicationPanel.SetActive(false);

        UpdatePlayerData();
        UpdateAllEnemyData();

        //UpdateEnemyData();

        InitTimeLinePos();
        InitHandcardPos();

        UpdateHandCard();


    }
    public static void MoveTimeLine()
    {
        for(int i=0;i< timelineObj.Count; i++)
        {
            if (timelineObj[i].transform.localPosition.x == TimeLinePos_Enemy[0].x && timelineObj[i].transform.localPosition.x == TimeLinePos_Player[0].x)
            {
                Destroy(timelineObj[i]);
                timelineObj.RemoveAt(i);
                continue;
            }
            timelineObj[i].transform.localPosition = timelineObj[i].transform.localPosition + new Vector3(-150f, 0,0);
            if (timelineObj[i].transform.localPosition.x == TimeLinePos_Enemy[3].x)
            {
                timelineObj[i].SetActive(true);
            }
        }
    }



    public static void ShowNotation(Card card)
    {
        if (card.RangeNotation!="None")
        {
            for(int i = 0; i < card.Info.direction.Count; i++)
            {
                GameObject notation= GameObject.Instantiate(Resources.Load("Prefab/Notation/" + card.RangeNotation) as GameObject);
                notation.transform.position = ToolFunction.FromCoorinateToWorld(card.Info.direction[i]);
                notation.transform.SetParent(NotationList.transform);
            }
        }
        if (card.SelectionNotation != "None")
        {
            //ToDo 
        }
    }
    public static void DeleteNotation()
    {
        foreach (Transform child in NotationList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public static void UpdateTimeLine(List<List<Card.InfoForActivate>> timeLineSlots)
    {
        for (int i = 0; i < timelineObj.Count; i++)
        {
            Destroy(timelineObj[i]);
        }
        timelineObj.Clear();
        for (int i = 0; i < timeLineSlots.Count; i++) {
            for (int j = 0; j < timeLineSlots[i].Count; j++) {
                if (timeLineSlots[i][j].card.Name == "Discard")
                {
                    continue;
                }
                GameObject obj= Instantiate(Resources.Load("Prefab/CardOnTimeLine/" + timeLineSlots[i][j].card.Name) as GameObject);
                obj.transform.SetParent(GameObject.Find("Canvas/TimeLine").transform);
                obj.transform.localScale = new Vector3(20,20,0);
                if(timeLineSlots[i][j].owner_ID == 0)
                {
                    obj.transform.localPosition = TimeLinePos_Player[i];
                }
                else if (timeLineSlots[i][j].card.Speed <= 3)
                {
                    obj.transform.localPosition = TimeLinePos_Enemy[i];
                }
                else
                {
                    obj.transform.localPosition = TimeLinePos_Enemy[i];
                    obj.SetActive(false);
                }
                timelineObj.Add(obj);
               
            }
        }
    }

    public static void UpdatePlayerData()
    {
        health.value = BattleData.playerData.currentHealth;
        energy.value = BattleData.playerData.currentEnergy;
        custodian.transform.position = ToolFunction.FromCoorinateToWorld(BattleData.playerData.position);
    }

    public static void loadEnemyGameObject(List<string> enemyNames)
    {
        for (int i = 0; i < enemyNames.Count; i++)
        {
            GameObject obj = GameObject.Find(enemyNames[i]);
            Enemies.Add(i, obj);
        }
    }

    public static void UpdateEnemyData(int ID)
    {
        BattleData.EnemyDataList[ID].obj.transform.position= ToolFunction.FromCoorinateToWorld(BattleData.EnemyDataList[ID].position);
        
    }
    public static void UpdateAllEnemyData()
    {
        for (int i = 1; i < BattleData.EnemyDataList.Count+1; i++)
        {
            UpdateEnemyData(i);
        }
    }
    private static void InitTimeLinePos()
    {
        for (int i = 0; i < 10; i++)
        {
            TimeLinePos_Enemy[i] = new Vector3(150*i,-110,0);
        }
        for (int i = 0; i < 10; i++)
        {
            TimeLinePos_Player[i] = new Vector3(150 * i, -230, 0);
        }
    }
    public static void UpdateHandCard()
    {
        SetOtherPilesInative();
        for (int i = 0; i < BattleData.playerData.handCard.Count; i++)
        {
            BattleData.playerData.handCard[i].gameObject.SetActive(true);
            BattleData.playerData.handCard[i].transform.SetParent(GameObject.Find("Canvas/HandCard").transform);
            BattleData.playerData.handCard[i].transform.localPosition = HandCardPos[i];
            BattleData.playerData.handCard[i].transform.localScale = new Vector3(20,20,1);
        }
    }
    private static void InitHandcardPos()
    {
        int handcardNum = BattleData.playerData.handCard.Count;
        HandCardPos = new List<Vector3>();
        for (int i = 0; i < handcardNum; i++)
        {
            HandCardPos.Add(new Vector3(400/(handcardNum-1)*i-200, 0f,0));
        }
    }

    public static void SetOtherPilesInative()
    {
        //just move the gameobjects away to somewhere;
        for (int i = 0; i < BattleData.playerData.discardPile.Count; i++)
        {
            BattleData.playerData.discardPile[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < BattleData.playerData.drawPile.Count; i++)
        {
            BattleData.playerData.drawPile[i].gameObject.SetActive(false);
        }
    }

    public static void ShowDuplicationWin()
    {
        duplicationPanel.SetActive(true);
        Vector3 initpos = new Vector3(-0.4f, 0.06f, 0);
        int i = 0, j = 0;
        foreach (Card card in BattleData.NewCard)
        {

            GameObject obj = Instantiate(Resources.Load("Prefab/CardOnTimeLine/" + card.Name) as GameObject, duplicationPanel.transform);
            //obj.transform.SetParent(GameObject.Find("Canvas/DuplicationPanel").transform);
            obj.transform.localScale = new Vector3(0.03f, 0.03f, 0);
            if (i >= 7)
            {
                i = 0;
                j++;
            }
            obj.transform.localPosition = new Vector3(initpos.x + 0.1f * i, initpos.y + 0.05f * j, 0);
            i++;
        }
    }

    public static void DestroyDuplicationWin()
    {
        for (int i = 0; i < duplicationPanel.transform.childCount; ++i)
        {
            Destroy(duplicationPanel.transform.GetChild(i).gameObject);
        }
    }
    public static void Pause()
    {
        if (isPaused)
        {
            //Child 1 is continue, 0 is pause
            pauseButton.transform.GetChild(0).gameObject.SetActive(false);
            pauseButton.transform.GetChild(1).gameObject.SetActive(true);
            //duplicationPanel.SetActive(false);
            isPaused = false;
            DestroyDuplicationWin();
            duplicationPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
            pauseButton.transform.GetChild(0).gameObject.SetActive(true);
            pauseButton.transform.GetChild(1).gameObject.SetActive(false);
            //duplicationPanel.SetActive(true);
            isPaused = true;
            ShowDuplicationWin();
        }
    }
}