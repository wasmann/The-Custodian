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
    int prevBehavior = -1;
    public int EnemyToAttackID;
    PathSearchAlgorithm pathSearchAlgorithm = new PathSearchAlgorithm();
    List<Vector2> pathToPlayer = new List<Vector2>();
    private struct State 
    {
        public int Aggressive;
        public bool InDanger;
        public bool PlayerInDanger;
        public bool AlignedWithEnemy;
        public bool BarkRange;
        public bool BiteRange;
        public int  Injured;//from 0 to 2
        public bool PathTooLong;
        public bool TooClose;
        public bool ObsticalInBetween;
        public bool RunToEscape;
    }

    private struct Weight
    {
        public int w_Aggressive;
        public int w_InDanger;
        public int w_RunLeft;
        public int w_RunRight;
        public int w_RunUp;
        public int w_RunDown;
        public int w_RunLeftEscape;
        public int w_RunRightEscape;
        public int w_RunUpEscape;
        public int w_RunDownEscape;
        public int w_PlayerInDanger;
        public int w_AlignedWithEnemy;
        public int w_ObsticalInBetween;
        public int w_BarkRange;
        public int w_BiteRange;
        public int w_Injured;
        public int w_PathToLong;
        public int w_TooClose;
    }
    enum RunUpBehavior
    {
        RunUpFollowingPath,
        RunUpForEscape,
        RunUpRandom
    }
    enum RunDownBehavior
    {
        RunDownFollowingPath,
        RunDownForEscape,
        RunDownRandom
    }
    enum RunLeftBehavior
    {
        RunLeftFollowingPath,
        RunLeftForEscape,
        RunLeftRandom
    }
    enum RunRightBehavior
    {
        RunRightFollowingPath,
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
        RushnBiteFollowingPath,
        RushnBiteToAttack,
        RushnBiteForFollowingPath,
    }
    private void UpdateWeight()
    {
        weight.w_Aggressive = state.Aggressive;
        weight.w_Injured = state.Injured;
        if (state.InDanger)
            weight.w_InDanger = 10;
        else
            weight.w_InDanger = 0;
        if (state.PlayerInDanger)
            weight.w_PlayerInDanger = 8;
        else
            weight.w_PlayerInDanger = -12;
        if (state.BiteRange)
            weight.w_BiteRange = 8;
        else
            weight.w_BiteRange = 0;
        if (state.BarkRange)
            weight.w_BarkRange = 5;
        else
            weight.w_BarkRange = 0;

        if (state.ObsticalInBetween)
            weight.w_ObsticalInBetween = -10;
        else
            weight.w_ObsticalInBetween = 10;
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

        if(pathToPlayer.Count != 0)
        {
            weight.w_RunRightEscape = 0;
            weight.w_RunLeftEscape = 0;
            weight.w_RunDownEscape = 0;
            weight.w_RunUpEscape = 0;
            weight.w_RunRight = 0;
            weight.w_RunLeft = 0;
            weight.w_RunDown = 0;
            weight.w_RunUp = 0;
            if (state.Injured >= 1)
            {

                weight.w_RunRightEscape = Random.Range(7, 10) * (int)(Mathf.Abs(pathToPlayer[0].y));
                weight.w_RunLeftEscape = Random.Range(7, 10) * (int)(Mathf.Abs(pathToPlayer[0].y));
                weight.w_RunUpEscape = Random.Range(7, 10) * (int)(Mathf.Abs(pathToPlayer[0].x));
                weight.w_RunDownEscape = Random.Range(7, 10) * (int)(Mathf.Abs(pathToPlayer[0].x));
            }
            else
            {
                weight.w_RunDown = 5 * (int)(-pathToPlayer[0].y);
                weight.w_RunLeft = 5 * (int)(-pathToPlayer[0].x);
                weight.w_RunRight = 5 * (int)(pathToPlayer[0].x);
                weight.w_RunUp = 5 * (int)(pathToPlayer[0].y);
            }

        }

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
            state.PlayerInDanger = true;
        else
            state.PlayerInDanger = false;

        //injured
        if (BattleData.EnemyDataList[EnemyID].currentHealth <= (BattleData.EnemyDataList[EnemyID].maxHealth / 3))
            state.Injured = (int)(1 - BattleData.EnemyDataList[EnemyID].currentHealth / (BattleData.EnemyDataList[EnemyID].maxHealth / 3)) * 2 + 1;
        else
            state.Injured = 0;

      
        float playerEnemyDistance = 999;
        //check for all enemies
        foreach (var enemy in BattleData.EnemyDataList.Keys)
        {
            if (enemy != EnemyID)
            {
                float dist = Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[enemy].position);
                if (dist < playerEnemyDistance)
                {
                    playerEnemyDistance = dist;
                    EnemyToAttackID = enemy;
                }
            }
        }

        if (prevBehavior != 0)
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
            }
            if (pathToPlayer.Count == Vector2.Distance(BattleData.EnemyDataList[EnemyToAttackID].position, BattleData.EnemyDataList[EnemyID].position))
                state.ObsticalInBetween = false;
            else
                state.ObsticalInBetween = true;
        }
        else
        {
            pathToPlayer.RemoveAt(0);
        }
        float currentDistance = Vector2.Distance(BattleData.EnemyDataList[EnemyID].position, BattleData.EnemyDataList[EnemyToAttackID].position);
        if (playerEnemyDistance < 4)
            state.PlayerInDanger = true;
        else
            state.PlayerInDanger = false;
        if (BattleData.EnemyDataList[EnemyToAttackID].position.x == BattleData.EnemyDataList[EnemyID].position.x || BattleData.EnemyDataList[EnemyToAttackID].position.y == BattleData.EnemyDataList[EnemyID].position.y)
            state.AlignedWithEnemy = true;
        else
            state.AlignedWithEnemy = true;
        if (state.AlignedWithEnemy && !state.ObsticalInBetween && currentDistance <= 4)
            state.BiteRange = true;
        else
            state.BiteRange = false;

        if (state.AlignedWithEnemy && !state.ObsticalInBetween && currentDistance <= 2)
        {
            state.BarkRange = true;
            state.TooClose = true;
        }
        else 
        {
            state.BarkRange = false;
            state.TooClose = false;
        }
    }
    private List<float>RunUpUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run up follow path
        utility = weight.w_PathToLong - weight.w_BiteRange - weight.w_BarkRange + weight.w_RunUp + weight.w_ObsticalInBetween;
        result.Add(utility);
        //run up escape
        utility = weight.w_InDanger + weight.w_AlignedWithEnemy + weight.w_Injured + weight.w_RunUpEscape;
        result.Add(utility);
        //run up random
        utility = Random.Range(0, 4) - weight.w_Aggressive - weight.w_PlayerInDanger;
        result.Add(utility);
        return result;
    } 
    private List<float>RunDownUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run down follow path
        utility = weight.w_PathToLong - weight.w_BiteRange - weight.w_BarkRange + weight.w_RunDown + weight.w_ObsticalInBetween;
        result.Add(utility);
        //run down escape
        utility = weight.w_InDanger + weight.w_AlignedWithEnemy + weight.w_Injured + weight.w_RunDownEscape;
        result.Add(utility);
        //run down random
        utility = Random.Range(0, 4) - weight.w_Aggressive - weight.w_PlayerInDanger;
        result.Add(utility);
        return result;
    } 
    private List<float>RunLeftUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run down follow path
        utility = weight.w_PathToLong - weight.w_BiteRange - weight.w_BarkRange + weight.w_RunLeft + weight.w_ObsticalInBetween;
        result.Add(utility);
        //run down escape
        utility = weight.w_InDanger + weight.w_AlignedWithEnemy + weight.w_Injured + weight.w_RunLeftEscape;
        result.Add(utility);
        //run down random
        utility = Random.Range(0, 4) - weight.w_Aggressive - weight.w_PlayerInDanger;
        result.Add(utility);
        return result;
    }  
    private List<float>RunRightUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Run down follow path
        utility = weight.w_PathToLong - weight.w_BiteRange - weight.w_BarkRange + weight.w_RunRight + weight.w_ObsticalInBetween;
        result.Add(utility);
        //run down escape
        utility = weight.w_InDanger + weight.w_AlignedWithEnemy + weight.w_Injured + weight.w_RunRightEscape;
        result.Add(utility);
        //run down random
        utility = Random.Range(0, 4) - weight.w_Aggressive - weight.w_PlayerInDanger;
        result.Add(utility);
        return result;
    }
    private List<float> BarkUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //Bark for attack
        utility = weight.w_BarkRange + weight.w_PlayerInDanger + weight.w_InDanger + weight.w_Injured;
        result.Add(utility);
        //Bark random
        utility = Random.Range(0, 4) + weight.w_Aggressive - weight.w_PlayerInDanger;
        result.Add(utility);
        return result;
    }
    private List<float> RushBiteUtility()
    {
        List<float> result = new List<float>();
        int utility;
        //to follow path
        utility = weight.w_PathToLong - weight.w_Injured + weight.w_PlayerInDanger + weight.w_BiteRange * 2 + weight.w_AlignedWithEnemy;
        result.Add(utility);
        //to attack
        utility =  weight.w_AlignedWithEnemy + weight.w_TooClose - weight.w_Injured + weight.w_PlayerInDanger + weight.w_BiteRange - weight.w_ObsticalInBetween;
        result.Add(utility);
        return result;
    }   
    private List<float> EscapeInstinctUtility()
    {
        int utility;
        List<float> result = new List<float>();
        utility = weight.w_InDanger * 3 + weight.w_TooClose + weight.w_Injured * 3;
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
        if (info.card.ID <= 6 || info.card.ID == 16)
        {
            for (int i = 0; i < info.Selection.Count; ++i)
            {
                if (!pathSearchAlgorithm.IsValid(BattleData.EnemyDataList[EnemyID].position + info.Selection[i]))
                {
                    info.Selection.RemoveRange(i, info.Selection.Count);
                    break;
                }
            }
        }
        if (info.Selection.Count == 0)
            info.Selection.Add(new Vector2(0, 0));
        if (allowToPlay)
            BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);

    }

    private void EscapeInstinctFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        if (state.AlignedWithEnemy) {
            if (BattleData.EnemyDataList[EnemyToAttackID].position.y == BattleData.EnemyDataList[EnemyID].position.y)
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
            case (int)RushnBiteBehavior.RushnBiteToAttack:
                if (state.AlignedWithEnemy)
                {
                    if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x)
                        info.Selection.Add(new Vector2(BattleData.EnemyDataList[EnemyToAttackID].position.x - BattleData.EnemyDataList[EnemyID].position.x, 0));
                    else
                        info.Selection.Add(new Vector2(0, BattleData.EnemyDataList[EnemyToAttackID].position.y - BattleData.EnemyDataList[EnemyID].position.y));
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
            case (int)RunRightBehavior.RunRightForEscape:
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

            case (int)RunLeftBehavior.RunLeftForEscape:
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

            case (int)RunDownBehavior.RunDownForEscape:
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

            case (int)RunUpBehavior.RunUpForEscape:
            case (int)RunUpBehavior.RunUpRandom:
                int steps = Random.Range(0, 3);
                info.Selection.Add(new Vector2(0, steps));
                break;
        }
    }
}
