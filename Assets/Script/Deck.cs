using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public BattleData battleData;
    public UI ui;
    //Get playerData from BattleData.cs
    public void DrawCard()
    {
        //TODO drawcard
        ui.UpdateHandCard();
    }

    public static(List<Card>,List<Card>) StartingHandCards(int num, BattleData battleData)
    {
        return (battleData.playerData.drawPile, battleData.playerData.handCard);
    }
    public void Shuffle() { 

    }
    public void RebuildDrawPile()
    {

    }
    // Anything left?



}
