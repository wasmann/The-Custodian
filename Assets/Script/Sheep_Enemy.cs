using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sheep_Enemy : Enemy
{
    public enum ActionStates
    {
        Walk,
        Run,
        Headbutt,
        RushAndCollision,
        Unknown,
    }

    public enum PositionStates
    {
        InRange,
        NotInRange,
        Unknown,
    }

    int ID;
    ActionStates CurrentActionState;
    PositionStates CurrentPositionState;

    Sheep_Enemy(int ID)
    {
        this.ID = ID;
        this.CurrentActionState = ActionStates.Unknown;
        this.CurrentPositionState = PositionStates.Unknown;
    }
    
    
    public override int EnemyChooseACardToPlay(BattleData data)
    {
        Vector2 PlayerPosition = data.playerData.position;
        Vector2 SelfPosition = data.EnemyDataList[ID].position;


        // Are we in range?
            // If yes, do we have the headbutt attack card or rush attack?
                // We have both.
                    // If we are 1 grid away from the player, use the headbutt card. Otherwise, use rush attack.
                // We have the headbutt attack card.
                    // If we are 1 grid away, use the card.
                    // If we are more than 1 grid away, discard a card other than the headbutt card and try to get a movement card or the rush attack card.
                // We have the rush attack card.
                    // Use the card.
                // We have neither.
                    // Discard a card and try to get the headbutt card or rush attack card.
            // If no, try to move into range
                // Do we have a movement card?
                    // If yes, play it!
                    // If no, discard one of the cards at hand to get a movement card.

        // Are we in range?
        // If the enemy is directly , at most, 3 grid units to the right, left, above or below the enemy,
        // then the enemy is within range.
        if ((PlayerPosition.x == SelfPosition.x || PlayerPosition.y == SelfPosition.y) && 
            (Math.Abs(PlayerPosition.x - SelfPosition.x) <= 3 || Math.Abs(PlayerPosition.y - SelfPosition.y) <= 3))
        {
        }
        // If no, try to move into range
        else
        {
            // Do we have a movement card?
            if (data.playerData.handCard.Contains("ID for the walk card") || data.playerData.handCard.Contains("ID for the run card"))
            {
                // Do we have both cards?
                if (data.playerData.handCard.Contains("ID for the walk card") && data.playerData.handCard.Contains("ID for the run card"))
                {
                    // If we have both cards and the distance to get into range is greater then 1 grid, then use the run card.
                    if ((Math.Abs(PlayerPosition.x - SelfPosition.x) > 1 || Math.Abs(PlayerPosition.y - SelfPosition.y) > 1))
                    {
                        // Play the run card!
                    }
                    // If we have both cards and the distance to get into range is equal to 1 grid, then use the run card.
                    else if ((Math.Abs(PlayerPosition.x - SelfPosition.x) == 1 || Math.Abs(PlayerPosition.y - SelfPosition.y)== 1))
                    {
                        // Play the walk card!
                    }
                }
                // Otherwise, play whatever movement card we have at hand!
                else
                {

                }
            }
            // If we don't have any movement cards, discard one of the cards at hand to try to get a movement card.
            else
            {
                // Discard a card at hand!
            }
        }
    }
}
