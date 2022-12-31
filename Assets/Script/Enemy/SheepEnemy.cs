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

    bool SheepIsInRange = false;
    bool SheepIsNotInRange = false;

    bool PlayedACard = false;

    // This function resets the flags to their initial state.
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

        SheepIsInRange = false;
        SheepIsNotInRange = false;

        PlayedACard = false;
    }

    // This function discards the card with the given ID.
    private void Discard(Card.InfoForActivate info, int cardID)
    {
        for(int i = 0; i < BattleData.EnemyDataList[EnemyID].handCard.Count; i++)
        {
            if (BattleData.EnemyDataList[EnemyID].handCard[i].ID != cardID)
            {
                info.otherInfo.Add(BattleData.EnemyDataList[EnemyID].handCard[i].ID + "");
                break;
            }
        }
        info.card = dicardManager;
        BattleLevelDriver.NewCardPlayed(info);
    }

    // This function plays the chosen card.
    private void Play(Card.InfoForActivate info, int cardID, BattleData.EnemyData SheepData, 
                      Vector2 ActionTarget)
    {   
        info.card = Deck.FindCardInHand(SheepData.handCard, cardID);
        info.Selection.Add(ActionTarget);
        BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);
    }

    private bool walk (Card.InfoForActivate info, BattleData.EnemyData SheepData, 
                       Vector2 PlayerPosition, Vector2 SelfPosition, 
                       float DifferenceInXCoordinate, float DifferenceInYCoordinate)
    {
        // If we are closer in the y coordinate or aligned in the x coordinate and the player is 
        // roughly or exactly above us then walk upwards.
        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || 
            PlayerPosition.x == SelfPosition.x) && PlayerIsAbove)
        {
            Play(info, WALKCARDID, SheepData, new Vector2(0, 1));
            return true;
        }
        // If we are closer in the y coordinate or aligned in the x coordinate and the player is 
        // roughly or exactly below us then walk downwards.
        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || 
                 PlayerPosition.x == SelfPosition.x) && PlayerIsBelow)
        {
            Play(info, WALKCARDID, SheepData, new Vector2(0, -1));
            return true;
        }
        // If we are closer in the x coordinate or aligned in the y coordinate and the player is 
        // roughly or exactly to the right of us then walk towards right.
        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || 
                 PlayerPosition.y == SelfPosition.y) && PlayerIsToTheRight)
        {
            Play(info, WALKCARDID, SheepData, new Vector2(1, 0));
            return true;
        }
        // If we are closer in the x coordinate or aligned in the y coordinate and the player is 
        // roughly or exactly to the left of us then walk towards left.
        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || 
                 PlayerPosition.y == SelfPosition.y) && PlayerIsToTheLeft)
        {
            Play(info, WALKCARDID, SheepData, new Vector2(-1, 0));
            return true;
        }
        return false;
    }

    private bool run (Card.InfoForActivate info, BattleData.EnemyData SheepData, 
                      Vector2 PlayerPosition, Vector2 SelfPosition, 
                      float DifferenceInXCoordinate, float DifferenceInYCoordinate)
    {
        // If we are closer in the y coordinate or aligned in the x coordinate and the player is 
        // roughly or exactly above us then use a run top card in that direction!
        // We also play run top card even if we are closer in the x coordinate if we don't have
        // a run righ or left card.
        if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || 
            (DifferenceInYCoordinate > DifferenceInXCoordinate && 
            !(HasRunLeftCardAtHand || HasRunRightCardAtHand)) ||
            PlayerPosition.x == SelfPosition.x) && HasRunTopCardAtHand && 
            PlayerIsAbove)
        {
            Vector2 movementVector;
            if ((DifferenceInYCoordinate < 3 || 
                (DifferenceInYCoordinate > DifferenceInXCoordinate && 
                !(HasRunLeftCardAtHand || HasRunRightCardAtHand)) || 
                PlayerPosition.x == SelfPosition.x) && SheepIsInRange)
            {
                movementVector = new Vector2(0, (DifferenceInYCoordinate - 1) % 4);
            }
            else if ((DifferenceInYCoordinate < 3 || 
                     (DifferenceInYCoordinate > DifferenceInXCoordinate && 
                     !(HasRunLeftCardAtHand || HasRunRightCardAtHand)) || 
                     PlayerPosition.x == SelfPosition.x) && SheepIsNotInRange)
            {
                movementVector = new Vector2(0, (DifferenceInYCoordinate) % 4);
            }
            else // if (DifferenceInYCoordinate >= 3 && SheepIsNotInRange)
            {
                movementVector = new Vector2(0, 3);
            }
            Play(info, RUNTOPCARDID, SheepData, movementVector);
            return true;
        }
        // If we are closer in the y coordinate or aligned in the x coordinate and the player is 
        // roughly or exactly below us then use a run down card in that direction!
        // We also play run down card even if we are closer in the x coordinate if we don't have
        // a run righ or left card.
        else if ((DifferenceInYCoordinate <= DifferenceInXCoordinate || 
                 (DifferenceInYCoordinate > DifferenceInXCoordinate && 
                 !(HasRunLeftCardAtHand || HasRunRightCardAtHand)) ||
                 PlayerPosition.x == SelfPosition.x) && HasRunDownCardAtHand && 
                 PlayerIsBelow)
        {
            Vector2 movementVector;
            if ((DifferenceInYCoordinate < 3 || 
                (DifferenceInYCoordinate > DifferenceInXCoordinate && 
                !(HasRunLeftCardAtHand || HasRunRightCardAtHand)) || 
                PlayerPosition.x == SelfPosition.x) && SheepIsInRange)
            {
                movementVector = new Vector2(0, (-(DifferenceInYCoordinate - 1)) % 4);
            }
            else if ((DifferenceInYCoordinate < 3 || 
                     (DifferenceInYCoordinate > DifferenceInXCoordinate && 
                     !(HasRunLeftCardAtHand || HasRunRightCardAtHand)) || 
                     PlayerPosition.x == SelfPosition.x) && SheepIsNotInRange)
            {
                movementVector = new Vector2(0, (-DifferenceInYCoordinate) % 4);
            }
            else // if (DifferenceInYCoordinate >= 3 && SheepIsNotInRange)
            {
                movementVector = new Vector2(0, -3);
            }
            Play(info, RUNDOWNCARDID, SheepData, movementVector);
            return true;
        }
        // If we are closer in the x coordinate or aligned in the y coordinate and the player is 
        // roughly or exactly to the right of us then use a run right card in that direction!
        // We also play run right card even if we are closer in the x coordinate if we don't have
        // a run righ or left card.
        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || 
                 (DifferenceInXCoordinate > DifferenceInYCoordinate && 
                 !(HasRunTopCardAtHand || HasRunDownCardAtHand)) ||
                 PlayerPosition.y == SelfPosition.y) && HasRunRightCardAtHand && 
                 PlayerIsToTheRight)
        {
            Vector2 movementVector;
            if ((DifferenceInXCoordinate < 3 || 
                (DifferenceInXCoordinate > DifferenceInYCoordinate && 
                !(HasRunTopCardAtHand || HasRunDownCardAtHand)) || 
                PlayerPosition.y == SelfPosition.y) && SheepIsInRange)
            {
                movementVector = new Vector2((DifferenceInXCoordinate - 1) % 4, 0);
            }
            else if ((DifferenceInXCoordinate < 3 || 
                     (DifferenceInXCoordinate > DifferenceInYCoordinate && 
                     !(HasRunTopCardAtHand || HasRunDownCardAtHand)) || 
                     PlayerPosition.y == SelfPosition.y) && SheepIsNotInRange)
            {
                movementVector = new Vector2((DifferenceInXCoordinate) % 4, 0);
            }
            else // if (DifferenceInXCoordinate >= 3 && SheepIsNotInRange)
            {
                movementVector = new Vector2(3, 0);
            }
            Play(info, RUNRIGHTCARDID, SheepData, movementVector);
            return true;
        }
        // If we are closer in the x coordinate or aligned in the y coordinate and the player is 
        // roughly or exactly to the left of us then use a run left card in that direction!
        // We also play run left card even if we are closer in the x coordinate if we don't have
        // a run righ or left card.
        else if ((DifferenceInXCoordinate <= DifferenceInYCoordinate || 
                 (DifferenceInXCoordinate > DifferenceInYCoordinate && 
                 !(HasRunTopCardAtHand || HasRunDownCardAtHand)) ||
                 PlayerPosition.y == SelfPosition.y) && HasRunLeftCardAtHand && 
                 PlayerIsToTheLeft)
        {
            Vector2 movementVector;
            if ((DifferenceInXCoordinate < 3 || 
                (DifferenceInXCoordinate > DifferenceInYCoordinate && 
                !(HasRunTopCardAtHand || HasRunDownCardAtHand)) || 
                PlayerPosition.y == SelfPosition.y) && SheepIsInRange)
            {
                movementVector = new Vector2((-(DifferenceInXCoordinate - 1)) % 4, 0);
            }
            else if ((DifferenceInXCoordinate < 3 || 
                     (DifferenceInXCoordinate > DifferenceInYCoordinate && 
                     !(HasRunTopCardAtHand || HasRunDownCardAtHand)) || 
                     PlayerPosition.y == SelfPosition.y) && SheepIsNotInRange)
            {
                movementVector = new Vector2((-DifferenceInXCoordinate) % 4, 0);
            }
            else // if (DifferenceInXCoordinate >= 3 && SheepIsNotInRange)
            {
                movementVector = new Vector2(-3, 0);
            }
            Play(info, RUNLEFTCARDID, SheepData, movementVector);
            return true;
        }
        return false;
    }

    // This function follows the logic detailed below:
    // Are we in range?
        // If yes, do we have the headbutt attack card or rush attack?
            // We have both.
                // If we are 1 grid away from the player, use the headbutt card. Otherwise, 
                // use rush attack.
            // We have the headbutt attack card.
                // If we are 1 grid away, use the card.
            // If we are more than 1 grid away, discard a card other than the headbutt card 
            // and try to get a movement card or the rush attack card.
            // We have the rush attack card.
                // Use the card.
            // We have neither.
                // Discard a card and try to get the headbutt card or rush attack card.
        // If no, try to move into range
            // Do we have a movement card?
                // If yes, play it!
                // If no, discard one of the cards at hand to get a movement card.

    // TO DO: In the next updates, make the direction random and consider factors such as 
    // obstacles etc.
    public override void EnemyChooseACardToPlay()
    {
        resetFlags();

        BattleData.EnemyData SheepData = BattleData.EnemyDataList[EnemyID];
        Vector2 PlayerPosition = BattleData.playerData.position;
        Vector2 SelfPosition = SheepData.position;

        // Calculate these values once in order to optimize the code and make it more readable.
        float DifferenceInXCoordinate = Math.Abs(PlayerPosition.x - SelfPosition.x);
        float DifferenceInYCoordinate = Math.Abs(PlayerPosition.y - SelfPosition.y);

        Debug.Log("Player Position: " + PlayerPosition);
        Debug.Log("Sheep Position: " + SelfPosition);

        // Information required to play the cards will be stored in this struct.
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.otherInfo = new List<string>();
        info.Selection = new List<Vector2>();

        // Check which cards we have and set the correct flags
        for (var i = 0; i < SheepData.handCard.Count; ++i)
        {
            if (SheepData.handCard[i].ID == WALKCARDID) {
                HasWalkCardAtHand = true;
                Debug.Log("Sheep has the walk card");
            }
            else if (SheepData.handCard[i].ID == RUNTOPCARDID) { 
                HasRunTopCardAtHand = true;
                Debug.Log("Sheep has the run top card");
            }
            else if (SheepData.handCard[i].ID == RUNDOWNCARDID)
            {
                HasRunDownCardAtHand = true;
                Debug.Log("Sheep has the run down card");
            }
            else if (SheepData.handCard[i].ID == RUNLEFTCARDID)
            {
                HasRunLeftCardAtHand = true;
                Debug.Log("Sheep has the run left card");
            }
            else if (SheepData.handCard[i].ID == RUNRIGHTCARDID)
            {
                HasRunRightCardAtHand = true;
                Debug.Log("Sheep has the run right card");
            }
            else if (SheepData.handCard[i].ID == HEADBUTTCARDID)
            {
                HasHeadbuttCardAtHand = true;
                Debug.Log("Sheep has the headbutt card");
            }
            else if (SheepData.handCard[i].ID == RUSHATTACKCARDID)
            {
                HasRushAttackCardAtHand = true;
                Debug.Log("Sheep has the rush attack card");
            }
        }

        // Is the player roughly above us?
        if (PlayerPosition.y > SelfPosition.y)
        {
            PlayerIsAbove = true;
            Debug.Log("Player is roughly above sheep.");
        }
        // Is the player rougly below us?
        else if (PlayerPosition.y < SelfPosition.y)
        {
            PlayerIsBelow = true;
            Debug.Log("Player is roughly below sheep.");
        }

        // Is the player roughly to the right of us?
        if (PlayerPosition.x > SelfPosition.x)
        {
            PlayerIsToTheRight = true;
            Debug.Log("Player is roughly to the right of sheep.");
        }
        // Is the player rougly to the left of us?
        else if (PlayerPosition.x < SelfPosition.x)
        {
            PlayerIsToTheLeft = true;
            Debug.Log("Player is roughly to the left of sheep.");
        }

        // If we are on top of each other, then sheep won't know what to do!
        if (PlayerPosition.x == SelfPosition.x && PlayerPosition.y == SelfPosition.y) {
            Debug.Log("Sheep doesn't know what to do!");
            // We discard the first card on our hand!
            Discard(info, SheepData.handCard[0].ID);
            return;
        }

        // Are we in range?
        // If the enemy is directly, at most, 3 grid units to the right, left, above or below the 
        // enemy, then the enemy is within range. We check if we are aligned in our x or y axis 
        // to make sure we are not at a diagonal position to the player.
        if ((PlayerPosition.x == SelfPosition.x && DifferenceInYCoordinate <= 3) ||
            (PlayerPosition.y == SelfPosition.y && DifferenceInXCoordinate <= 3))
        {
            Debug.Log("Sheep is in range!");
            SheepIsInRange = true;
            if (HasHeadbuttCardAtHand && HasRushAttackCardAtHand)
            {
                // If we are 1 grid away from the player, use the headbutt card.
                if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                {
                    Play(info, HEADBUTTCARDID, SheepData, PlayerPosition - SelfPosition);
                    return;
                }
                // Otherwise, use rush attack as it does more damage.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {
                    Play(info, RUSHATTACKCARDID, SheepData, PlayerPosition - SelfPosition);
                    return;
                }
            }
            else if (HasRushAttackCardAtHand)
            {
                Play(info, RUSHATTACKCARDID, SheepData, PlayerPosition - SelfPosition);
                return;
            }
            else if (HasHeadbuttCardAtHand)
            {
                // If we are 1 grid away, use the card.
                if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                {
                    Play(info, HEADBUTTCARDID, SheepData, PlayerPosition - SelfPosition);
                    return;
                }
                // If we are more than 1 grid away but we have a movement card, then try to use it!
                else if (HasWalkCardAtHand || HasRunTopCardAtHand ||
                         HasRunDownCardAtHand || HasRunLeftCardAtHand || HasRunRightCardAtHand)
                {
                    // If we have a run card, try to use it!
                    if ((HasRunTopCardAtHand && PlayerIsAbove) || 
                        (HasRunDownCardAtHand && PlayerIsBelow) ||
                        (HasRunLeftCardAtHand && PlayerIsToTheLeft) ||
                        (HasRunRightCardAtHand && PlayerIsToTheRight))
                    { 
                        PlayedACard = run(info, SheepData, PlayerPosition, SelfPosition, 
                                          DifferenceInXCoordinate, DifferenceInYCoordinate);
                        if (PlayedACard) { return; }
                    }
                    // If we, however, don't have the run card we need, then play the walk card 
                    // instead!
                    else if (HasWalkCardAtHand)
                    {
                        PlayedACard = walk(info, SheepData, PlayerPosition, SelfPosition, 
                                           DifferenceInXCoordinate, DifferenceInYCoordinate);
                        if (PlayedACard) { return; }
                    }
                    // If we are more than 1 grid away, discard a card other than the headbutt 
                    // card and try to get a movement card or the rush attack card.
                    Discard(info, HEADBUTTCARDID);
                    return;   
                }
                // If we are more than 1 grid away, discard a card other than the headbutt card 
                // and try to get a movement card or the rush attack card.
                else // if (DifferenceInXCoordinate != 1 || DifferenceInYCoordinate != 1)
                {
                    Discard(info, HEADBUTTCARDID);
                    return;
                }
            }
            // If we don't have any attack card but we have movement cards and we can still get 
            // closer to the player, then try to!
            else if (!(DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1))
            {
                // If we have a run card, try to use it!
                if ((HasRunTopCardAtHand && PlayerIsAbove) || 
                    (HasRunDownCardAtHand && PlayerIsBelow) ||
                    (HasRunLeftCardAtHand && PlayerIsToTheLeft) ||
                    (HasRunRightCardAtHand && PlayerIsToTheRight))
                { 
                    PlayedACard = run(info, SheepData, PlayerPosition, SelfPosition, 
                                      DifferenceInXCoordinate, DifferenceInYCoordinate);
                    if (PlayedACard) { return; }
                }
                // If we, however, don't have the run card we need, 
                // then play the walk card instead!
                else
                {
                    PlayedACard = walk(info, SheepData, PlayerPosition, SelfPosition, 
                                       DifferenceInXCoordinate, DifferenceInYCoordinate);
                    if (PlayedACard) { return; }
                }
                Debug.Log("Discarding to try to get an attack card or movement card we can use!");
                // If we have neither attack card or a useful movement card, then discard a card 
                // and try to get the headbutt card or rush attack card!
                // We discard the first card on our hand!
                Discard(info, SheepData.handCard[0].ID);
                return;
            }
            else
            {
                Debug.Log("Discarding to try to get the headbutt card or rush attack card!");
                // If we have neither attack card, then discard a card and try to get the headbutt 
                // card or rush attack card!
                // We discard the first card on our hand!
                Discard(info, SheepData.handCard[0].ID);
                return;
            }
        }
        // If we are not in range, try to move into range!
        else
        {
            Debug.Log("Sheep is not in range!");
            SheepIsNotInRange = true;
            // Do we have a movement card?
            if (HasWalkCardAtHand || HasRunTopCardAtHand || HasRunDownCardAtHand || 
                HasRunLeftCardAtHand || HasRunRightCardAtHand)
            {
                // Do we have both the walk and run cards?
                if (HasWalkCardAtHand && (HasRunTopCardAtHand || HasRunDownCardAtHand || 
                    HasRunLeftCardAtHand || HasRunRightCardAtHand))
                {
                    // If we have both types of cards and the distance to get into range is 
                    // greater then 1 grid, then use a run card.
                    if ((DifferenceInXCoordinate >= 5 || DifferenceInYCoordinate >= 5) && 
                        !(DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1))
                    {
                        // If we have a run card, try to use it!
                        if ((HasRunTopCardAtHand && PlayerIsAbove) || 
                            (HasRunDownCardAtHand && PlayerIsBelow) ||
                            (HasRunLeftCardAtHand && PlayerIsToTheLeft) ||
                            (HasRunRightCardAtHand && PlayerIsToTheRight))
                        { 
                            PlayedACard = run(info, SheepData, PlayerPosition, SelfPosition, 
                                              DifferenceInXCoordinate, DifferenceInYCoordinate);
                            if (PlayedACard) { return; }
                        }
                        // If we, however, don't have the run card we need, then play the walk 
                        // card instead!
                        else
                        {
                            PlayedACard = walk(info, SheepData, PlayerPosition, SelfPosition, 
                                               DifferenceInXCoordinate, DifferenceInYCoordinate);
                            if (PlayedACard) { return; }
                        }
                    }
                    // If we have both cards and the distance to get into range is equal to 1 
                    // grid, then use the walk card.
                    else if (DifferenceInXCoordinate == 1 || DifferenceInYCoordinate == 1)
                    {
                        PlayedACard = walk(info, SheepData, PlayerPosition, SelfPosition, 
                                           DifferenceInXCoordinate, DifferenceInYCoordinate);
                        if (PlayedACard) { return; }
                    }
                    // If we have both cards and we are not in one of the two special conditions 
                    // described above, then first try to play a run card to cover as much 
                    // distance as possible. If that is not possible, then play a walk card!
                    else
                    {
                        // If we have a run card, try to use it!
                        if ((HasRunTopCardAtHand && PlayerIsAbove) || 
                            (HasRunDownCardAtHand && PlayerIsBelow) ||
                            (HasRunLeftCardAtHand && PlayerIsToTheLeft) ||
                            (HasRunRightCardAtHand && PlayerIsToTheRight))
                        { 
                            PlayedACard = run(info, SheepData, PlayerPosition, SelfPosition, 
                                              DifferenceInXCoordinate, DifferenceInYCoordinate);
                        if (PlayedACard) { return; }
                        }
                        // If we, however, don't have the run card we need, then play the walk 
                        // card instead!
                        else
                        {
                            PlayedACard = walk(info, SheepData, PlayerPosition, SelfPosition, 
                                               DifferenceInXCoordinate, DifferenceInYCoordinate);
                            if (PlayedACard) { return; }
                        }
                    }
                }
                // Otherwise, play whatever movement card we have at hand!
                else
                {
                    // If we have a run card, try to use it!
                    if ((HasRunTopCardAtHand && PlayerIsAbove) || 
                        (HasRunDownCardAtHand && PlayerIsBelow) ||
                        (HasRunLeftCardAtHand && PlayerIsToTheLeft) ||
                        (HasRunRightCardAtHand && PlayerIsToTheRight))
                    { 
                        PlayedACard = run(info, SheepData, PlayerPosition, SelfPosition, 
                                          DifferenceInXCoordinate, DifferenceInYCoordinate);
                        if (PlayedACard) { return; }
                    }
                    // If we, however, don't have the run card we need, then play the walk card 
                    // instead!
                    else if (HasWalkCardAtHand)
                    {
                        PlayedACard = walk(info, SheepData, PlayerPosition, SelfPosition, 
                                           DifferenceInXCoordinate, DifferenceInYCoordinate);
                        if (PlayedACard) { return; }
                    }
                    // There may be a case where although we have a movement card at hand, we 
                    // shouldn't play it. Then, we simply discard to try to get another movement
                    // card.
                    Debug.Log("Discarding to try to get another movement card we can play");
                    // We discard the first card on our hand!
                    Discard(info, SheepData.handCard[0].ID);
                    return;
                }
            }
            // If we don't have any movement cards, discard one of the cards at hand to try to get 
            // a movement card.
            else
            {
                Debug.Log("Discarding to try to get a movement card!");
                // We discard the first card on our hand!
                Discard(info, SheepData.handCard[0].ID);
                return;
            }
        }
        // Getting this might be caused by a bug or in case of player and sheep getting on top of
        // each other, it is the defined behavior.
        Debug.Log("No action was taken!");
        return;
    }
}
