using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLevelDriver : MonoBehaviour
{
    public BattleData battleData;
    public GameData gameData;
    public UI uI;
    public bool Paused;
    public bool BattleOver;

    public List<List<Card.InfoForActivate>> TimeLineSlots;


    public void BeginABattleLevel(int ID)
    {
        battleData.LoadBattlelevel(ID);
        uI.UpdateHandCard();
        StartCoroutine(EnableTimeLineSlots());
        gameData.currentState = GameData.state.Battle;
        while (!Paused && !BattleOver)
        {
        
        }


    }

    private IEnumerator EnableTimeLineSlots()
    {
        TimeLineSlots=new List<List<Card.InfoForActivate>>();
        while (!Paused && !BattleOver)
        {
            yield return new WaitForSeconds(1);

            //Remove the cards at time steps 0 and add a new list at  time step 10
            List<Card.InfoForActivate> currentCards = TimeLineSlots[0];
            TimeLineSlots.RemoveAt(0);
            TimeLineSlots.Add(new List<Card.InfoForActivate>());

            foreach (Card.InfoForActivate info in currentCards)
            {
                //TODO: pick out player's card and activate it first.
                info.card.Acitvate(info);
            }
            
        }
    }

    public void NewCardPlayed(Card.InfoForActivate info)
    {
        if (info.card.speed == 0)//instant
        {
            info.card.Acitvate(info);
            return;
        }
        if (info.card.speed <= 3)
            UI.NewCardsCanBeSeen(info);

        TimeLineSlots[info.card.speed].Add(info);

    }
}
