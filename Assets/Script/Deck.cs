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
        Shuffle();
        //BattleData.playerData.drawPile = BattleData.playerData.discardPile;
        //BattleData.playerData.discardPile = new List<Card>();
    }

    public static void Withdraw(Card card)
    {
        BattleData.playerData.drawPile.Add(card);
        DrawCard();
    }

    // Anything left?
    public static Card FindCardInHand(List<Card> handcard, int CardID)
    {
        for(int i = 0; i < handcard.Count; i++)
        {
            if (handcard[i].ID == CardID)
                return handcard[i];
        }
        return null;
    }

    public static void UpdateEnemyPiles(int ID,Card card)
    {

    }

    public static void Shuffle()
    {
        int count = BattleData.playerData.discardPile.Count;
        for(int i = count - 1; i >= 0; --i)
        {
            int k = Random.Range(0, i);
            BattleData.playerData.drawPile.Add(BattleData.playerData.discardPile[k]);
            BattleData.playerData.discardPile.RemoveAt(k);
        }
        Debug.Log(BattleData.playerData.discardPile.Count);
        Debug.Log(BattleData.playerData.drawPile.Count + "==" + count);
    }
}
