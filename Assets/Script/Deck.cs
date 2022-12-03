using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    
    
    //Get playerData from BattleData.cs

    //private HashSet<Card> deck; // drawPile

    //private HashSet<Card> deckPlayed; // discardPile

    public void DrawCard(ref BattleData battleData)
    {
        //TODO drawcard
        if(battleData.playerData.drawPile.Count == 0)
        {
            RebuildDrawPile(ref battleData);
        }

        Card card = battleData.playerData.drawPile.First();
        battleData.playerData.drawPile.Remove(card);
        battleData.playerData.discardPile.Add(card);
        UI.UpdateHandCard(battleData);
        
    }

    public (HashSet<Card>,List<Card>) StartingHandCards(int num, BattleData battleData)
    {
        // initialize battle deck
        foreach(Card a in GameData.Deck) 
        {
            battleData.playerData.drawPile.Add(a);
        }

        List<Card> startingCards = new List<Card>();
        for (int i = 0; i < 4; ++i)
        {
            DrawCard(ref battleData);
        }
        return (battleData.playerData.drawPile, startingCards); 
    }

    /*
     * public void Shuffle() {
        
    }*/

    public void RebuildDrawPile(ref BattleData battleData)
    {
        battleData.playerData.drawPile = battleData.playerData.discardPile;
        battleData.playerData.discardPile = new HashSet<Card>();
    }

    public void Withdraw(Card card, ref BattleData battleData)
    {
        battleData.playerData.drawPile.Add(card);
        DrawCard(ref battleData);
    }

    // Anything left?



}
