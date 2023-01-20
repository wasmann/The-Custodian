using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleLevelDriver : MonoBehaviour
{
    public bool Paused;
    public bool BattleOver;

    public static List<List<Card.InfoForActivate>> TimeLineSlots;
    [SerializeField]
    public int LevelID;

    public float tickTime;
    [SerializeField]
    public Slider tickSlider;

    private void Start()
    {
        BeginABattleLevel(LevelID);
    }
    public void BeginABattleLevel(int ID)
    {
        List<Card.InfoForActivate>[] TimeLinearray = new List<Card.InfoForActivate>[10];
        TimeLineSlots = TimeLinearray.ToList();
        for(int i = 0; i < TimeLineSlots.Count; i++)
        {
            TimeLineSlots[i] = new List<Card.InfoForActivate>();
        }

        BattleData.BattleLevelInit(ID);
        UI.LoadBattleBegin();
        GameData.currentState = GameData.state.Battle;
        tickTime = GameData.tickspeed;
        StartCoroutine(EnableTimeLineSlots());
        StartCoroutine(BattleLevelGame());
        
    }

    private IEnumerator BattleLevelGame()
    {        
        yield return new WaitUntil(() => BattleOver == true);
        
        GameData.currentState = GameData.state.WorldMap;
        //TODO: load gameover or show battle summary
        /*if (BattleData.CheckWinCondition())
        {
            SceneManager.LoadScene("WinningScreen");
        }

        if (BattleData.CheckLoseCondition())
        {
            SceneManager.LoadScene("FailedScreen");
        }*/
    }

    private IEnumerator EnableTimeLineSlots()
    {
        for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
        {
            BattleData.EnemyData enemy = BattleData.EnemyDataList[i];
            enemy.obj.GetComponent<Enemy>().EnemyChooseACardToPlay();
        }
        UI.UpdateTimeLine(TimeLineSlots);
        while (!BattleOver)
        {
            while (Paused)
            {
                yield return new WaitForSeconds(0.2f);
                //yield return new WaitUntil(() => Paused == false);
                
            }
            yield return new WaitForSeconds(tickTime);
            //Remove the cards at time steps 0 and add a new list at  time step 10
            List<Card.InfoForActivate> currentCards = TimeLineSlots[0];
            TimeLineSlots.RemoveAt(0);
            TimeLineSlots.Add(new List<Card.InfoForActivate>());

            UI.MoveTimeLine();

            foreach (Card.InfoForActivate info in currentCards)
            {

                if (info.owner_ID == 99)
                {
                    Debug.Log("finish duplicate" + info.card.Name);
                    System.Type myType = Type.GetType(info.card.Name);

                    Card newcard=(Card)GameObject.Instantiate(GameObject.Find("CardBank/"+info.card.Name)).GetComponent(myType);


                    GameData.Deck.Add(newcard);
                    GameData.duplicated += 1;
                    BattleData.playerData.drawPile.Add(newcard);
                    BattleData.duplicated.Add(newcard);
                    continue;
                }

                if (info.owner_ID == 0)
                {
                    BattleData.AbleToPalyCard = true;
                }
                
                info.card.Activate(info);

                if (info.owner_ID != 0 && info.owner_ID != 99)
                {
                    if(info.card.Name != "Discard" && !BattleData.duplicated.Contains(info.card))
                    {
                        BattleData.NewCard.Add(info.card);//for duplication
                    }
                    
                    BattleData.EnemyDataList[info.owner_ID].enemy.EnemyChooseACardToPlay();
                    UI.UpdateTimeLine(TimeLineSlots);
                }
            }
            
        }
    }

    public static void NewCardPlayed(Card.InfoForActivate info)
    {
        //Debug.Log(info.card.name);

        TimeLineSlots[info.card.Speed].Add(info);
        if (info.card.Speed == 0)//instant
        {
            info.card.Activate(info);
            return;
        }
        if (info.card.Speed <= 3)
            UI.UpdateTimeLine(TimeLineSlots);
        
    }

   /* public void PauseButton()
    {
        if (Paused)
        {       
            UI.Pause();
            Paused = false;
        }
        else
        {
            UI.Pause();
            Pause();
            Paused = true;
        }
    }
    public IEnumerable Pause()
    {
        UI.ShowDuplicationWin();
        yield return new WaitUntil(() => UI.isPaused == false);
    }*/

    public void SetTicktime()
    {
        tickTime = tickSlider.value;
        GameData.tickspeed = tickSlider.value;
    }

    public void Pause()
    {
        
        if (Paused)
        {
            
            Paused = false;
            //Debug.Log(Paused);
        }
        else
        {
            
            Paused = true;
            //Debug.Log(Paused);
        }
        
    }
}
