using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    
    
    //Get playerData from BattleData.cs

    //private HashSet<Card> deck; // drawPile

    //private HashSet<Card> deckPlayed; // discardPile

    public void DrawCard()
    {
        //TODO drawcard
        if(BattleData.playerData.drawPile.Count == 0)
        {
            RebuildDrawPile();
        }

        Card card = BattleData.playerData.drawPile.First();
        BattleData.playerData.drawPile.Remove(card);
        BattleData.playerData.discardPile.Add(card);
        UI.UpdateHandCard();
        
    }

    public (HashSet<Card>,List<Card>) StartingHandCards(int num, BattleData battleData)
    {
        // initialize battle deck
        foreach(Card a in GameData.Deck) 
        {
            BattleData.playerData.drawPile.Add(a);
        }

        List<Card> startingCards = new List<Card>();
        for (int i = 0; i < 4; ++i)
        {
            DrawCard();
        }
        return (BattleData.playerData.drawPile, startingCards); 
    }

    /*
     * public void Shuffle() {
        
    }*/

    public void RebuildDrawPile()
    {
        BattleData.playerData.drawPile = BattleData.playerData.discardPile;
        BattleData.playerData.discardPile = new HashSet<Card>();
    }

    public void Withdraw(Card card)
    {
        BattleData.playerData.drawPile.Add(card);
        DrawCard();
    }

    // Anything left?



}
