using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLevelDriver : MonoBehaviour
{
    public BattleData battleData;
    //public UI uI;
    public bool Paused;
    public bool BattleOver;

    public static List<List<Card.InfoForActivate>> TimeLineSlots;

    public void BeginABattleLevel(int ID)
    {
        BattleData.BattleLevelInit(ID);
        UI.LoadBattleBegin();
        GameData.currentState = GameData.state.Battle;
        StartCoroutine(EnableTimeLineSlots());
        StartCoroutine(BattleLevelGame());
    }

    private IEnumerator BattleLevelGame()
    {
        yield return new WaitUntil(() => BattleOver == true);
        GameData.currentState = GameData.state.WorldMap;
        //TODO: load gameover or show battle summary
    }

    private IEnumerator EnableTimeLineSlots()
    {
        TimeLineSlots=new List<List<Card.InfoForActivate>>();
        while (!Paused && !BattleOver)
        {
            while (Paused)
            {
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(1);

            //Remove the cards at time steps 0 and add a new list at  time step 10
            List<Card.InfoForActivate> currentCards = TimeLineSlots[0];
            TimeLineSlots.RemoveAt(0);
            TimeLineSlots.Add(new List<Card.InfoForActivate>());

            UI.MoveTimeLine();

            foreach (Card.InfoForActivate info in currentCards)
            {
                //TODO: pick out player's card and activate it first.
                info.card.Activate(info);
                if (info.owner_ID != 0)
                {
                    battleData.NewCard.Add(info.card.ID);
                    BattleData.EnemyDataList[info.owner_ID].enemy.EnermyChooseACardToPlay(info.owner_ID);

                }
            }
            
        }
    }

    public static void NewCardPlayed(Card.InfoForActivate info)
    {
        if (info.card.Speed == 0)//instant
        {
            info.card.Activate(info);
            return;
        }
        if (info.card.Speed <= 3)
            UI.UpdateTimeLine(TimeLineSlots);
        //player 
        TimeLineSlots[info.card.Speed].Add(info);
    }

    public IEnumerable Pause()
    {
        Paused = true;
        UI.ShowDuplicationWin();
        yield return new WaitUntil(() => Paused == false);
    }

}
