using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep_Enemy : Enemy
{
    public ActionStates CurrentActionState;
    public PositionStates CurrentPositionState;

    Sheep_Enemy(int ID)
    {
        this.ID = ID;
        this.CurrentActionState = ActionStates.unknown;
        this.CurrentPositionState = PositionStates.notInRange;
    }
    enum ActionStates
    {
        walk,
        run,
        headbutt,
        rushAndCollision,
        unknown,
    }
    enum PositionStates
    {
        inRange,
        notInRange,
    }
    public override void EnermyChooseACardToPlay(BattleData data)
    {
        Vector2 playerPosition = data.playerData.position;
        Vector2 selfPosition = data.EnermyDataList.at(ID).position;
        if(playerPosition.x == selfPosition.x || playerPosition.y == selfPosition.y)
        {
            if ((playerPosition - selfPosition).Length <= 3)
                this.CurrentPositionState = PositionStates.inRange;
        }
        if(this.CurrentPositionState == PositionStates.inRange)
        {
            if ((playerPosition - selfPosition).Length == 1)
            {
                if (data.EnermyDataList.at(ID).handCard.OfType<Headbutt_Card>().FirstOrDefault() != null)
                {
                    //play the card
                }
                else if (data.EnermyDataList.at(ID).handCard.OfType<RushAndCollisionAttack_Card>().FirstOrDefault() != null)
                {
                    //play the card
                }
                else
                {
                    //discard card and return

                }
            }
            else
            {
                if (data.EnermyDataList.at(ID).handCard.OfType<RushAndCollisionAttack_Card>().FirstOrDefault() != null)
                {
                    //play the card
                }
                else if (data.EnermyDataList.at(ID).handCard.OfType<Run_Card>().FirstOrDefault() != null)
                {
                    //choose direction play and return (newPosition = selfPosition - selfPosition - playerPosition)
                }
                else if (data.EnermyDataList.at(ID).handCard.OfType<Walk_Card>().FirstOrDefault() != null)
                {
                    //choose direction play and return (newPosition = selfPosition - normalize(selfPosition - playerPosition))
                }
            }
        }
        else
        {
            //TODO implement out of range case
        }
        //foreach (var card in data.EnermyDataList.at(ID).handCard)
        //{

        //    switch (card.GetType())
        //    {
        //        case typeof(Walk_Card):
        //            CurrentSate = States.walk;
        //        case typeof(Run_Card):
        //            return 3;
        //        case typeof(Headbutt_Card):
        //            return 0;
        //        case typeof(RushAndCollisionAttack_Card):
        //            return 2;
        //        default:
        //            return -1;
        //    }
        //}
    }
}
