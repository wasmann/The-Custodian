using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SheepEnemy : Enemy
{
    public override string EnemyName { get { return "Sheep"; } }

    public List<Card> deck;
    public override List<Card> CardsDeck
    {
        get
        {
            return deck;
        }
    }
    public override int Health { get { return 5; } }

    public override int HandCardNum { get { return 3; } }


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
    public override void EnemyChooseACardToPlay()
    {
        resetFlags();
        BattleData.EnemyData SheepData = BattleData.EnemyDataList[EnemyID];
        Vector2 PlayerPosition = BattleData.playerData.position;
        Vector2 SelfPosition = SheepData.position;
        Vector2 DifferenceVector = PlayerPosition - SelfPosition;
        float DifferenceInXCoordinate = Math.Abs(PlayerPosition.x - SelfPosition.x);
        float DifferenceInYCoordinate = Math.Abs(PlayerPosition.y - SelfPosition.y);

        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.Selection = new List<Vector2>();

        // Check which cards we have and set the correct flags
        //foreach (var ACardAtHand in data.playerData.handCard)
        for (var i = 0; i < SheepData.handCard.Count; ++i)
        {
            Debug.Log(SheepData.handCard[i].ID);
            if (SheepData.handCard[i].ID == WALKCARDID) {
                HasWalkCardAtHand = true;
            }
            else if (SheepData.handCard[i].ID == RUNTOPCARDID) { 
                HasRunTopCardAtHand = true;
            }
            else if (SheepData.handCard[i].ID == RUNDOWNCARDID)
            {
                HasRunDownCardAtHand = true;
            }
            else if (SheepData.handCard[i].ID == RUNLEFTCARDID)
            {
                HasRunLeftCardAtHand = true;
            }
            else if (SheepData.handCard[i].ID == RUNRIGHTCARDID)
            {
                HasRunRightCardAtHand = true;
            }
            else if (SheepData.handCard[i].ID == HEADBUTTCARDID)
            {
                HasHeadbuttCardAtHand = true;
            }
            else if (SheepData.handCard[i].ID == RUSHATTACKCARDID)
            {
                HasRushAttackCardAtHand = true;
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
                    info.card = Deck.FindCardInHand(SheepData.handCard, HEADBUTTCARDID);
                    info.Selection.Add(PlayerPosition-SelfPosition);
                    BattleLevelDriver.NewCardPlayed(info);
                    UpdatePiles(info.card);
                    return;
                }
                // Otherwise, use rush attack as it does more damage.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {
                    info.card = Deck.FindCardInHand(SheepData.handCard, RUSHATTACKCARDID);
                    info.Selection.Add(PlayerPosition - SelfPosition);
                    BattleLevelDriver.NewCardPlayed(info);
                    UpdatePiles(info.card);
                    return;
                }
            }
            else if (HasHeadbuttCardAtHand)
            {
                // If we are 1 grid away, use the card.
                if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                {
                    info.card = Deck.FindCardInHand(SheepData.handCard, HEADBUTTCARDID);
                    info.Selection.Add(PlayerPosition - SelfPosition);
                    BattleLevelDriver.NewCardPlayed(info);
                    UpdatePiles(info.card);
                    return;
                }
                // If we are more than 1 grid away but we have a movement card, then try to use it!
                else if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
                {
                    // If we are closer in the y coordinate and the player is roughly above us then use a run top card in that direction!
                    if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    { 
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                        info.Selection.Add(new Vector2(0, -(DifferenceInYCoordinate - 1)));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the y coordinate and the player is roughly below us then use a run down card in that direction!
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                        info.Selection.Add(new Vector2(0, DifferenceInYCoordinate - 1));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the right of us then use a run right card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                        info.Selection.Add(new Vector2(DifferenceInXCoordinate - 1, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the left of us then use a run left card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                        info.Selection.Add(new Vector2(-(DifferenceInXCoordinate - 1), 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                    // then move up.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                        info.Selection.Add(new Vector2(0, -(DifferenceInYCoordinate - 1)));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                    // then move down.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                        info.Selection.Add(new Vector2(0, DifferenceInYCoordinate - 1));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                    // then move right.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                        info.Selection.Add(new Vector2(DifferenceInXCoordinate - 1, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                    // then move left.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                        info.Selection.Add(new Vector2(-(DifferenceInXCoordinate - 1), 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else
                    {
                        // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {

                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                    }
                }
                // If we are more than 1 grid away, discard a card other than the headbutt card and try to get a movement card or the rush attack card.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {
                    Debug.Log("???");
                    return;
                }
            }
            else if (HasRushAttackCardAtHand)
            {


                info.card = Deck.FindCardInHand(SheepData.handCard, RUSHATTACKCARDID);
                info.Selection.Add(PlayerPosition - SelfPosition);
                BattleLevelDriver.NewCardPlayed(info);
                UpdatePiles(info.card);
                return;
            }
            else
            {
                Debug.Log("HERERERER!");
                return;
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
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                            info.Selection.Add(new Vector2(0, 3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then use a run down card in that direction!
                        else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                            info.Selection.Add(new Vector2(0, -3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then use a run right card in that direction!
                        else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                            info.Selection.Add(new Vector2(3,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then use a run left card in that direction!
                        else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                            info.Selection.Add(new Vector2(-3, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                        // then move up.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                            info.Selection.Add(new Vector2(0,3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                        // then move down.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                            info.Selection.Add(new Vector2(0, -3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                        // then move right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                            info.Selection.Add(new Vector2(3,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                        // then move left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                            info.Selection.Add(new Vector2(-3,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we, however, don't have the run card we need, then play the walk card instead!
                        else
                        {
                            // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                            if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, -1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                            else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, 1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                            else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(1, 0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                            else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(-1, 0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, -1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, 1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(1,0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                            else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(-1,0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                        }
                    }
                    // If we have both cards and the distance to get into range is equal to 1 grid, then use the walk card.
                    else if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                    {
                        // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2( -1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
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
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                        info.Selection.Add(new Vector2(0, 3));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the y coordinate and the player is roughly below us then use a run down card in that direction!
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                        info.Selection.Add(new Vector2(0, -3));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the right of us then use a run right card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                        info.Selection.Add(new Vector2(3,0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate and the player is roughly to the left of us then use a run left card in that direction!
                    else if ((DifferenceInXCoordinate < DifferenceInYCoordinate) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                        info.Selection.Add(new Vector2(-3,0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run top card and the player is roughly above us,
                    // then move up.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                        info.Selection.Add(new Vector2(0,3));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run down card and the player is roughly below us,
                    // then move down.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                        info.Selection.Add(new Vector2(0,-3));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run right card and the player is roughly to the right of us,
                    // then move right.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                        info.Selection.Add(new Vector2(3, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If the distance difference is the same in both coordinates, we have the run left card and the player is roughly to the left of us,
                    // move left.
                    else if (DifferenceInXCoordinate == DifferenceInYCoordinate && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                        info.Selection.Add(new Vector2(-3, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else
                    {
                        // If we are closer in the y coordinate and the player is roughly above us then walk upwards.
                        if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y < SelfPosition.y &&HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0,1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate and the player is roughly below us then walk downwards.
                        else if (DifferenceInYCoordinate < DifferenceInXCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate < DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly above us then walk upwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly below us then walk downwards.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the right of us then walk towards right.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If the distance difference is the same in both coordinates and the player is roughly to the left of us then walk towards left.
                        else if (DifferenceInXCoordinate == DifferenceInYCoordinate && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1,0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                    }
                }
            }
            // If we don't have any movement cards, discard one of the cards at hand to try to get a movement card.
            else
            {
                Debug.Log("Discard case");
                return;
                // Discard a non movement card at hand!
            }
        }
        Debug.Log("What");
    }
    private  void resetFlags()
    {
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
