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

    public Queue<List<Card.InfoForActivate>> TimeLineSlots;


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
        while (!Paused && !BattleOver)
        {
            //Wait for 1 sec
            //TimeLineSlots. pop
            foreach (Card.InfoForActivate card in TimeLineSlots[0])
            {
                card.card.activate(card);
            }
            
        }
    }

    public void NewCardPlayed(Card.InfoForActivate info)
    {
        //update TimeLineSlots
        //if in prediction: NewCardsCanBeSeen()

    }
}
