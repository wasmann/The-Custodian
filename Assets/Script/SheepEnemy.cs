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
                    NewCardPlayed(SheepData.handCard[HeadButtCardIndex].Info);
                    return;
                }
                // If we are more than 1 grid away but we have a movement card, then try to use it!
                else if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
                {
                    // If we are closer in the y coordinate and the player is roughly above us then use a run top card in that direction!
                    if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                        SheepData.handCard[RunTopCardIndex].Info.Selection = (0, -(DifferenceInYCoordinate - 1)); // Move 2 or 1 units to be right under the player (1 grid below)
                        NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                        return;
                    }
                    // If we are closer in the y coordinate and the player is roughly below us then use a run down card in that direction!
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        SheepData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunDownCardIndex].Info.card = SheepData.handCard[RunDownCardIndex];
                        SheepData.handCard[RunDownCardIndex].Info.Selection = (0, DifferenceInYCoordinate - 1); // Move 2 or 1 units down to be right above the player (1 grid above)
                        NewCardPlayed(SheepData.handCard[RunDownCardIndex].Info);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the right of us then use a run right card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        SheepData.handCard[RunRightCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunRightCardIndex].Info.card = SheepData.handCard[RunRightCardIndex];
                        SheepData.handCard[RunRightCardIndex].Info.Selection = (DifferenceInXCoordinate - 1, 0); // Move 2 or 1 units right to be exactly to left of the player (1 grid to the left)
                        NewCardPlayed(SheepData.handCard[RunRightCardIndex].Info);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the left of us then use a run left card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        SheepData.handCard[RunLeftCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunLeftCardIndex].Info.card = SheepData.handCard[RunLeftCardIndex];
                        SheepData.handCard[RunLeftCardIndex].Info.Selection = (-(DifferenceInXCoordinate - 1), 0); // Move 2 or 1 units left to be exactly to right of the player (1 grid to the right)
                        NewCardPlayed(SheepData.handCard[RunLeftCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                    // then move up.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                        SheepData.handCard[RunTopCardIndex].Info.Selection = (0, -(DifferenceInYCoordinate - 1)); // Move 2 or 1 units up to be right under the player (1 grid below)
                        NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                    // then move down.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        SheepData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunDownCardIndex].Info.card = SheepData.handCard[RunDownCardIndex];
                        SheepData.handCard[RunDownCardIndex].Info.Selection = (0, DifferenceInYCoordinate - 1); // Move 2 or 1 units down to be right above the player (1 grid above)
                        NewCardPlayed(SheepData.handCard[RunDownCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                    // then move right.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        SheepData.handCard[RunRightCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunRightCardIndex].Info.card = SheepData.handCard[RunRightCardIndex];
                        SheepData.handCard[RunRightCardIndex].Info.Selection = (DifferenceInXCoordinate - 1, 0); // Move 2 or 1 units right to be exactly to left of the player (1 grid to the left)
                        NewCardPlayed(SheepData.handCard[RunRightCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                    // then move left.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        SheepData.handCard[RunLeftCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunLeftCardIndex].Info.card = SheepData.handCard[RunLeftCardIndex];
                        SheepData.handCard[RunLeftCardIndex].Info.Selection = (-(DifferenceInXCoordinate - 1), 0); // Move 2 or 1 units left be exactly to right of the player (1 grid to the right)
                        NewCardPlayed(SheepData.handCard[RunLeftCardIndex].Info);
                        return;
                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else 
                    {
                        // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit right
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit left
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit right
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit left
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
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
                SheepData.handCard[RushAttackCardIndex].Info.owner_ID = EnemyID;
                SheepData.handCard[RushAttackCardIndex].Info.card = SheepData.handCard[RushAttackCardIndex];
                SheepData.handCard[RushAttackCardIndex].Info.Selection = PlayerPosition;
                NewCardPlayed(SheepData.handCard[RushAttackCardIndex].Info);
                return;
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
                        // If we are closer in the y coordinate and the player is roughly above us then use a run top card in that direction!
                        if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {
                            SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                            SheepData.handCard[RunTopCardIndex].Info.Selection = (0, -3); // We want to get as closer as we can so we run the maximum distance up.
                            NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then use a run down card in that direction!
                        else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {
                            SheepData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunDownCardIndex].Info.card = SheepData.handCard[RunDownCardIndex];
                            SheepData.handCard[RunDownCardIndex].Info.Selection = (0, 3); // We want to get as closer as we can so we run the maximum distance down.
                            NewCardPlayed(SheepData.handCard[RunDownCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then use a run right card in that direction!
                        else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {
                            SheepData.handCard[RunRightCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunRightCardIndex].Info.card = SheepData.handCard[RunRightCardIndex];
                            SheepData.handCard[RunRightCardIndex].Info.Selection = (3, 0); // We want to get as closer as we can so we run the maximum distance to the right.
                            NewCardPlayed(SheepData.handCard[RunRightCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then use a run left card in that direction!
                        else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {
                            SheepData.handCard[RunLeftCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunLeftCardIndex].Info.card = SheepData.handCard[RunLeftCardIndex];
                            SheepData.handCard[RunLeftCardIndex].Info.Selection = (-3, 0); // We want to get as closer as we can so we run the maximum distance to the left.
                            NewCardPlayed(SheepData.handCard[RunLeftCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                        // then move up.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {
                            SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                            SheepData.handCard[RunTopCardIndex].Info.Selection = (0, -3); // We want to get as closer as we can so we run the maximum distance up.
                            NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                        // then move down.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {
                            SheepData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunDownCardIndex].Info.card = SheepData.handCard[RunDownCardIndex];
                            SheepData.handCard[RunDownCardIndex].Info.Selection = (0, 3); // We want to get as closer as we can so we run the maximum distance down.
                            NewCardPlayed(SheepData.handCard[RunDownCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                        // then move right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {
                            SheepData.handCard[RunRightCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunRightCardIndex].Info.card = SheepData.handCard[RunRightCardIndex];
                            SheepData.handCard[RunRightCardIndex].Info.Selection = (3, 0); // We want to get as closer as we can so we run the maximum distance to the right.
                            NewCardPlayed(SheepData.handCard[RunRightCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                        // then move left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {
                            SheepData.handCard[RunLeftCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[RunLeftCardIndex].Info.card = SheepData.handCard[RunLeftCardIndex];
                            SheepData.handCard[RunLeftCardIndex].Info.Selection = (-3, 0); // We want to get as closer as we can so we run the maximum distance to the left.
                            NewCardPlayed(SheepData.handCard[RunLeftCardIndex].Info);
                            return;
                        }
                        // If we, however, don't have the run card we need, then play the walk card instead!
                        else 
                        {
                            // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                            if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                            else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                            else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit right
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                            else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit left
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit right
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                            {
                                SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                                SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                                SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit left
                                NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                                return;
                            }
                        }
                    }
                    // If we have both cards and the distance to get into range is equal to 1 grid, then use the walk card.
                    else if (DifferenceInXCoordinate == 4 || DifferenceInYCoordinate == 4)
                    {
                        // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit right
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit left
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit to the right
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;   
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit to the left
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                    }
                }
                // Otherwise, play whatever movement card we have at hand!
                else
                {
                    // If we are closer in the y coordinate and the player is roughly above us then use a run top card in that direction!
                    if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                        SheepData.handCard[RunTopCardIndex].Info.Selection = (0, -3); // We want to get as closer as we can so we run the maximum distance up.
                        NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                        return;
                    }
                    // If we are closer in the y coordinate and the player is roughly below us then use a run down card in that direction!
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        SheepData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunDownCardIndex].Info.card = SheepData.handCard[RunDownCardIndex];
                        SheepData.handCard[RunDownCardIndex].Info.Selection = (0, 3); // We want to get as closer as we can so we run the maximum distance down.
                        NewCardPlayed(SheepData.handCard[RunDownCardIndex].Info);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the right of us then use a run right card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        SheepData.handCard[RunRightCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunRightCardIndex].Info.card = SheepData.handCard[RunRightCardIndex];
                        SheepData.handCard[RunRightCardIndex].Info.Selection = (3, 0); // We want to get as closer as we can so we run the maximum distance to the right.
                        NewCardPlayed(SheepData.handCard[RunRightCardIndex].Info);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the left of us then use a run left card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        SheepData.handCard[RunLeftCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunLeftCardIndex].Info.card = SheepData.handCard[RunLeftCardIndex];
                        SheepData.handCard[RunLeftCardIndex].Info.Selection = (-3, 0); // We want to get as closer as we can so we run the maximum distance to the left.
                        NewCardPlayed(SheepData.handCard[RunLeftCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                    // then move up.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        SheepData.handCard[RunTopCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunTopCardIndex].Info.card = SheepData.handCard[RunTopCardIndex];
                        SheepData.handCard[RunTopCardIndex].Info.Selection = (0, -3); // We want to get as closer as we can so we run the maximum distance up.
                        NewCardPlayed(SheepData.handCard[RunTopCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                    // then move down.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        SheepData.handCard[RunDownCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunDownCardIndex].Info.card = SheepData.handCard[RunDownCardIndex];
                        SheepData.handCard[RunDownCardIndex].Info.Selection = (0, 3); // We want to get as closer as we can so we run the maximum distance down.
                        NewCardPlayed(SheepData.handCard[RunDownCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                    // then move right.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        SheepData.handCard[RunRightCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunRightCardIndex].Info.card = SheepData.handCard[RunRightCardIndex];
                        SheepData.handCard[RunRightCardIndex].Info.Selection = (3, 0); // We want to get as closer as we can so we run the maximum distance to the right.
                        NewCardPlayed(SheepData.handCard[RunRightCardIndex].Info);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                    // move left.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        SheepData.handCard[RunLeftCardIndex].Info.owner_ID = EnemyID;
                        SheepData.handCard[RunLeftCardIndex].Info.card = SheepData.handCard[RunLeftCardIndex];
                        SheepData.handCard[RunLeftCardIndex].Info.Selection = (-3, 0); // We want to get as closer as we can so we run the maximum distance to the left.
                        NewCardPlayed(SheepData.handCard[RunLeftCardIndex].Info);
                        return;
                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else 
                    {
                        // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit to the right
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit to the left
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, -1); // Move one unit up
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (0, 1); // Move one unit down
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (1, 0); // Move one unit right
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x)
                        {
                            SheepData.handCard[WalkCardIndex].Info.owner_ID = EnemyID;
                            SheepData.handCard[WalkCardIndex].Info.card = SheepData.handCard[WalkCardIndex];
                            SheepData.handCard[WalkCardIndex].Info.Selection = (-1, 0); // Move one unit left
                            NewCardPlayed(SheepData.handCard[WalkCardIndex].Info);
                            return;
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
