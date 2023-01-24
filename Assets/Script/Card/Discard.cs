using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discard : Card
{
    public override string Name { get { return "Discard"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 1; } }
    public override int ID { get { return 0; } }
    public override IEnumerator Play()
    {
        Info.owner_ID = 0;
        //Info.otherInfo Add when player right click the card rdy to discarded // ID
        yield return new WaitForSeconds(0.1f);
        BattleLevelDriver.NewCardPlayed(this.Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        
        if (Info.owner_ID == 0)
        {
            Card cardtodiscard = Deck.FindCardInHand(BattleData.playerData.handCard, int.Parse(Info.otherInfo[0]));
            BattleData.playerData.handCard.Remove(cardtodiscard);
            Deck.DrawCard();
        }
        else
        {
            Card cardtodiscard = Deck.FindCardInHand(BattleData.EnemyDataList[Info.owner_ID].handCard, int.Parse(Info.otherInfo[0]));
            BattleData.EnemyDataList[Info.owner_ID].enemy.UpdatePiles(cardtodiscard);
        }
    }
    private void Start()
    {
        Effect = "DiscardNotation";
    }

    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
