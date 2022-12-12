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
    //static string PREFAB_PATH = "Prefab/Card/";

    public static List<Vector3> HandCardPos;
    public static Vector3[] TimeLinePos_Enemy=new Vector3[10];
    public static Vector3[] TimeLinePos_Player = new Vector3[10];

    static List<GameObject> timelineObj;

    static Vector3 mouseWorldPos;
    
    public static void LoadBattleBegin()
    {
        timeline = GameObject.Find("TimeLine");
        custodian = GameObject.Find("Custodian");
        var sliders = Object.FindObjectsOfType<Slider>();
        health = sliders[0];
        energy = sliders[1];

        health.maxValue = BattleData.playerData.maxHealth;
        energy.maxValue = BattleData.playerData.maxEnergy;

        timelineObj = new List<GameObject>();

        UpdatePlayerData();

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
    //public static void MoveTimeLine()
    //{
    //    //move one timeslot
    //    for (int i = 0; i < 2; ++i)
    //    {
    //        for (int j = 0; j < TIME_LINE_LENGTH - 1; ++j)
    //        {

    //            if (j == TIME_LINE_LENGTH)
    //            {
    //                Destroy(timelineObj[i, j].gameObject);

    //            }
    //            Destroy(timelineObj[i, j].gameObject);
    //            timelineObj[i, j] = timelineObj[i, j + 1];
    //            timelineObj[i, j] = Instantiate(timelineObj[i, j], timeline.transform.position + pos[i, j], Quaternion.identity);

    //            if (j == 3)
    //                timelineObj[i, j].SetActive(true);
    //        }
    //    }

    //public static void MoveTimeLine()
    //{
    //    for (int j = 0; j < TIME_LINE_LENGTH - 1; ++j)
    //    {
    //        if (j == TIME_LINE_LENGTH && j == 0)
    //        {
    //            Destroy(timelineObj[0, j].gameObject);
    //            continue;
    //        }
    //        if (timelineObj[0, j + 1] != null)
    //            timelineObj[0, j] = timelineObj[0, j + 1];
    //        timelineObj[0, j].transform.position = TimeLinePos_Enemy[j];
    //        if (j == 2)
    //            timelineObj[0, j].SetActive(true);
    //    }

    //    for (int j = 0; j < TIME_LINE_LENGTH - 1; ++j)
    //    {
    //        if (j == TIME_LINE_LENGTH && j == 0)
    //        {
    //            Destroy(timelineObj[0, j].gameObject);
    //            continue;
    //        }
    //        timelineObj[1, j] = timelineObj[1, j + 1];
    //        timelineObj[1, j].transform.position = TimeLinePos_Player[j];
    //    }
    //}



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

    //public static void UpdateTimeLine(List<List<Card.InfoForActivate>> timeLineSlots)
    //{// problem here , timeline OBJ is 2*10*N
    //    Debug.Log("add card on timeline");
    //    for (int i = 0; i < timeLineSlots.Count; ++i)
    //    {
    //        if (timeLineSlots[i] != null)
    //        {
    //            for (int j = 0; j < timeLineSlots[i].Count; ++j)
    //            {
    //                if (timeLineSlots[i][j].owner_ID == 0)
    //                {
    //                    timelineObj[1, i] = GameObject.Instantiate(Resources.Load("Prefab/CardOnTimeLine/" + timeLineSlots[i][j].card.Name) as GameObject, TimeLinePos_Player[i], Quaternion.identity);

    //                }
    //                else if (i <= 3)
    //                { 
    //                    //timelineObj[0, i] = Instantiate(Resources.Load("Prefab/CardOnTimeLine/" + timeLineSlots[i][j].card.Name) as GameObject, TimeLinePos_Enemy[i], Quaternion.identity);
    //                    timelineObj[0, i] = GameObject.Instantiate(GameObject.Find("CardOnTimeLine/" + timeLineSlots[i][j].card.Name));
    //                    timelineObj[0, i].transform.position = TimeLinePos_Enemy[i];
    //                }
    //                else
    //                {
    //                    timelineObj[0, i] = GameObject.Instantiate(Resources.Load("Prefab/CardOnTimeLine/" + timeLineSlots[i][j].card.Name) as GameObject, TimeLinePos_Enemy[i], Quaternion.identity);
    //                    timelineObj[0, i].SetActive(false);
    //                }
    //            }
    //        }
    //    }
    //}


    public static void UpdateTimeLine(List<List<Card.InfoForActivate>> timeLineSlots)
    {
        for (int i = 0; i < timeLineSlots.Count; i++) {
            for (int j = 0; j < timeLineSlots[i].Count; j++) {
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



    public static void ShowDuplicationWin()
    {

    }

    public static void UpdatePlayerData()
    {

        health.value = BattleData.playerData.currentHealth;
        energy.value = BattleData.playerData.currentEnergy;
        custodianPos = BattleData.playerData.position;
        //TODO : change the player sprite to correct pos

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
        DestroyHandcard();
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

    public static void DestroyHandcard()
    {
        //just move the gameobjects away to somewhere;
        for (int i = 0; i < BattleData.playerData.handCard.Count; i++)
        {
            BattleData.playerData.handCard[i].gameObject.SetActive(false);
        }
    }
}