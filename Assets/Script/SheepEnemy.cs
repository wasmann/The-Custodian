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
    
    // This function follows the logic detailed below:
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
    // TO DO: In the next updates, make the direction random and consider factors such as obstacles etc.
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
        // If the enemy is directly , at most, 3 grid units to the right, left, above or below the enemy,
        // then the enemy is within range. We check if we are aligned in our x or y axis to make sure we
        // are not at a diagonal position to the player.
        if ((PlayerPosition.x == SelfPosition.x || PlayerPosition.y == SelfPosition.y) && 
            (DifferenceInXCoordinate <= 3 || DifferenceInYCoordinate <= 3))
        {
            if (HasHeadbuttCardAtHand && HasRushAttackCardAtHand)
            {
                // If we are 1 grid away from the player, use the headbutt card.
                if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                {

                }
                // Otherwise, use rush attack as it does more damage.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {

                }
            }
            else if (HasHeadbuttCardAtHand)
            {
                // If we are 1 grid away, use the card.
                if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                {

                }
                // If we are more than 1 grid away but we have a movement card, then try to use it!
                else if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
                {
                    // If we are closer in the x coordinate and the player is roughly above us then use a run top card in that direction unless our x position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {

                    }
                    // If we are closer in the x coordinate and the player is roughly below us then use a run down card in that direction unless our x position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {

                    }
                    // If we are closer in the y coordinate and the player is roughly to the right of us then use a run right card in that direction unless our y position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {

                    }
                    // If we are closer in the y coordinate and the player is roughly to the left of us then use a run left card in that direction unless our y position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                    // move up.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                    // move down.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                    // move right.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                    // move left.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {

                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else 
                    {
                        // If we are closer in the x coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {

                        }
                        // If we are closer in the x coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                        {

                        }
                    }       
                }
                // If we are more than 1 grid away, discard a card other than the headbutt card and try to get a movement card or the rush attack card.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {

                }
            }
            else if (HasRushAttackCardAtHand)
            {
                // If we only have the rush attack card, then play it!
            }
            else
            {
                // If we have neither attack card, then discard a card and try to get the headbutt card or rush attack card!
            }
        }
        // If we are not in range, try to move into range!
        else
        {
            // Do we have a movement card?
            if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
            {
                // Do we have both the walk and run cards?
                if (HasWalkCardAtHand && (HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand))
                {
                    // If we have both types of cards and the distance to get into range is greater then 1 grid, then use a run card.
                    if (DifferenceInXCoordinate > 5 || DifferenceInYCoordinate > 5)
                    {
                        // If we are closer in the x coordinate and the player is roughly above us then use a run top card in that direction unless our x position is aligned with that of the player
                        // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                        // our allignment.
                        if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {

                        }
                        // If we are closer in the x coordinate and the player is roughly below us then use a run down card in that direction unless our x position is aligned with that of the player
                        // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                        // our allignment.
                        else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the right of us then use a run right card in that direction unless our y position is aligned with that of the player
                        // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                        // our allignment.
                        else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the left of us then use a run left card in that direction unless our y position is aligned with that of the player
                        // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                        // our allignment.
                        else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {

                        }
                        // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                        // move up.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {

                        }
                        // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                        // move down.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {

                        }
                        // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                        // move right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {

                        }
                        // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                        // move left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {

                        }
                        // If we, however, don't have the run card we need, then play the walk card instead!
                        else 
                        {
                            // If we are closer in the x coordinate and the player is roughly above us then walk upwards.
                            if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                            {

                            }
                            // If we are closer in the x coordinate and the player is roughly below us then walk downwards.
                            else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                            {

                            }
                            // If we are closer in the y coordinate and the player is roughly to the right of us then walk towards right.
                            else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                            {

                            }
                            // If we are closer in the y coordinate and the player is roughly to the left of us then walk towards left.
                            else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                            {

                            }
                            // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                            {

                            }
                            // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                            {

                            }
                            // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                            {

                            }
                            // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                            {

                            }
                        }
                    }
                    // If we have both cards and the distance to get into range is equal to 1 grid, then use the run card.
                    else if (DifferenceInXCoordinate == 4 || DifferenceInYCoordinate == 4)
                    {
                        // If we are closer in the x coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {

                        }
                        // If we are closer in the x coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                        {

                        }
                    }
                }
                // Otherwise, play whatever movement card we have at hand!
                else
                {
                    // If we are closer in the x coordinate and the player is roughly above us then use a run top card in that direction unless our x position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {

                    }
                    // If we are closer in the x coordinate and the player is roughly below us then use a run down card in that direction unless our x position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {

                    }
                    // If we are closer in the y coordinate and the player is roughly to the right of us then use a run right card in that direction unless our y position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {

                    }
                    // If we are closer in the y coordinate and the player is roughly to the left of us then use a run left card in that direction unless our y position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                    // move up.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                    // move down.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                    // move right.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {

                    }
                    // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                    // move left.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {

                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else 
                    {
                        // If we are closer in the x coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {

                        }
                        // If we are closer in the x coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                        {

                        }
                        // If we are closer in the y coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                        {

                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                        {

                        }
                    }
                }
            }
            // If we don't have any movement cards, discard one of the cards at hand to try to get a movement card.
            else
            {
                // Discard a non movement card at hand!
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
