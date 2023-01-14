using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Hound : Enemy
{
    public override string EnemyName { get { return "Hound"; } }

    public Animator Animator;

    public List<Card> deck;
    public override List<Card> CardsDeck 
    {
        get
        {
            return deck; //3,4,5,6,16,16,17,17,18
        }
    }

    public override int Health { get { return 6; } }
    public override int AttackMaxRange { get { return 4; } }
    public override int HandCardNum { get { return 3; } }

    private State state;
    Weight weight;
    public int CloseEnemyID;
    int prevBehavior = -1;
    public int EnemyToAttackID;
    List<Vector2> pathToPlayer = new List<Vector2>();
    private struct State
    {
        public int Aggressive;
        public bool inDanger;
        public bool playerInDanger;
        public bool AlignedWithPlayer;
        public bool AlignedWithEnemy;
        public bool RunUp;
        public bool RunDown;
        public bool RunLeft;
        public bool RunRight;
        public bool BarkRange;
        public bool BiteRange;
        public int  Injured;//from 0 to 2
        public bool PathTooLong;
        public bool TooClose;
        public bool ObsticalInBetween;
    }

    private struct Weight
    {
        public int w_Aggressive;
        public int w_inDanger;
        public int w_playerInDanger;
        public int w_AlignedWithPlayer;
        public int w_AlignedWithEnemy;
        public int w_ObsticalInBetween;
        public int w_RunUp;
        public int w_RunDown;
        public int w_RunLeft;
        public int w_RunRight;
        public int w_BarkRange;
        public int w_BiteRange;
        public int w_Injured;
        public int w_PathToLong;
        public int w_TooClose;
    }
    enum RunUpBehavior
    {
        RunUpFollowingPath,
        RunUpForAligning, //for biting
        RunUpForEscape,
        RunUpRandom
    }
    enum RunDownBehavior
    {
        RunDownFollowingPath,
        RunDownForAligning,
        RunDownForEscape,
        RunDownRandom
    }
    enum RunLeftBehavior
    {
        RunLeftFollowingPath,
        RunLeftForAligning,
        RunLeftForEscape,
        RunLeftRandom
    }
    enum RunRightBehavior
    {
        RunRightFollowingPath,
        RunRightForAligning,
        RunRightForEscape,
        RunRightRandom
    }
    enum BarkBehavior
    {
        BarkAtEnemy,
        RandomBark,
    }
    enum RushnBiteBehavior
    {
        RushnBiteToAttack,
        RushnBiteForAligning,
        RushnBiteForFollowingPath,
    }
    private void UpdateWeight()
    {
        weight.w_Aggressive = state.Aggressive;
        if (state.inDanger)
            weight.w_inDanger = 10;
        else
            weight.w_inDanger = 0;
        if (state.playerInDanger)
            weight.w_playerInDanger = 5;
        else
            weight.w_playerInDanger = 0;
        if (state.BiteRange)
            weight.w_BiteRange = 8;
        else
            weight.w_BiteRange = 0;
        if (state.RunDown)
            weight.w_RunDown = 7;
        else
            weight.w_RunDown = 0;
        if(state.RunLeft)
            weight.w_RunLeft = 7;
        else
            weight.w_RunLeft= 0;
        if (state.RunRight)
            weight.w_RunRight = 7;
        else
            weight.w_RunLeft = 0;
        if (state.RunUp)
            weight.w_RunUp = 7;
        else
            weight.w_RunUp = 0;
        if (state.BarkRange)
            weight.w_BarkRange = 5;
        else
            weight.w_BarkRange = 0;

        if (state.ObsticalInBetween)
            weight.w_ObsticalInBetween = 10;
        else
            weight.w_ObsticalInBetween = 0;

        if (state.TooClose)
            weight.w_TooClose = 5;
        else
            weight.w_TooClose = 0;

        if (state.PathTooLong)
            weight.w_PathToLong = 10;
        else
            weight.w_PathToLong = 0;
        if (state.AlignedWithEnemy)
            weight.w_AlignedWithEnemy = 8;
        else
            weight.w_AlignedWithEnemy = 0;
        if (state.AlignedWithPlayer)
            weight.w_AlignedWithPlayer = 6;
        else
            weight.w_AlignedWithPlayer = 0;

    }
    private void UpdateState()
    {
        //aggresive
        if (BattleData.playerData.currentHealth < (BattleData.playerData.maxHealth / 2))
            state.Aggressive = (int)(1 - BattleData.playerData.currentHealth / (BattleData.playerData.maxHealth / 2)) * 5 + 1;
        else
            state.Aggressive = 0;
        //player is in danger
        if (BattleData.playerData.buff.Fear == true)
            state.playerInDanger = true;
        else
            state.playerInDanger = false;
        //aligned with player
        if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x || BattleData.playerData.position.y == BattleData.EnemyDataList[EnemyID].position.y)
            state.AlignedWithPlayer = true;
        else
            state.AlignedWithPlayer = false;
        //align with player
        if (Mathf.Abs(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) <= 3)
        {
            if (BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x > 0)
                state.RunLeft = true;
            else
                state.RunRight = true;
        }
        if (Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) <= 3)
        {
            if (BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y > 0)
                state.RunUp = true;
            else
                state.RunDown = true;
        }
        //injured
        if (BattleData.EnemyDataList[EnemyID].currentHealth <= (BattleData.EnemyDataList[EnemyID].maxHealth / 3))
            state.Injured = (int)(1 - BattleData.EnemyDataList[EnemyID].currentHealth / (BattleData.EnemyDataList[EnemyID].maxHealth / 3)) * 2 + 1;
        else
            state.Injured = 0;

        //check for all enemies
        foreach (var enemy in BattleData.EnemyDataList.Keys)
        {
            if (enemy != EnemyID)
            {
                //if hound is in danger
                if (Vector2.Distance(BattleData.EnemyDataList[EnemyID].position, BattleData.EnemyDataList[enemy].position) <= 2)
                {
                    state.inDanger = true;
                    state.BarkRange = true;
                    state.BiteRange = true;
                    CloseEnemyID = enemy;
                }
                else
                {
                    state.inDanger = false;
                    state.BarkRange = false;
                }
                
                if (Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[enemy].position) <= Random.Range(2, BattleData.EnemyDataList[enemy].enemy.AttackMaxRange))
                {
                    state.playerInDanger = true;
                    state.BarkRange = false;
                    CloseEnemyID = enemy;
                }
                else
                {
                    state.BarkRange = false;
                    state.playerInDanger = false;
                }

                if (BattleData.EnemyDataList[EnemyID].position.x == BattleData.EnemyDataList[enemy].position.x || BattleData.EnemyDataList[EnemyID].position.y == BattleData.EnemyDataList[enemy].position.y)
                {
                    state.AlignedWithEnemy = true;
                    CloseEnemyID = enemy;
                }
                else
                    state.AlignedWithEnemy = false;
                //if custodin is in danger
                if(Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[enemy].position) <= Random.Range(2, BattleData.EnemyDataList[enemy].enemy.AttackMaxRange))
                {
                    state.playerInDanger = true;
                }

                //Align with Enemy
                if (Mathf.Abs(BattleData.EnemyDataList[enemy].position.x - BattleData.EnemyDataList[EnemyID].position.x) <= 3)
                {
                    if(BattleData.EnemyDataList[enemy].position.x - BattleData.EnemyDataList[EnemyID].position.x > 0)
                        state.RunLeft = true;
                    else
                        state.RunRight = true;
                }
                if (Mathf.Abs(BattleData.EnemyDataList[enemy].position.y - BattleData.EnemyDataList[EnemyID].position.y) <= 3)
                {
                    if (BattleData.EnemyDataList[enemy].position.y - BattleData.EnemyDataList[EnemyID].position.y > 0)
                        state.RunUp = true;
                    else
                        state.RunDown = true;
                }
            }
        }
    }
    private List<float>RunUpUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run up follow path
        utility = weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured;
        result.Add(utility);
        //run up align
        utility = weight.w_Aggressive + weight.w_RunUp + weight.w_AlignedWithEnemy + weight.w_AlignedWithPlayer + weight.w_playerInDanger - weight.w_Injured - weight.w_BiteRange;
        result.Add(utility);
        //run up escape
        utility = weight.w_inDanger + weight.w_RunUp + weight.w_AlignedWithEnemy + weight.w_Injured - weight.w_BiteRange - weight.w_BarkRange;
        result.Add(utility);
        //run up random
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        return result;
    } 
    private List<float>RunDownUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run down follow path
        utility = weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured;
        result.Add(utility);
        //run down align
        utility = weight.w_Aggressive + weight.w_RunDown + weight.w_AlignedWithEnemy + weight.w_AlignedWithPlayer + weight.w_playerInDanger - weight.w_Injured - weight.w_BiteRange;
        result.Add(utility);
        //run down escape
        utility = weight.w_inDanger + weight.w_RunDown + weight.w_AlignedWithEnemy + weight.w_Injured - weight.w_BiteRange - weight.w_BarkRange;
        result.Add(utility);
        //run down random
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        return result;
    } 
    private List<float>RunLeftUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run down follow path
        utility = weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured;
        result.Add(utility);
        //run down align
        utility = weight.w_Aggressive + weight.w_RunLeft + weight.w_AlignedWithEnemy + weight.w_AlignedWithPlayer + weight.w_playerInDanger - weight.w_Injured - weight.w_BiteRange;
        result.Add(utility);
        //run down escape
        utility = weight.w_inDanger + weight.w_RunLeft + weight.w_AlignedWithEnemy + weight.w_Injured - weight.w_BiteRange - weight.w_BarkRange;
        result.Add(utility);
        //run down random
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        return result;
    }  
    private List<float>RunRightUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run down follow path
        utility = weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured;
        result.Add(utility);
        //run down align
        utility = weight.w_Aggressive + weight.w_RunRight + weight.w_AlignedWithEnemy + weight.w_AlignedWithPlayer + weight.w_playerInDanger - weight.w_Injured - weight.w_BiteRange;
        result.Add(utility);
        //run down escape
        utility = weight.w_inDanger + weight.w_RunRight + weight.w_AlignedWithEnemy + weight.w_Injured - weight.w_BiteRange - weight.w_BarkRange;
        result.Add(utility);
        //run down random
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        return result;
    }
    private List<float> BarkUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Bark for attack
        utility = weight.w_BarkRange + weight.w_playerInDanger + weight.w_inDanger + weight.w_Injured;
        result.Add(utility);
        //Bark random
        utility = Random.Range(0, 4) + weight.w_Aggressive;
        result.Add(utility);
        return result;
    }
    private List<float> RushBiteUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //to attack
        utility = weight.w_BiteRange + weight.w_playerInDanger + weight.w_inDanger + weight.w_Injured + weight.w_AlignedWithEnemy + weight.w_Aggressive - weight.w_ObsticalInBetween;
        result.Add(utility);
        //to align
        utility = weight.w_PathToLong - weight.w_AlignedWithEnemy - weight.w_Injured + weight.w_playerInDanger;
        result.Add(utility);
        //to follow path
        utility = weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured + weight.w_playerInDanger;
        result.Add(utility);
        return result;
    }   
    private List<float> EscapeInstinctUtility()
    {
        int utility;
        List<float> result = new List<float>();
        utility = weight.w_inDanger + weight.w_TooClose + weight.w_Injured - (weight.w_RunDown + weight.w_RunLeft + weight.w_RunRight + weight.w_RunUp) / 4;
        result.Add(utility);
        return result;
    }

    private List<List<float>> CalculateAllUtilities(List<Card> handcards)
    {
        List<List<float>> result = new List<List<float>>();
        for (int i = 0; i < handcards.Count; i++)
        {
            switch (handcards[i].ID)
            {
                case 2:
                    result.Add(RunUpUtility());
                    break;
                case 3:
                    result.Add(RunDownUtility());
                    break;
                case 4:
                    result.Add(RunLeftUtility());
                    break;
                case 5:
                    result.Add(RunRightUtility());
                    break;
                case 15:
                    result.Add(BarkUtility());
                    break;
                case 16:
                    result.Add(RushBiteUtility());
                    break;
                case 17:
                    result.Add(EscapeInstinctUtility());
                    break;
            }
        }
        return result;
    }
    public override void EnemyChooseACardToPlay()
    {
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.animator = Animator;
        if (prevBehavior != 0 || state.AlignedWithPlayer || state.AlignedWithEnemy)
        {
            if (pathToPlayer.Count != 0) pathToPlayer.Clear();
            var a = new PathSearchAlgorithm();
            var path = a.AStarSearch(new Vector2Int((int)BattleData.EnemyDataList[EnemyID].position.x, (int)BattleData.EnemyDataList[EnemyID].position.y), new Vector2Int((int)BattleData.playerData.position.x, (int)BattleData.playerData.position.y));
            if (path.Count == 0)
            {
                state.ObsticalInBetween = false;
            }
            else
            {

                for (int i = 0; i < path.Count - 1; i++)
                {
                    pathToPlayer.Add(path[i + 1] - path[i]);
                }
                state.ObsticalInBetween = a.ObstacleInBetween;
            }
        }
        else
        {
            pathToPlayer.RemoveAt(0);
        }
        UpdateState();
        UpdateWeight();
        List<List<float>> BehaviourUtility;
        BehaviourUtility = CalculateAllUtilities(BattleData.EnemyDataList[EnemyID].handCard);

        int BehaviourIndex = 0;
        int BehaviourCardID = 0;
        float maxUtility = -9999;
        for (int i = 0; i < BehaviourUtility.Count; i++)
        {
            for (int j = 0; j < BehaviourUtility[i].Count; j++)
            {
                if (BehaviourUtility[i][j] > maxUtility)
                {
                    maxUtility = BehaviourUtility[i][j];
                    BehaviourIndex = j;
                    BehaviourCardID = i;
                }
            }
        }

        info.card = BattleData.EnemyDataList[EnemyID].handCard[BehaviourCardID];
        info.Selection = new List<Vector2>();
        bool allowToPlay = true;
        switch (info.card.ID)
        {
            case 2:
                RunUpFunc(BehaviourIndex, info);
                break;
            case 3:
                RunDownFunc(BehaviourIndex, info);
                break;
            case 4:
                RunLeftFunc(BehaviourIndex, info);
                break;
            case 5:
                RunRightFunc(BehaviourIndex, info);
                break;
            case 15:
                if (BehaviourIndex != (int)BarkBehavior.BarkAtEnemy)
                    allowToPlay = Random.Range(0, 1) == 1;              
                break;
            case 16:
                RushnBiteFunc(BehaviourIndex, info);
                break;
            case 17:
                EscapeInstinctFunc(BehaviourIndex, info);
                break;
        }
        if(allowToPlay)
            BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);

    }

    private void EscapeInstinctFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        if (state.AlignedWithEnemy) {
            if (BattleData.EnemyDataList[CloseEnemyID].position.y == BattleData.EnemyDataList[EnemyID].position.y)
                info.Selection.Add(new Vector2((int)Random.Range(0, 1) == 1 ? -1 : 1, 0));
             else
                info.Selection.Add(new Vector2(0, (int)Random.Range(0, 1) == 1 ? -1 : 1));
        }
    }

    private void RushnBiteFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new(0, 0);
        switch (behaviourIndex)
        {
            case (int)RushnBiteBehavior.RushnBiteForFollowingPath:
                for (int i = 1; i < 3; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else
                        break;
                }
                info.Selection.Add(result);
                break;
            case (int)RushnBiteBehavior.RushnBiteForAligning:
                float disx = Mathf.Abs(BattleData.EnemyDataList[CloseEnemyID].position.x - BattleData.EnemyDataList[EnemyID].position.x);
                float disy = Mathf.Abs(BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disx < disy)
                {
                    if (disx <= 5)
                        info.Selection.Add(new Vector2(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x, 0));
                    else
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.EnemyDataList[CloseEnemyID].position.x - BattleData.EnemyDataList[EnemyID].position.x) * 4, 0));
                }
                else
                {
                    if (disy <= 5)
                        info.Selection.Add(new Vector2(0, BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y));
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y) * 4));

                }
                break;
            case (int)RushnBiteBehavior.RushnBiteToAttack:
                if (state.AlignedWithEnemy)
                {
                    if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x)
                        info.Selection.Add(new Vector2(BattleData.EnemyDataList[CloseEnemyID].position.x - BattleData.EnemyDataList[EnemyID].position.x, 0));
                    else
                        info.Selection.Add(new Vector2(0, BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y));
                }
                else
                {

                    if (System.Convert.ToBoolean(Random.Range(0, 1)))
                    {
                        info.Selection.Add(new Vector2(Random.Range(-4, 4), 0));
                    }
                    else
                    {
                        info.Selection.Add(new Vector2(0, Random.Range(-4, 4)));
                    }
                }
                break;
        }
    }

    private void RunRightFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new(0, 0);
        switch (behaviourIndex)
        {
            case (int)RunRightBehavior.RunRightFollowingPath:
                for (int i = 0; i < pathToPlayer.Count; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else if (result.magnitude > 3) break;
                    else
                        break;
                }
                info.Selection.Add(result);
                break;

            case (int)RunRightBehavior.RunRightForAligning:
                float disty = Mathf.Abs(BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disty > 0)
                {
                    if (disty < 3)
                        info.Selection.Add(new Vector2(BattleData.EnemyDataList[CloseEnemyID].position.x - BattleData.EnemyDataList[EnemyID].position.x, 0));
                    else
                        info.Selection.Add(new Vector2(3, 0));
                }
                else
                    break;
                break;

            case (int)RunRightBehavior.RunRightRandom:
                int steps = Random.Range(3, 0);
                info.Selection.Add(new Vector2(steps, 0));
                break;
        }
    }

    private void RunLeftFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new Vector2(0, 0);
        switch (behaviourIndex)
        {
            case (int)RunLeftBehavior.RunLeftFollowingPath:
                for (int i = 0; i < pathToPlayer.Count; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else if (result.magnitude > 3) break;
                    else
                        break;
                }
                info.Selection.Add(result);
                break;

            case (int)RunLeftBehavior.RunLeftForAligning:
                float disty = Mathf.Abs(BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disty > 0)
                {
                    if (disty < 3)
                        info.Selection.Add(new Vector2(BattleData.EnemyDataList[CloseEnemyID].position.x - BattleData.EnemyDataList[EnemyID].position.x, 0));
                    else
                        info.Selection.Add(new Vector2(-3, 0));
                }
                else
                    break;
                break;

            case (int)RunLeftBehavior.RunLeftRandom:
                int steps = Random.Range(3, 0);
                info.Selection.Add(new Vector2(-steps, 0));
                break;
        }
    }

    private void RunDownFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new Vector2(0, 0);
        switch (behaviourIndex)
        {
            case (int)RunDownBehavior.RunDownFollowingPath:
                for (int i = 0; i < pathToPlayer.Count; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else if (result.magnitude > 3) break;
                    else
                        break;
                }
                info.Selection.Add(result);
                break;

            case (int)RunDownBehavior.RunDownForAligning:
                float disty = Mathf.Abs(BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disty > 0)
                {
                    if (disty < 3)
                        info.Selection.Add(new Vector2(0, BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y));
                    else
                        info.Selection.Add(new Vector2(0, -3));
                }
                else
                    break;
                break;

            case (int)RunDownBehavior.RunDownRandom:
                int steps = Random.Range(0, 3);
                info.Selection.Add(new Vector2(0, -steps));
                break;
        }
    }

    private void RunUpFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new Vector2(0, 0);
        switch (behaviourIndex)
        {
            case (int)RunUpBehavior.RunUpFollowingPath:
                for (int i = 0; i < pathToPlayer.Count; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else if (result.magnitude > 3) break;
                    else
                        break;
                }
                info.Selection.Add(result);
                break;

            case (int)RunUpBehavior.RunUpForAligning:
                float disty = Mathf.Abs(BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disty > 0)
                {
                    if (disty < 3)
                        info.Selection.Add(new Vector2(0, BattleData.EnemyDataList[CloseEnemyID].position.y - BattleData.EnemyDataList[EnemyID].position.y));
                    else
                        info.Selection.Add(new Vector2(0, 3));
                }
                else
                    break;
                break;

            case (int)RunUpBehavior.RunUpRandom:
                int steps = Random.Range(0, 3);
                info.Selection.Add(new Vector2(0, steps));
                break;
        }
    }
}
