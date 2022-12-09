using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sheep_Enemy : Enemy
{
    public string EnemyName;
    public int Health;
    public List<Card> Deck;
    public int EnemyID; // Not the ID in BattleData

    const int WALKCARDID = 1;
    const int RUNTOPCARDID = 2;
    const int RUNDOWNCARDID = 3;
    const int RUNLEFTCARDID = 4;
    const int RUNRIGHTCARDID = 5;
    const int HEADBUTTCARDID = 6;
    const int RUSHATTACKCARDID = 7;
    const int PHOTOSYNTHESISCARDID = 8;

    int WalkCardIndex = -1;
    int RunTopCardIndex = -1;
    int RunDownCardIndex = -1;
    int RunLeftCardIndex = -1;
    int RunRightCardIndex = -1;
    int HeadButtCardIndex = -1;
    int RushAttackCardIndex = -1;
    int PhotoSynthesisCardIndex = -1;

    bool HasWalkCardAtHand = false;
    bool HasRunTopCardAtHand = false;
    bool HasRunDownCardAtHand = false;
    bool HasRunLeftCardAtHand = false;
    bool HasRunRightCardAtHand = false;
    bool HasHeadbuttCardAtHand = false;
    bool HasRushAttackCardAtHand = false;
    bool HasPhotosynthesisCardAtHand = false;

    Sheep_Enemy(string EnemyName, int Health, int EnemyID)
    {
        this.EnemyName = EnemyName;
        this.Health = Health;
        this.EnemyID = EnemyID;
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
    public override void EnemyChooseACardToPlay(int ID)
    {
        EnemyData SheepData = EnemyDataList[EnemyID];
        Vector2 PlayerPosition = data.playerData.position;
        Vector2 SelfPosition = SheepData.position;
        Vector2 DifferenceVector = PlayerPosition - SelfPosition;
        float DifferenceInXCoordinate = Math.Abs(PlayerPosition.x - SelfPosition.x);
        float DifferenceInYCoordinate = Math.Abs(PlayerPosition.y - SelfPosition.y);

        // Check which cards we have and set the correct flags
        //foreach (var ACardAtHand in data.playerData.handCard)
        for (var i = 0; i < SheepData.handCard.Count; ++i)
        {
            if (SheepData.handCard[i].EnemyID == WALKCARDID)
            {
                HasWalkCardAtHand = true;
                WalkCardIndex = i;
            }
            else if (SheepData.handCard[i].EnemyID == RUNTOPCARDID)
            {
                HasRunTopCardAtHand = true;
                RunTopCardIndex = i;
            }
            else if (SheepData.handCard[i].EnemyID == RUNDOWNCARDID)
            {
                HasRunDownCardAtHand = true;
                RunDownCardIndex = i;
            }
            else if (SheepData.handCard[i].EnemyID == RUNLEFTCARDID)
            {
                HasRunLeftCardAtHand = true;
                RunLeftCardIndex = i;
            }
            else if (SheepData.handCard[i].EnemyID == RUNRIGHTCARDID)
            {
                HasRunRightCardAtHand = true;
                RunRightCardIndex = i;
            }
            else if (SheepData.handCard[i].EnemyID == HEADBUTTCARDID)
            {
                HasHeadbuttCardAtHand = true;
                HeadButtCardIndex = i;
            }
            else if (SheepData.handCard[i].EnemyID == RUSHATTACKCARDID)
            {
                HasRushAttackCardAtHand = true;
                RushAttackCardIndex = i;
            }
            else // if (SheepData.handCard[i].EnemyID == PHOTOSYNTHESISCARDID)
            {
                HasPhotosynthesisCardAtHand = true;
                PhotoSynthesisCardIndex = i;
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
                    SheepData.handCard[HeadButtCardIndex].Info.owner_ID = EnemyID;
                    SheepData.handCard[HeadButtCardIndex].Info.card = SheepData.handCard[HeadButtCardIndex];
                    SheepData.handCard[HeadButtCardIndex].Info.Selection = PlayerPosition;
                    NewCardPlayed(SheepData.handCard[HeadButtCardIndex].Info);
                    return;
                }
                // Otherwise, use rush attack as it does more damage.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {
                    SheepData.handCard[RushAttackCardIndex].Info.owner_ID = EnemyID;
                    SheepData.handCard[RushAttackCardIndex].Info.card = SheepData.handCard[RushAttackCardIndex];
                    SheepData.handCard[RushAttackCardIndex].Info.Selection = PlayerPosition;
                    NewCardPlayed(SheepData.handCard[RushAttackCardIndex].Info);
                    return;
                }
            }
            else if (HasHeadbuttCardAtHand)
            {
                // If we are 1 grid away, use the card.
                if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                {
                    SheepData.handCard[HeadButtCardIndex].Info.owner_ID = EnemyID;
                    SheepData.handCard[HeadButtCardIndex].Info.card = SheepData.handCard[HeadButtCardIndex];
                    SheepData.handCard[HeadButtCardIndex].Info.Selection = PlayerPosition;
                    NewCardPlayed(data.playerData.handCard[HeadButtCardIndex].Info);
                    return;
                }
                // If we are more than 1 grid away but we have a movement card, then try to use it!
                else if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
                {
                    // If we are closer in the x coordinate and the player is roughly above us then use a run top card in that direction unless our x position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                        SheepData.handCard[RunTopCardIndex].Info.Selection = (0, 2);
                        NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly below us then use a run down card in that direction unless our x position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && !(PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        // CONTINUE, YOU MIGHT HAVE TO REWRITE / CHANGE ALL OF THE X AND Y CHECKS BECAUSE X IS HORIZONTAL WHILE Y IS VERTICAL
                    }
                    // If we are closer in the y coordinate and the player is roughly to the right of us then use a run right card in that direction unless our y position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        data.playerData.handCard[RunDownCardIndex].Info.direction = (0, PlayerPosition.y - SelfPosition.y);
                        data.playerData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                        data.playerData.handCard[RunDownCardIndex].Info.card = data.playerData.handCard[RunDownCardIndex];
                        NewCardPlayed(data.playerData.handCard[RunDownCardIndex].Info);
                    }
                    // If we are closer in the y coordinate and the player is roughly to the left of us then use a run left card in that direction unless our y position is aligned with that of the player
                    // because once we align an axis, the goal is to just get closer in the opposite axis and get in range to attack. We don't want to destroy
                    // our allignment.
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && !(PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && (PlayerPosition.y < SelfPosition.y))
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
                    if (DifferenceInXCoordinate >= 5 || DifferenceInYCoordinate >= 5)
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
                    // If we have both cards and the distance to get into range is equal to 1 grid, then use the walk card.
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
                DrawCard()
            }
        }

        // Reset indices
        WalkCardIndex = -1;
        RunTopCardIndex = -1;
        RunDownCardIndex = -1;
        RunLeftCardIndex = -1;
        RunRightCardIndex = -1;
        HeadButtCardIndex = -1;
        RushAttackCardIndex = -1;
        PhotoSynthesisCardIndex = -1;

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
