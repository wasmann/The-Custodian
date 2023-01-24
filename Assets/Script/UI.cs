using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

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
    public static GameObject NotationList;

    //pause
    static GameObject pauseButton;
    public static bool isPaused = false;

    //duplication
    static GameObject duplicationPanel;

    public static bool waitForDuplicate;
    public static GameObject duplicationGrid;

    static Vector3 mouseWorldPos;


    //move function
    static Vector3 newPosition;
    static Vector3 lastPosition;
    static float timeElapsed = 0;

    static Animator custoAnim;
    static SpriteRenderer custoRender;
    static GameObject damage;
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

        pauseButton = GameObject.Find("Button");
        duplicationPanel = GameObject.Find("DuplicationPanel");
        duplicationPanel.SetActive(false);
        duplicationGrid = GameObject.Find("DuplicationGrid");

        custoAnim = custodian.GetComponent<Animator>();
        custoRender = custodian.GetComponent<SpriteRenderer>();

        UpdatePlayerData();
        custodian.transform.position = ToolFunction.FromCoorinateToWorld(BattleData.playerData.position);

        UpdateAllEnemyData();

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
                if((timeLineSlots[i][j].owner_ID == 0)  || (timeLineSlots[i][j].owner_ID == 99))
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
        if (health.value > BattleData.playerData.currentHealth)
        {
            ShowDamage(health.value - BattleData.playerData.currentHealth);
        }
        health.value = BattleData.playerData.currentHealth;
        energy.value = BattleData.playerData.currentEnergy;

        health.GetComponentInChildren<TMP_Text>().text = health.value + " / " + health.maxValue;
        energy.GetComponentInChildren<TMP_Text>().text = energy.value + " / " + energy.maxValue;

        //custodian.transform.position = ToolFunction.FromCoorinateToWorld(BattleData.playerData.position);
        lastPosition = custodian.transform.position;
        newPosition = ToolFunction.FromCoorinateToWorld(BattleData.playerData.position);
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
        if (BattleData.EnemyDataList[ID].obj.GetComponentInChildren<Slider>().value != BattleData.EnemyDataList[ID].currentHealth)
        {
            GameObject damage = Instantiate(Resources.Load("Prefab/UI/Damage") as GameObject, BattleData.EnemyDataList[ID].obj.transform);
            damage.GetComponent<TMP_Text>().text = "-" + (BattleData.EnemyDataList[ID].obj.GetComponentInChildren<Slider>().value - BattleData.EnemyDataList[ID].currentHealth);
            Destroy(damage, 0.5f);
            BattleData.EnemyDataList[ID].obj.GetComponentInChildren<Slider>().value = BattleData.EnemyDataList[ID].currentHealth;

            if(BattleData.EnemyDataList[ID].obj.GetComponentInChildren<Slider>().value <= 0)
            {
                BattleData.EnemyDataList.Remove(ID);
                GameData.killed += 1;
            }
        }
    }
    public static void UpdateAllEnemyData()
    {
        for (int i = 1; i < BattleData.EnemyDataList.Count+1; i++)
        {
            BattleData.EnemyDataList[i].obj.GetComponentInChildren<Slider>().maxValue = BattleData.EnemyDataList[i].maxHealth;
            BattleData.EnemyDataList[i].obj.GetComponentInChildren<Slider>().value = BattleData.EnemyDataList[i].currentHealth;
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
            //BattleData.playerData.handCard[i] = Instantiate(Resources.Load("Prefab/Card/" + BattleData.playerData.handCard[i].Name) as GameObject);
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
        duplicationGrid.SetActive(true);

        foreach (Card card in BattleData.NewCard)
        {
            
           GameObject obj = Instantiate(Resources.Load("Prefab/Card/" + card.Name) as GameObject, duplicationGrid.transform);
           //Debug.Log(card.Name);

        }
        
    }


    public static void DestroyDuplicationWin()
    {
        for (int i = 0; i < duplicationGrid.transform.childCount; ++i)
        {
            Destroy(duplicationGrid.transform.GetChild(i).gameObject);
        }
    }
    public static void Pause()
    {
        if (isPaused)
        {
            //Child 1 is continue, 0 is pause
            pauseButton.transform.GetChild(0).gameObject.SetActive(false);
            pauseButton.transform.GetChild(1).gameObject.SetActive(true);
            
            isPaused = false;
            
            DestroyDuplicationWin();
            duplicationPanel.SetActive(false);
            duplicationGrid.SetActive(false);
          
            waitForDuplicate = false;

            GameObject.Find("GameManager").GetComponent<BattleLevelDriver>().Pause();
        }
        else
        {
            pauseButton.transform.GetChild(0).gameObject.SetActive(true);
            pauseButton.transform.GetChild(1).gameObject.SetActive(false);
            
            isPaused = true;
            ShowDuplicationWin();
           
            waitForDuplicate = true;
            GameObject.Find("GameManager").GetComponent<BattleLevelDriver>().Pause();
        }
    }

    public static void FinishDuplicate(Card card, Card.Rarity rarity)
    {
        //Debug.Log("card name " + card.Name);
        
        int time = 0;
        switch (rarity)
        {
            case Card.Rarity.basic:
                time = 1;
                break;

            case Card.Rarity.common:
                time = 2;
                break;

            case Card.Rarity.rare:
                time = 4;
                break;

            case Card.Rarity.epic:
                time = 7;
                break;

            case Card.Rarity.legendary:
                time = 9;
                break;
        }
        
        //Debug.Log("card info " + card.Info);
        card.Info.owner_ID = 99;
        card.Info.card = card;
        BattleLevelDriver.TimeLineSlots[time].Add(card.Info);

        //GameData.Deck.Add(card);
        //BattleData.playerData.drawPile.Add(card);

        Pause();
        //BattleData.NewCard.Remove(card);
    }
    public static void MoveCusto()
    {
        //according to timeslots tick needed
        if (timeElapsed < 1)
        {
            custodian.transform.position = Vector3.Lerp(lastPosition, newPosition, timeElapsed / 1);
            timeElapsed += Time.deltaTime;
            if ((lastPosition.x - newPosition.x) > 0)
            {
                custoRender.flipX = true;
               
            }
            else if ((lastPosition.x - newPosition.x) < 0)
            {
                custoRender.flipX = false;
            
            }
           
        }
        else
        {
            custodian.transform.position = newPosition;
            timeElapsed = 0;
           
        }

    }

    public static void ShowDamage(float _damage)
    {
        GameObject damage = Instantiate(Resources.Load("Prefab/UI/Damage") as GameObject, custodian.transform);//custodian.transform.position, Quaternion.identity
        
        damage.GetComponent<TMP_Text>().text = "-" + _damage;
        Destroy(damage, 0.5f);
    }

    private void Update()
    {
        if (newPosition != custodian.transform.position)
        {
            MoveCusto();
        }

    }
}