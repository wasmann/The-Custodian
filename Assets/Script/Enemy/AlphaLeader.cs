using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaLeader : Enemy
{
    public Animator Animator;
    public override string EnemyName { get { return "AlfaLeader"; } }

    public List<Card> deck;
    public override List<Card> CardsDeck
    {
        get
        {
            return deck;//1, 1, 21, 22, 23, 23, 23, 24, 25, 25, 25
            //1.walk : Basic
            //21 Storm Strike : legendary
            //22.Reverse Electrodes : epic
            //23. Electrical Shield : common
            //24. call reinforcement : trash
            //25. falling lightning : common
        }
    }

    public override int Health { get { return 15; } }

    public override int AttackMaxRange { get { return 5; } }

    public override int HandCardNum { get { return 4; } }

    float w_Common = 0.8f;
    float w_Basic = 1;
    float w_Rare = 0.7f;
    float w_Trash = 1;
    float w_Legendary = 0.5f;
    float w_Epic = 0.3f;

    int prevBehavior = -1;
    private State state;
    Weight weight;
    List<Vector2> pathToPlayer = new List<Vector2>();
    PathSearchAlgorithm pathSearchAlgorithm = new PathSearchAlgorithm();
    private struct State 
    {
        public int Aggressive;//from 0 to 5
        public bool AlignOrNot;
        public bool DiamondAligned;
        public bool ObsticalInBetween;// wall but not some environment that can't pass
        public bool AttackShootInRange;
        public bool WalkCanAlign;
        public int Injured;//from 0 to 2
        public int AlliesInDanger;
        public bool PathTooLong;
        public bool TooClose;
        public int LegendaryCardNotUsed;
        public int EpicCardNotUsed;
    }
    enum WalkBehavior
    {
        WalkFollowingPath,
        WalkForAligning,
        RandomWalk,
        WalkBackWardsToPlayer,
    }
    enum StormBehavior
    {
        StormAttack,
        RandomStorm
    }
    enum LightningBevior
    {
        LightningAttack,
        RandomLightning
    }
    enum ShieldBehavior
    {
        ShieldToProtect,
        ShieldRandom
    }
    enum ReverseElectrodes
    {
        BlockAttack,
        BlockAttackRandom,
    }
    enum CallReinforcements
    {
        Call,
        RandomCall,
    }
    private struct Weight
    {
        public int w_Aggressive;//from 0 to 5
        public int w_AlignOrNot;
        public int w_DiamondAligned;
        public int w_ObsticalInBetween;// wall but not some environment that can't pass
        public int w_AttackShootInRange;
        public int w_WalkCanAlign;
        public int w_Injured;//from 0 to 2
        public int w_AlliesInDanger;
        public int w_PathTooLong;
        public int w_TooClose;
        public int w_LegendaryCardNotUsed;
        public int w_EpicCardNotUsed;
    }
    internal void UpdateWeight()
    {
        weight.w_Aggressive = state.Aggressive;

        if (state.AlignOrNot)
            weight.w_AlignOrNot = 10;
        else
            weight.w_AlignOrNot = -5;

        if (state.AttackShootInRange)
            weight.w_AttackShootInRange = 10;
        else
            weight.w_AttackShootInRange = -5;

        weight.w_Injured = state.Injured;

        weight.w_AlliesInDanger = state.AlliesInDanger;

        if (state.ObsticalInBetween)
            weight.w_ObsticalInBetween = -10;
        else
            weight.w_ObsticalInBetween = 10;

        if (state.DiamondAligned)
            weight.w_DiamondAligned = 8;
        else
            weight.w_DiamondAligned = -8;

        if (state.WalkCanAlign)
            weight.w_WalkCanAlign = 3;
        else
            weight.w_WalkCanAlign = 0;

        if (state.TooClose)
            weight.w_TooClose = 5;
        else
            weight.w_TooClose = 0;

        if (state.PathTooLong)
            weight.w_PathTooLong = 10;
        else
            weight.w_PathTooLong = 0;

        weight.w_LegendaryCardNotUsed = state.LegendaryCardNotUsed * 3;
        weight.w_EpicCardNotUsed = state.EpicCardNotUsed * 2;
    }
    private void UpdateState()
    {
        //obstacle in between
        if (pathToPlayer.Count == Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position))
            state.ObsticalInBetween = false;
        else
            state.ObsticalInBetween = true;
        //aggressive
        if (BattleData.playerData.currentHealth < (BattleData.playerData.maxHealth / 2))
            state.Aggressive = (int)(1 - BattleData.playerData.currentHealth / (BattleData.playerData.maxHealth / 2)) * 5 + 1;
        else
            state.Aggressive = 0;
        //diamond range
        if (Mathf.Abs(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) + Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) <= 5)
            state.DiamondAligned = true;
        else
            state.DiamondAligned = false;
        //align
        if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x || BattleData.playerData.position.y == BattleData.EnemyDataList[EnemyID].position.y)
            state.AlignOrNot = true;
        else
            state.AlignOrNot = false;
        //attack in range
        if (state.AlignOrNot && Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position) <= Random.Range(AttackMaxRange / 2, AttackMaxRange))
            state.AttackShootInRange = true;
        else
            state.AttackShootInRange = false;
        //walk can align
        if (Mathf.Abs(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) == 1 || Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) == 1)
            state.WalkCanAlign = true;
        else
            state.WalkCanAlign = false;
        //injured
        if (BattleData.EnemyDataList[EnemyID].currentHealth <= (BattleData.EnemyDataList[EnemyID].maxHealth / 3))
            state.Injured = (int)(1 - BattleData.EnemyDataList[EnemyID].currentHealth / (BattleData.EnemyDataList[EnemyID].maxHealth / 2)) * 2 + 1;
        else
            state.Injured = 0;
        //path Too Long;
        if (pathToPlayer.Count >= 8)
            state.PathTooLong = true;
        else
            state.PathTooLong = false;
        //too cose;
        if (Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position) <= 2)
            state.TooClose = true;
        else
            state.TooClose = false;
        for(int i = 1; i < BattleData.EnemyDataList.Count; i++)
        {
            if(i != EnemyID)
            {
                state.AlliesInDanger += BattleData.EnemyDataList[i].currentHealth < BattleData.EnemyDataList[i].maxHealth / 2 ? 1 : 0;
            }
        }
    }
    private List<List<float>> CalculateAllUtilities(List<Card> handcards)
    {
        List<List<float>> result = new List<List<float>>();// The outer list indicates card, the inner list indicates ultility according to baviour
        for (int i = 0; i < handcards.Count; i++)
        {
            switch (handcards[i].ID)
            {
                case 1:
                    result.Add(WalkUtility());
                    break;
                case 21:
                    result.Add(StormUtility());
                    break;
                case 22:
                    result.Add(ReverseElectrodesUtility());
                    break;
                case 23:
                    result.Add(ElectricShieldUtility());
                    break;
                case 24:
                    result.Add(CallUtility());
                    break;
                case 25:
                    result.Add(FallingLightningUtility());
                    break;
            }
        }
        return result;
    }

    private List<float> FallingLightningUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //ToAttack
        utility = weight.w_AttackShootInRange + weight.w_DiamondAligned * 3 + weight.w_Aggressive + weight.w_TooClose - weight.w_ObsticalInBetween;
        result.Add(utility);
        //Random
        utility = weight.w_AttackShootInRange + weight.w_Aggressive + weight.w_TooClose - weight.w_ObsticalInBetween + Random.Range(-3, 3);
        result.Add(utility);
        return result;
    }

    private List<float> CallUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //ToAttack
        utility = weight.w_PathTooLong + weight.w_Injured * 4;
        result.Add(utility);
        //Random
        utility = weight.w_PathTooLong + weight.w_Injured + weight.w_ObsticalInBetween + Random.Range(-3, 3);
        result.Add(utility);
        return result;
    }

    private List<float> ElectricShieldUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //ToAttack
        utility = weight.w_TooClose * 2 + weight.w_AlliesInDanger + weight.w_Injured  - weight.w_Aggressive - weight.w_PathTooLong;
        result.Add(utility);
        //Random
        utility = weight.w_TooClose + weight.w_AlliesInDanger + weight.w_Injured - Random.Range(-2, 2);
        result.Add(utility);
        return result;
    }

    private List<float> ReverseElectrodesUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //ToAttack
        utility = weight.w_EpicCardNotUsed + weight.w_TooClose * 3 + weight.w_Injured + weight.w_AlignOrNot;
        result.Add(utility);
        //Random
        utility = weight.w_EpicCardNotUsed + weight.w_TooClose + weight.w_Injured + weight.w_Aggressive + weight.w_AlignOrNot - Random.Range(-4, 1);
        result.Add(utility);
        return result;
    }

    private List<float> StormUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //ToAttack
        utility = weight.w_LegendaryCardNotUsed + weight.w_AttackShootInRange + weight.w_Injured + weight.w_AlignOrNot + weight.w_ObsticalInBetween + weight.w_TooClose;
        result.Add(utility);
        //Random
        utility = weight.w_LegendaryCardNotUsed + weight.w_AttackShootInRange + weight.w_Injured + weight.w_Aggressive + weight.w_TooClose - Random.Range(-3, 1);
        result.Add(utility);
        return result;
    }

    private List<float> WalkUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //WalkfollowingPath
        utility = weight.w_PathTooLong + weight.w_ObsticalInBetween - weight.w_Injured;
        result.Add(utility);
        //WalkForAligning
        utility = weight.w_Aggressive + weight.w_WalkCanAlign * 4 - weight.w_Injured - weight.w_AlignOrNot;
        result.Add(utility);
        //RandomWalk
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        //WalkBackWardsToPlayer
        utility = -10;// weight.w_TooClose + (5 - weight.w_Aggressive) + Random.Range(-3, 3) + weight.w_Injured;
        result.Add(utility);
        return result;
    }

    public override void EnemyChooseACardToPlay()
    {
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.animator = Animator;
        if (prevBehavior != 0 || state.AlignOrNot)
        {
            pathToPlayer.Clear();
            var path = pathSearchAlgorithm.AStarSearch(new Vector2Int((int)BattleData.EnemyDataList[EnemyID].position.x, (int)BattleData.EnemyDataList[EnemyID].position.y), new Vector2Int((int)BattleData.playerData.position.x, (int)BattleData.playerData.position.y));
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
                state.ObsticalInBetween = pathSearchAlgorithm.ObstacleInBetween;
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
        switch (info.card.ID)
        {
            case 1:
                WalkFunc(BehaviourIndex, info);
                break;
            case 21:
                StormFunc(BehaviourIndex, info);
                break;
            case 22:
                ReverseElectrodesFunc(BehaviourIndex, info);
                break;
            case 23:
                ElectricShieldFunc(BehaviourIndex, info);
                break;
            case 24:
                CallFunc(BehaviourIndex, info);
                break;
            case 25:
                FallingLightningFunc(BehaviourIndex, info);
                break;
        }
        if (info.card.rarity == Card.Rarity.legendary)
            state.LegendaryCardNotUsed++;
        else
            state.LegendaryCardNotUsed = 0;
        if (info.card.rarity == Card.Rarity.epic)
            state.EpicCardNotUsed++;
        else
            state.EpicCardNotUsed = 0;
        BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);
    }

    private void FallingLightningFunc(int behaviourIndex, Card.InfoForActivate info)
    {
    }

    private void CallFunc(int behaviourIndex, Card.InfoForActivate info)
    {
       
    }

    private void ElectricShieldFunc(int behaviourIndex, Card.InfoForActivate info)
    {
       
    }

    private void ReverseElectrodesFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        
    }

    private void StormFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        
    }

    private void WalkFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        switch (behaviourIndex)
        {
            case (int)WalkBehavior.WalkFollowingPath:
                info.Selection.Add(pathToPlayer[0]);
                break;

            case (int)WalkBehavior.WalkForAligning:
                if (BattleData.playerData.position.x != BattleData.EnemyDataList[EnemyID].position.x)
                {
                    if (BattleData.playerData.position.x > BattleData.EnemyDataList[EnemyID].position.x)
                    {
                        info.Selection.Add(new Vector2(1, 0));
                        break;
                    }
                    else
                    {
                        info.Selection.Add(new Vector2(-1, 0));
                        break;
                    }

                }
                else
                {
                    if (BattleData.playerData.position.y > BattleData.EnemyDataList[EnemyID].position.y)
                    {
                        info.Selection.Add(new Vector2(0, 1));
                        break;
                    }
                    else
                    {
                        info.Selection.Add(new Vector2(0, -1));
                        break;
                    }
                }

            case (int)WalkBehavior.RandomWalk:
                int dir = Random.Range(0, 3);
                switch (dir)
                {
                    case 0:
                        info.Selection.Add(new Vector2(0, 1));
                        break;
                    case 1:
                        info.Selection.Add(new Vector2(0, -1));
                        break;
                    case 2:
                        info.Selection.Add(new Vector2(-1, 0));
                        break;
                    case 3:
                        info.Selection.Add(new Vector2(1, 0));
                        break;
                }
                break;

            case (int)WalkBehavior.WalkBackWardsToPlayer:
                Vector2 dirVec = BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position;
                if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
                {
                    info.Selection.Add(new Vector2(0, -dirVec.y / Mathf.Abs(dirVec.y)));
                    break;
                }
                else
                {
                    info.Selection.Add(new Vector2(-dirVec.x / Mathf.Abs(dirVec.x), 0));
                    break;
                }
        }
        if (!pathSearchAlgorithm.IsValid(BattleData.EnemyDataList[EnemyID].position + info.Selection[0]))
            info.Selection[0] = new Vector2(0, 0);
    }
}
