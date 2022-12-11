using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    
    
    //Get playerData from BattleData.cs

    //private HashSet<Card> deck; // drawPile

    //private HashSet<Card> deckPlayed; // discardPile

    public static void DrawCard()
    {
        if(BattleData.playerData.drawPile.Count == 0)
        {
            RebuildDrawPile();
        }

        Card card = BattleData.playerData.drawPile[0];
        BattleData.playerData.drawPile.Remove(card);
        BattleData.playerData.handCard.Add(card);
        UI.UpdateHandCard();
    }

    public static void RebuildDrawPile()
    {
        BattleData.playerData.drawPile = BattleData.playerData.discardPile;
        BattleData.playerData.discardPile = new List<Card>();
    }

    public static void Withdraw(Card card)
    {
        BattleData.playerData.drawPile.Add(card);
        DrawCard();
    }

    // Anything left?



}
