using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLevelDriver : MonoBehaviour
{
    public BattleData battleData;
    public UI uI;
    public bool Paused;
    public bool BattleOver;

    public List<List<Card.InfoForActivate>> TimeLineSlots;


    public void BeginABattleLevel(int ID)
    {
        battleData.LoadBattlelevel(ID);
        uI.UpdateHandCard();
        GameData.currentState = GameData.state.Battle;
        StartCoroutine(EnableTimeLineSlots());
        StartCoroutine(BattleLevelGame());
    }

    private IEnumerator BattleLevelGame()
    {
        yield return new WaitUntil(() => BattleOver == true);
        GameData.currentState = GameData.state.WorldMap;
        //TODO: load to Menu or show battle summary


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

            foreach (Card.InfoForActivate info in currentCards)
            {
                //TODO: pick out player's card and activate it first.
                info.card.Acitvate(info);
                if (info.player_ID != 0)
                {
                    battleData.NewCard.Add(info.card.ID);
                    battleData.EnermyDataList[info.player_ID].enermy.EnermyChooseACardToPlay(battleData);
                }
            }
            
        }
    }

    public void NewCardPlayed(Card.InfoForActivate info)
    {
        if (info.card.Speed == 0)//instant
        {
            info.card.Acitvate(info);
            return;
        }
        if (info.card.Speed <= 3)
            UI.UpdateTimeLine();
        TimeLineSlots[info.card.Speed].Add(info);
    }

    public IEnumerable Pause()
    {
        Paused = true;
        UI.ShowDuplicationWin();
        yield return new WaitUntil(() => Paused == false);
    }

}
