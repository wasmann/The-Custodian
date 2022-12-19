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

    bool PlayerIsAbove = false;
    bool PlayerIsBelow = false;
    bool PlayerIsToTheRight = false;
    bool PlayerIsToTheLeft = false;

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
        Debug.Log("Player Position: " + PlayerPosition);
        Debug.Log("Sheep Position: " + SelfPosition);

        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.otherInfo = new List<string>();
        info.Selection = new List<Vector2>();

        // Check which cards we have and set the correct flags
        for (var i = 0; i < SheepData.handCard.Count; ++i)
        {
            //Debug.Log(SheepData.handCard[i].ID);
            if (SheepData.handCard[i].ID == WALKCARDID) {
                HasWalkCardAtHand = true;
                Debug.Log("Walk card");
            }
            else if (SheepData.handCard[i].ID == RUNTOPCARDID) { 
                HasRunTopCardAtHand = true;
                Debug.Log("Run top card");
            }
            else if (SheepData.handCard[i].ID == RUNDOWNCARDID)
            {
                HasRunDownCardAtHand = true;
                Debug.Log("Run down card");
            }
            else if (SheepData.handCard[i].ID == RUNLEFTCARDID)
            {
                HasRunLeftCardAtHand = true;
                Debug.Log("Run left card");
            }
            else if (SheepData.handCard[i].ID == RUNRIGHTCARDID)
            {
                HasRunRightCardAtHand = true;
                Debug.Log("Run right card");
            }
            else if (SheepData.handCard[i].ID == HEADBUTTCARDID)
            {
                HasHeadbuttCardAtHand = true;
                Debug.Log("Headbutt card");
            }
            else if (SheepData.handCard[i].ID == RUSHATTACKCARDID)
            {
                HasRushAttackCardAtHand = true;
                Debug.Log("Rush attack card");
            }
        }

        // Is the player roughly above us?
        if (PlayerPosition.y > SelfPosition.y)
        {
            PlayerIsAbove = true;
        }
        // Is the player rougly below us?
        else // if (PlayerPosition.y < SelfPosition.y)
        {
            PlayerIsBelow = true;
        }

        // Is the player roughly to the right of us?
        if (PlayerPosition.x > SelfPosition.x)
        {
            PlayerIsToTheRight = true;
        }
        // Is the player rougly to the left of us?
        else // if (PlayerPosition.x < SelfPosition.x)
        {
            PlayerIsToTheLeft = true;
        }

        // Are we in range?
        // If the enemy is directly , at most, 3 grid units to the right, left, above or below the enemy,
        // then the enemy is within range. We check if we are aligned in our x or y axis to make sure we
        // are not at a diagonal position to the player.
        if ((PlayerPosition.x == SelfPosition.x && DifferenceInYCoordinate <= 3) || (PlayerPosition.y == SelfPosition.y && DifferenceInXCoordinate <= 3))
        {
            Debug.Log("In range!");
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
            else if (HasRushAttackCardAtHand)
            {
                info.card = Deck.FindCardInHand(SheepData.handCard, RUSHATTACKCARDID);
                info.Selection.Add(PlayerPosition - SelfPosition);
                BattleLevelDriver.NewCardPlayed(info);
                UpdatePiles(info.card);
                return;
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
                    // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then use a run top card in that direction!
                    if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    { 
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                        info.Selection.Add(new Vector2(0, (DifferenceInYCoordinate - 1)));
                        //info.Selection.Add(new Vector2(0, -(DifferenceInYCoordinate - 1)));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then use a run down card in that direction!
                    else if ((DifferenceInYCoordinate < DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    //else if ((DifferenceInYCoordinate < DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                        info.Selection.Add(new Vector2(0, -(DifferenceInYCoordinate - 1)));
                        //info.Selection.Add(new Vector2(0, DifferenceInYCoordinate - 1));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then use a run right card in that direction!
                    else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                        info.Selection.Add(new Vector2(DifferenceInXCoordinate - 1, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then use a run left card in that direction!
                    else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                        info.Selection.Add(new Vector2(-(DifferenceInXCoordinate - 1), 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else if (HasWalkCardAtHand)
                    {
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then walk upwards.
                        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            //info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then walk downwards.
                        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                        //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            //info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then walk towards right.
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then walk towards left.
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                    }
                    // If we are more than 1 grid away, discard a card other than the headbutt card and try to get a movement card or the rush attack card.
                    else
                    {
                        for(int i = 0; i < BattleData.EnemyDataList[EnemyID].handCard.Count; i++)
                        {
                            if (BattleData.EnemyDataList[EnemyID].handCard[i].ID != HEADBUTTCARDID)
                            {
                                info.otherInfo.Add(BattleData.EnemyDataList[EnemyID].handCard[i].ID + "");
                                break;
                            }
                        }
                        info.card = dicardManager;
                        BattleLevelDriver.NewCardPlayed(info);
                        return;   
                    }
                }
                // If we are more than 1 grid away, discard a card other than the headbutt card and try to get a movement card or the rush attack card.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {
                    for(int i = 0; i < BattleData.EnemyDataList[EnemyID].handCard.Count; i++)
                    {
                        if (BattleData.EnemyDataList[EnemyID].handCard[i].ID != HEADBUTTCARDID)
                        {
                            info.otherInfo.Add(BattleData.EnemyDataList[EnemyID].handCard[i].ID + "");
                            break;
                        }
                    }
                    info.card = dicardManager;
                    BattleLevelDriver.NewCardPlayed(info);
                    return;
                }
            }
            // If we don't have any attack card but we have movement cards and we can still get closer to the player, then try to!
            else if (!(DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1))
            {
                // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then use a run top card in that direction!
                if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y > SelfPosition.y))
                //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                { 
                    info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                    info.Selection.Add(new Vector2(0, (DifferenceInYCoordinate - 1)));
                    //info.Selection.Add(new Vector2(0, -(DifferenceInYCoordinate - 1)));
                    BattleLevelDriver.NewCardPlayed(info);
                    UpdatePiles(info.card);
                    return;
                }
                // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then use a run down card in that direction!
                else if ((DifferenceInYCoordinate < DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y < SelfPosition.y))
                //else if ((DifferenceInYCoordinate < DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                {
                    info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                    info.Selection.Add(new Vector2(0, -(DifferenceInYCoordinate - 1)));
                    //info.Selection.Add(new Vector2(0, DifferenceInYCoordinate - 1));
                    BattleLevelDriver.NewCardPlayed(info);
                    UpdatePiles(info.card);
                    return;
                }
                // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then use a run right card in that direction!
                else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                {
                    info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                    info.Selection.Add(new Vector2(DifferenceInXCoordinate - 1, 0));
                    BattleLevelDriver.NewCardPlayed(info);
                    UpdatePiles(info.card);
                    return;
                }
                // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then use a run left card in that direction!
                else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
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
                    // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then walk upwards.
                    if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                    //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                        info.Selection.Add(new Vector2(0, 1));
                        //info.Selection.Add(new Vector2(0, -1));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then walk downwards.
                    else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y && HasWalkCardAtHand)
                    //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y && HasWalkCardAtHand)
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                        info.Selection.Add(new Vector2(0, -1));
                        //info.Selection.Add(new Vector2(0, 1));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then walk towards right.
                    else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x > SelfPosition.x && HasWalkCardAtHand)
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                        info.Selection.Add(new Vector2(1, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then walk towards left.
                    else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x < SelfPosition.x && HasWalkCardAtHand)
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                        info.Selection.Add(new Vector2(-1, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("Discard");

                info.otherInfo.Add(BattleData.EnemyDataList[EnemyID].handCard[0].ID + "");
                info.card = dicardManager;
                BattleLevelDriver.NewCardPlayed(info);
                return;
                // If we have neither attack card, then discard a card and try to get the headbutt card or rush attack card!
            }
        }
        // If we are not in range, try to move into range!
        else
        {
            Debug.Log("Not in range!");
            // Do we have a movement card?
            if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
            {
                // Do we have both the walk and run cards?
                if (HasWalkCardAtHand && (HasRunTopCardAtHand || HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand))
                {
                    // If we have both types of cards and the distance to get into range is greater then 1 grid, then use a run card.
                    if ((DifferenceInXCoordinate >= 5 || DifferenceInYCoordinate >= 5) && !(DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1))
                    {
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then use a run top card in that direction!
                        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                            info.Selection.Add(new Vector2(0, 3));
                            //info.Selection.Add(new Vector2(0, -3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then use a run down card in that direction!
                        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                            info.Selection.Add(new Vector2(0, -3));
                            //info.Selection.Add(new Vector2(0, 3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then use a run right card in that direction!
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                            info.Selection.Add(new Vector2(3, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordiante and the player is roughly or exactly to the left of us then use a run left card in that direction!
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
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
                            // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then walk upwards.
                            if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                            //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, 1));
                                //info.Selection.Add(new Vector2(0, -1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then walk downwards.
                            else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                            //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, -1));
                                //info.Selection.Add(new Vector2(0, 1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then walk towards right.
                            else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x > SelfPosition.x)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(1, 0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the x coordinate or aligned in the y coordiante and the player is roughly or exactly to the left of us then walk towards left.
                            else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x < SelfPosition.x)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(-1, 0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                        }
                    }
                    // If we have both cards and the distance to get into range is equal to 1 grid, then use the walk card.
                    else if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                    {
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then walk upwards.
                        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                        //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            //info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then walk downwards.
                        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                        //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            //info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then walk towards right.
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x > SelfPosition.x)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then walk towards left.
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x < SelfPosition.x)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                    }
                    // If we have both cards and we are not in one of the two special conditions described above, then first try to play a run card to cover as much distance as possible. If that is not possible, then play a walk card!
                    else
                    {
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then use a run top card in that direction!
                        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                            if (DifferenceInYCoordinate < 3)
                            {
                                info.Selection.Add(new Vector2(0, DifferenceInYCoordinate));
                            }
                            else // if (DifferenceInYCoordinate >= 3)
                            {
                                info.Selection.Add(new Vector2(0, 3));
                            }
                            //DifferenceInYCoordinate < 3 ? info.Selection.Add(new Vector2(0, DifferenceInYCoordinate)) : info.Selection.Add(new Vector2(0, 3));
                            //info.Selection.Add(new Vector2(0, 3));
                            //info.Selection.Add(new Vector2(0, -3));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then use a run down card in that direction!
                        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y < SelfPosition.y))
                        //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                            if (DifferenceInYCoordinate < 3)
                            {
                                info.Selection.Add(new Vector2(0, -DifferenceInYCoordinate));
                            }
                            else // if (DifferenceInYCoordinate >= 3)
                            {
                                info.Selection.Add(new Vector2(0, -3));
                            }
                            //DifferenceInYCoordinate < 3 ? info.Selection.Add(new Vector2(0, -DifferenceInYCoordinate)) : info.Selection.Add(new Vector2(0, -3));
                            //info.Selection.Add(new Vector2(0, -3));
                            //info.Selection.Add(new Vector2(0, 3));
                            Debug.Log("Updated info.Selection value:" + info.Selection[0]);
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then use a run right card in that direction!
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                            if (DifferenceInXCoordinate < 3)
                            {
                                info.Selection.Add(new Vector2(DifferenceInXCoordinate, 0));
                            }
                            else // if (DifferenceInXCoordinate >= 3)
                            {
                                info.Selection.Add(new Vector2(3, 0));
                            }
                            //DifferenceInXCoordinate < 3 ? info.Selection.Add(new Vector2(DifferenceInXCoordinate, 0)) : info.Selection.Add(new Vector2(3, 0));
                            //info.Selection.Add(new Vector2(3, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then use a run left card in that direction!
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                            if (DifferenceInXCoordinate < 3)
                            {
                                info.Selection.Add(new Vector2(-DifferenceInXCoordinate, 0));
                            }
                            else // if (DifferenceInXCoordinate >= 3)
                            {
                                info.Selection.Add(new Vector2(-3, 0));
                            }
                            //DifferenceInXCoordinate < 3 ? info.Selection.Add(new Vector2(-DifferenceInXCoordinate, 0)) : info.Selection.Add(new Vector2(-3, 0));
                            //info.Selection.Add(new Vector2(-3, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we, however, don't have the run card we need, then play the walk card instead!
                        else if (HasWalkCardAtHand)
                        {
                            // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then walk upwards.
                            if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                            //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, 1));
                                //info.Selection.Add(new Vector2(0, -1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then walk downwards.
                            else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                            //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(0, -1));
                                //info.Selection.Add(new Vector2(0, 1));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then walk towards right.
                            else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x > SelfPosition.x)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(1, 0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                            // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then walk towards left.
                            else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x < SelfPosition.x)
                            {
                                info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                                info.Selection.Add(new Vector2(-1, 0));
                                BattleLevelDriver.NewCardPlayed(info);
                                UpdatePiles(info.card);
                                return;
                            }
                        }
                    }
                }
                // Otherwise, play whatever movement card we have at hand!
                else
                {
                    Debug.Log("Play whatever movement card we have at hand!");
                    // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then use a run top card in that direction!
                    if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNTOPCARDID);
                        if (DifferenceInYCoordinate < 3)
                        {
                            info.Selection.Add(new Vector2(0, DifferenceInYCoordinate));
                        }
                        else // if (DifferenceInYCoordinate >= 3)
                        {
                            info.Selection.Add(new Vector2(0, 3));
                        }
                        //DifferenceInYCoordinate < 3 ? info.Selection.Add(new Vector2(0, DifferenceInYCoordinate)) : info.Selection.Add(new Vector2(0, 3));
                        //info.Selection.Add(new Vector2(0, 3));
                        //info.Selection.Add(new Vector2(0, -3));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then use a run down card in that direction!
                    else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y < SelfPosition.y))
                    //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && (PlayerPosition.y > SelfPosition.y))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNDOWNCARDID);
                        if (DifferenceInYCoordinate < 3)
                        {
                            info.Selection.Add(new Vector2(0, -DifferenceInYCoordinate));
                        }
                        else // if (DifferenceInYCoordinate >= 3)
                        {
                            info.Selection.Add(new Vector2(0, -3));
                        }
                        //DifferenceInYCoordinate < 3 ? info.Selection.Add(new Vector2(0, -DifferenceInYCoordinate)) : info.Selection.Add(new Vector2(0, -3));
                        //info.Selection.Add(new Vector2(0, -3));
                        //info.Selection.Add(new Vector2(0, 3));
                        Debug.Log("Updated info.Selection value:" + info.Selection[0]);
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then use a run right card in that direction!
                    else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && (PlayerPosition.x > SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNRIGHTCARDID);
                        if (DifferenceInXCoordinate < 3)
                        {
                            info.Selection.Add(new Vector2(DifferenceInXCoordinate, 0));
                        }
                        else // if (DifferenceInXCoordinate >= 3)
                        {
                            info.Selection.Add(new Vector2(3, 0));
                        }
                        //DifferenceInXCoordinate < 3 ? info.Selection.Add(new Vector2(DifferenceInXCoordinate, 0)) : info.Selection.Add(new Vector2(3, 0));
                        //info.Selection.Add(new Vector2(3, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then use a run left card in that direction!
                    else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && (PlayerPosition.x < SelfPosition.x))
                    {
                        info.card = Deck.FindCardInHand(SheepData.handCard, RUNLEFTCARDID);
                        if (DifferenceInXCoordinate < 3)
                        {
                            info.Selection.Add(new Vector2(-DifferenceInXCoordinate, 0));
                        }
                        else // if (DifferenceInXCoordinate >= 3)
                        {
                            info.Selection.Add(new Vector2(-3, 0));
                        }
                        //DifferenceInXCoordinate < 3 ? info.Selection.Add(new Vector2(-DifferenceInXCoordinate, 0)) : info.Selection.Add(new Vector2(-3, 0));
                        //info.Selection.Add(new Vector2(-3, 0));
                        BattleLevelDriver.NewCardPlayed(info);
                        UpdatePiles(info.card);
                        return;
                    }
                    // If we, however, don't have the run card we need, then play the walk card instead!
                    else if (HasWalkCardAtHand)
                    {
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly above us then walk upwards.
                        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                        //if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, 1));
                            //info.Selection.Add(new Vector2(0, -1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the y coordinate or aligned in the x coordinate and the player is roughly or exactly below us then walk downwards.
                        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y < SelfPosition.y)
                        //else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || PlayerPosition.x == SelfPosition.x) && PlayerPosition.y > SelfPosition.y)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(0, -1));
                            //info.Selection.Add(new Vector2(0, 1));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the right of us then walk towards right.
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x > SelfPosition.x)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                        // If we are closer in the x coordinate or aligned in the y coordinate and the player is roughly or exactly to the left of us then walk towards left.
                        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || PlayerPosition.y == SelfPosition.y) && PlayerPosition.x < SelfPosition.x)
                        {
                            info.card = Deck.FindCardInHand(SheepData.handCard, WALKCARDID);
                            info.Selection.Add(new Vector2(-1, 0));
                            BattleLevelDriver.NewCardPlayed(info);
                            UpdatePiles(info.card);
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("Discard");
                        info.otherInfo.Add(BattleData.EnemyDataList[EnemyID].handCard[0].ID + "");
                        info.card = dicardManager;
                        BattleLevelDriver.NewCardPlayed(info);
                        return;

                    }
                }
            }
            // If we don't have any movement cards, discard one of the cards at hand to try to get a movement card.
            else
            {
                Debug.Log("Discard");

                info.otherInfo.Add(BattleData.EnemyDataList[EnemyID].handCard[0].ID + "");
                info.card = dicardManager;
                BattleLevelDriver.NewCardPlayed(info);
                return;
            }
        }
        Debug.Log("What");
    }
    private void resetFlags()
    {
        HasWalkCardAtHand = false;
        HasRunTopCardAtHand = false;
        HasRunDownCardAtHand = false;
        HasRunLeftCardAtHand = false;
        HasRunRightCardAtHand = false;
        HasHeadbuttCardAtHand = false;
        HasRushAttackCardAtHand = false;
        HasPhotosynthesisCardAtHand = false;

        PlayerIsAbove = false;
        PlayerIsBelow = false;
        PlayerIsToTheLeft = false;
        PlayerIsToTheRight = false;
    }
}
