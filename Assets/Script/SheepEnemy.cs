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

    const int WALKCARDID = 1;
    const int RUNTOPCARDID = 2;
    const int RUNDOWNCARDID = 3;
    const int RUNLEFTCARDID = 4;
    const int RUNRIGHTCARDID = 5;
    const int HEADBUTTCARDID = 6;
    const int RUSHATTACKCARDID = 7;
    const int PHOTOSYNTHESISCARDID = 8;

    bool HasWalkCardAtHand = false;
    bool HasRunTopCardAtHand = false;
    bool HasRunDownCardAtHand = false;
    bool HasRunLeftCardAtHand = false;
    bool HasRunRightCardAtHand = false;
    bool HasHeadbuttCardAtHand = false;
    bool HasRushAttackCardAtHand = false;
    bool HasPhotosynthesisCardAtHand = false;

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
        Vector2 DifferenceVector = PlayerPosition - SelfPosition;
        float DifferenceInXCoordinate = Math.Abs(PlayerPosition.x - SelfPosition.x);
        float DifferenceInYCoordinate = Math.Abs(PlayerPosition.y - SelfPosition.y);

        // Check which cards we have and set the correct flags
        foreach (var ACardAtHand in data.playerData.handCard)
        {
            if (ACardAtHand.ID == WALKCARDID)
            {
                HasWalkCardAtHand = true;
            }
            else if (ACardAtHand.ID == RUNTOPCARDID)
            {
                HasRunTopCardAtHand = true;
            }
            else if (ACardAtHand.ID == RUNDOWNCARDID)
            {
                HasRunDownCardAtHand = true;
            }
            else if (ACardAtHand.ID == RUNLEFTCARDID)
            {
                HasRunLeftCardAtHand = true;
            }
            else if (ACardAtHand.ID == RUNRIGHTCARDID)
            {
                HasRunRightCardAtHand = true;
            }
            else if (ACardAtHand.ID == HEADBUTTCARDID)
            {
                HasHeadbuttCardAtHand = true;
            }
            else if (ACardAtHand.ID == RUSHATTACKCARDID)
            {
                HasRushAttackCardAtHand = true;
            }
            else // if (ACardAtHand.ID == PHOTOSYNTHESISCARDID)
            {
                HasPhotosynthesisCardAtHand = true;
            }
        }


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
        // then the enemy is within range. We check if we are aligned in our x or y axis to make sure we
        // are not at a diagonal position to the player.
        if ((PlayerPosition.x == SelfPosition.x || PlayerPosition.y == SelfPosition.y) && 
            (DifferenceInXCoordinate <= 3 || DifferenceInYCoordinate <= 3))
        {
        }
        // If no, try to move into range
        else
        {
            // Do we have a movement card?
            if (HasWalkCardAtHand == true ||
                HasRunTopCardAtHand == true ||
                HasRunDownCardAtHand == true ||
                HasRunLeftCardAtHand == true ||
                HasRunRightCardAtHand == true)
            {
                // Do we have both the walk and run cards?
                if (HasWalkCardAtHand == true && (HasRunTopCardAtHand == true ||
                                                  HasRunDownCardAtHand == true ||
                                                  HasRunLeftCardAtHand == true ||
                                                  HasRunRightCardAtHand == true))
                {
                    // If we have both types of cards and the distance to get into range is greater then 1 grid, then use a run card.
                    if (DifferenceInXCoordinate > 5 || DifferenceInYCoordinate > 5)
                    {
                        // If we are closer in the x coordinate then use a run card in that direction unless our x position is aligned with that of the player
                        // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                        // our allignment.
                        if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x))
                        {

                        }
                        // If we are closer in the y coordinate then use a run card in that direction unless our y position is aligned with that of the player
                        // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                        // our allignment.
                        else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x))
                        {

                        }
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate) // CONTINUE
                        {

                        }
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

        // Reset the flags
        HasWalkCardAtHand = false;
        HasRunTopCardAtHand = false;
        HasRunDownCardAtHand = false;
        HasRunLeftCardAtHand = false;
        HasRunRightCardAtHand = false;
        HasHeadbuttCardAtHand = false;
        HasRushAttackCardAtHand = false;
        HasPhotosynthesisCardAtHand = false;
    }
}
