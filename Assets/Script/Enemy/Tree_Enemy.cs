using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Enemy : Enemy
{
    public override string EnemyName { get { return "Tree"; } }

    public List<Card> deck;
    public override List<Card> CardsDeck
    {
        get {
            return deck;
        }
    }
    public override int Health { get { return 5; } }

    public override int HandCardNum { get { return 1; } }

    public override void EnemyChooseACardToPlay()
    {
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.card = BattleData.EnemyDataList[EnemyID].handCard[0];
        BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);
    }

}
