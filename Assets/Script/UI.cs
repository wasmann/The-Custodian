using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //custodian
    private static GameObject custodian;
    static Slider health;
    static Slider energy;
    static Vector3 custodianPos;
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
    static string PREFAB_PATH = "Prefab/Card/";

    static Vector3[,] pos;
    static Vector3[] handcardPos;
    static GameObject[,] timelineObj;

    static Vector3 mouseWorldPos;
    
    public static void LoadBattleBegin()
    {

        custodian = GameObject.Find("Custodian");
        var sliders = Object.FindObjectsOfType<Slider>();
        health = sliders[0];
        energy = sliders[1];

        health.maxValue = BattleData.playerData.maxHealth;
        energy.maxValue = BattleData.playerData.maxEnergy;

        UpdatePlayerData();

        //UpdateEnemyData();

        InitPos();
        InitHandcardPos();

        timeline = GameObject.Find("TimeLine");

        UpdateHandCard();


    }

    public static void UpdateHandCard()// After drawing a new card, reorgnize the hand card(align right) and move a card from deck to hand at the most left side.
    {
        DestroyHandcard();

        for (int i = 0; i < BattleData.playerData.handCard.Count; i++)
        {
            GameObject card = GameObject.Instantiate(Resources.Load(PREFAB_PATH + BattleData.playerData.handCard[i].Name) as GameObject,
              handcardPos[i], Quaternion.identity);
        }
    }

    public static void MoveTimeLine()
    {
        //move one timeslot
        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j < TIME_LINE_LENGTH - 1; ++j)
            {

                if (j == TIME_LINE_LENGTH)
                {
                    Destroy(timelineObj[i, j].gameObject);

                }
                Destroy(timelineObj[i, j].gameObject);
                timelineObj[i, j] = timelineObj[i, j + 1];
                timelineObj[i, j] = Instantiate(timelineObj[i, j], timeline.transform.position + pos[i, j], Quaternion.identity);

                if (j == 3)
                    timelineObj[i, j].SetActive(true);
            }
        }

    }


    /*public static IEnumerator ShowNotation(List<GameObject> notion, Card.InfoForActivate info)
    {
        // notion is used to show how is the range or attack damage of a card. For example move left can be arrow pointing left covering one grid.
        //this function will show the notation in the direction coresponding to the mouse and character position. For example if the mouse is at the top side of character, then the notion will placed at the top side of character.
        
        if()
        GameObject.Instantiate(notion, new Vector2(characterpos.x, characterpos.y), Quaternion.identity);
        GameObject.Instantiate(notion, new Vector2(mouseWorldPos.x, mouseWorldPos.y), Quaternion.identity);

        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        while (!Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject.Instantiate(notion, new Vector2(mouseWorldPos.x, mouseWorldPos.y), Quaternion.identity);

            //problem!!
       
        }
        UpdateTimeLine();
        yield return null;
    }*/

    public static void UpdateTimeLine(List<List<Card.InfoForActivate>> timeLineSlots)
    {
        for (int i = 0; i < timeLineSlots.Count; ++i)
        {
            for (int j = 0; j < timeLineSlots[i].Count; ++j)
            {
                if (timeLineSlots[i][j].owner_ID == 0)
                {
                    timelineObj[1, i] = GameObject.Instantiate(Resources.Load(timeLineSlots[i][j].card.Name) as GameObject,
            timeline.transform.position + pos[1, i], Quaternion.identity);

                }
                else if (i <= 3)
                {
                    timelineObj[0, i] = GameObject.Instantiate(Resources.Load(timeLineSlots[i][j].card.Name) as GameObject,
            timeline.transform.position + pos[0, i], Quaternion.identity);
                }
                else
                {
                    timelineObj[0, i] = GameObject.Instantiate(Resources.Load(timeLineSlots[i][j].card.Name) as GameObject,
            timeline.transform.position + pos[0, i], Quaternion.identity);
                    timelineObj[0, i].SetActive(false);
                }

            }
        }
    }

    public static void ShowDuplicationWin()
    {

    }

    public static void UpdatePlayerData()
    {
        health.value = BattleData.playerData.currentHealth;
        energy.value = BattleData.playerData.currentEnergy;
        custodianPos = BattleData.playerData.position;

    }

    public void loadEnemyGameObject(List<string> enemyNames)
    {
        for (int i = 0; i < enemyNames.Count; i++)
        {
            GameObject obj = GameObject.Find(enemyNames[i]);
            Enemies.Add(i, obj);
        }
    }

    public static void UpdateEnemyData(int ID)
    {

    }

    private static void InitPos()
    {

        
        timelineObj = new GameObject[2, TIME_LINE_LENGTH];

        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j < TIME_LINE_LENGTH; ++j)
            {
                timelineObj[i, j] = new GameObject();
            }
        }

        pos = new Vector3[2, TIME_LINE_LENGTH];
        //one line under timeline for enemy, one line above for player, one line for duplication

        float offsite = CARD_WIDTH * TIME_LINE_LENGTH / 2;
        float heightOffsite = CARD_HEIGHT;

        for (int i = 0; i < 2; ++i)
        {
            if (i != 0)
            {
                for (int j = 0; j < TIME_LINE_LENGTH; ++j)
                {
                    pos[i, j] = new Vector3(j * CARD_WIDTH - offsite, i * CARD_HEIGHT - heightOffsite + TIME_LINE_HEIGHT, 0);
                }
            }
            else
            {
                for (int j = 0; j < TIME_LINE_LENGTH; ++j)
                {
                    pos[i, j] = new Vector3(j * CARD_WIDTH - offsite, i * CARD_HEIGHT - heightOffsite, 0);
                }
            }

        }
    }

    private static void InitHandcardPos()
    {
        handcardPos = new Vector3[4];
        handcardPos[0] = new Vector3(1, 0.7f, 100);
        handcardPos[1] = new Vector3(1 - 2 * CARD_WIDTH, 0.7f, 100);
        handcardPos[2] = new Vector3(1 - 4 * CARD_WIDTH, 0.7f, 100);
        handcardPos[3] = new Vector3(1 - 6 * CARD_WIDTH, 0.7f, 100);
    }

    public static void DestroyHandcard()
    {
        for (int i = 0; i < BattleData.playerData.handCard.Count; i++)
        {
            Destroy(BattleData.playerData.handCard[i].gameObject);
        }
        //Destroy(card1.gameObject);
        //Destroy(card2.gameObject);
        //Destroy(card3.gameObject);
        //Destroy(card4.gameObject);
    }
}