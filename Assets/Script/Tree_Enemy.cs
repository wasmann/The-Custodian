using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Enemy : Enemy
{
    public override string EnemyName { get { return "Tree"; } }
    public override List<Card> Deck {
        get {
            List<Card> deck = new List<Card>();
            deck.Add(new Photovoltaics_Card());
            return deck;
        }
    }
    public override int Health { get { return 5; } }

    public override int EnemyID { 
        get { return 1; }
        set { EnemyID = value; }
    }

    public override void EnermyChooseACardToPlay(int ID)
    {
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
    }

}
