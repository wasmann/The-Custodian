using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sheep_Enemy : Enemy
{
<<<<<<< Updated upstream
    public enum ActionStates
    {
        Walk,
        Run,
        Headbutt,
        RushAndCollision,
        Unknown,
    }

    public enum PositionStates
=======
    public ActionStates CurrentActionState;
    public PositionStates CurrentPositionState;
    public int Health = 5;
    public string EnemyName;

    Sheep_Enemy(int ID)
    {
        this.EnemyID = ID;
        this.CurrentActionState = ActionStates.unknown;
        this.CurrentPositionState = PositionStates.notInRange;
    }
    public enum ActionStates
>>>>>>> Stashed changes
    {
        InRange,
        NotInRange,
        Unknown,
    }
<<<<<<< Updated upstream

    int ID;
    ActionStates CurrentActionState;
    PositionStates CurrentPositionState;

    Sheep_Enemy(int ID)
=======
    public enum PositionStates
>>>>>>> Stashed changes
    {
        this.ID = ID;
        this.CurrentActionState = ActionStates.Unknown;
        this.CurrentPositionState = PositionStates.Unknown;
    }
<<<<<<< Updated upstream
    
    
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
=======
    public override Card.InfoForActivate EnermyChooseACardToPlay(BattleData data, int ID)
    {
        Vector2 playerPosition = data.playerData.position;
        BattleData.EnermyData mydata = data.EnermyDataList[ID];
        List<Card> handCards = mydata.handCard;
        Vector2 selfPosition = mydata.position;

        if(playerPosition.x == selfPosition.x || playerPosition.y == selfPosition.y)
        {
            if (Vector2.Distance(playerPosition ,selfPosition)<= 3)
                this.CurrentPositionState = PositionStates.inRange;
>>>>>>> Stashed changes
        }
        // If no, try to move into range
        else
        {
<<<<<<< Updated upstream
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
=======
            if (Vector2.Distance(playerPosition, selfPosition) == 1)
            {
                if (Card.Contain(handCards,6))
                {
                    
                }
                else if (Card.Contain(handCards, 7))
                {
                    
>>>>>>> Stashed changes
                }
                // Otherwise, play whatever movement card we have at hand!
                else
                {

                }
            }
            // If we don't have any movement cards, discard one of the cards at hand to try to get a movement card.
            else
            {
<<<<<<< Updated upstream
                // Discard a card at hand!
=======
                if (Card.Contain(handCards, 7))
                {
                    //play the card
                }
                else if ()// 2345
                {
                    //choose direction play and return (newPosition = selfPosition - selfPosition - playerPosition)
                }
                else if (Card.Contain(handCards, 1))
                {
                    //choose direction play and return (newPosition = selfPosition - normalize(selfPosition - playerPosition))
                }
>>>>>>> Stashed changes
            }
        }
    }
}
