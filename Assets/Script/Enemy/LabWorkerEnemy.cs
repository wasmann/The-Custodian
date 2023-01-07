using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LabWorkerEnemy : Enemy
{
    public override string EnemyName { get { return "LabWorker"; } }

    public Animator Animator;

    public List<Card> deck;
    public override List<Card> CardsDeck
    {
        get
        {
            return deck;//1,1,1, 11, 11, 12, 13, 13, 13, 14
        }
    }

    public override int Health { get { return 10; } }

    public override int AttackMaxRange { get { return 5; } }

    public override int HandCardNum { get { return 3; } }

    float w_Common = 0.8f;
    float w_Basic = 1;
    float w_Rare = 0.7f;
    float w_Legendary = 0.5f;

    private State state;
    Weight weight;
    List<Vector2> pathToPlayer;
    private struct State
    {
        public int Aggressive;
        public bool Aligned;
        public int Injured;
        public bool ObsticalInBetween;
        public bool CryoliquidShootInRange;
        public bool CryoliquidAttackInRange;
        public bool WalkCanAlign;
        public bool PathTooLong;
        public bool TooClose;
        public int RareCardNotUsed;
        public int LegendaryCardNotUsed;

    }
    enum WalkBehavior
    {
        WalkFollowingPath,
        WalkForAligning,
        RandomWalk,
        WalkBackWardsToPlayer,
    }
    enum JumpBehavior
    {
        JumpFollowingPath,
        JumpForAligning,
        RandomJump,
        JumpBackWardsToPLayer
    }
    enum DoubleBehavior
    {
        JumpFollowingPath,
        JumpForAligning,
        RandomJump,
        JumpBackWardsToPLayer
    }
    enum CryoliquidShootingBehavior
    {
        ShootAtEnemy,
        ShootRandomly
    }
    enum CryoliquidAttackBehavior
    {
        AttackEnemy,
        AttackRandomly
    }
    private struct Weight
    {
        public int w_Aggressive;
        public int w_Aligned;
        public int w_Injured;
        public int w_ObsticalInBetween;
        public int w_CryoliquidShootInRange;
        public int w_CryoliquidAttackInRange;
        public int w_WalkCanAlign;
        public int w_PathTooLong;
        public int w_TooClose;
        public int w_RareCardNotUsed;
        public int w_LegendaryCardNotUsed;
    }
    internal void UpdateWeight()
    {
        weight.w_Aggressive = state.Aggressive;

        if (state.Aligned)
            weight.w_Aligned = 10;
        else
            weight.w_Aligned = -5;

        if (state.CryoliquidAttackInRange)
            weight.w_CryoliquidAttackInRange = 10;
        else
            weight.w_CryoliquidAttackInRange = -5;

        weight.w_Injured = state.Injured;

        if (state.ObsticalInBetween)
            weight.w_ObsticalInBetween = 10;
        else
            weight.w_ObsticalInBetween = 0;

        if (state.CryoliquidShootInRange)
            weight.w_CryoliquidShootInRange = 7;
        else
            weight.w_CryoliquidShootInRange = 0;

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

        weight.w_RareCardNotUsed = state.RareCardNotUsed * 3;
        weight.w_LegendaryCardNotUsed = state.LegendaryCardNotUsed * 2;
    }
    private void UpdateState()
    {
        //aggressive
        if (BattleData.playerData.currentHealth < (BattleData.playerData.maxHealth / 2))
            state.Aggressive = (int)(1 - BattleData.playerData.currentHealth / (BattleData.playerData.maxHealth / 2)) * 5 + 1;
        else
            state.Aggressive = 0;
        //align
        if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x || BattleData.playerData.position.y == BattleData.EnemyDataList[EnemyID].position.y)
            state.Aligned = true;
        else
            state.Aligned = false;
        //attack in range
        if (state.Aligned && Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position) <= 2)
            state.CryoliquidAttackInRange = true;
        else
            state.CryoliquidAttackInRange = false;
        //shooting in range
        if (state.Aligned && Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position) <= 5)
            state.CryoliquidShootInRange = true;
        else
            state.CryoliquidShootInRange = false;
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
                case 11:
                    result.Add(JumpUtility());
                    break;
                case 12:
                    result.Add(DoubleJumpUtility());
                    break;
                case 13:
                    result.Add(CryoliquidShootingUtility());
                    break;
                case 14:
                    result.Add(CryoliquidAttackUtility());
                    break;
            }
        }
        return result;
    }

    private List<float> CryoliquidShootingUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //CryoliquidShootingToAttack
        utility = (weight.w_LegendaryCardNotUsed + weight.w_Aligned + weight.w_CryoliquidShootInRange - weight.w_CryoliquidAttackInRange + weight.w_Aggressive) * w_Legendary;
        result.Add(utility);
        //CryoliquidShootingRandom
        utility = (weight.w_LegendaryCardNotUsed + weight.w_TooClose + weight.w_Injured - weight.w_CryoliquidAttackInRange + weight.w_Aggressive) * w_Legendary;
        result.Add(utility);
        return result;
    }
    private List<float> CryoliquidAttackUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //CryoliquidAttack
        utility = (weight.w_RareCardNotUsed + weight.w_Aligned + weight.w_CryoliquidAttackInRange + weight.w_Aggressive) * w_Rare;
        result.Add(utility);
        //CryoliquidAttackRandom
        utility = (weight.w_RareCardNotUsed + weight.w_TooClose + weight.w_Aggressive + weight.w_Injured + weight.w_Aggressive - weight.w_ObsticalInBetween) * w_Rare;
        result.Add(utility);
        return result;
    }

    private List<float> DoubleJumpUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //DoubleJumpfollowingPath
        utility = (weight.w_PathTooLong + 2 * weight.w_ObsticalInBetween - weight.w_Injured + Random.Range(-3, 3) + weight.w_RareCardNotUsed) * w_Rare;
        result.Add(utility);
        //DoubleJumpForAligning
        utility = (weight.w_Aggressive + weight.w_WalkCanAlign - weight.w_Injured - weight.w_Aligned + 2 * weight.w_ObsticalInBetween + weight.w_RareCardNotUsed) * w_Rare; 
        result.Add(utility);
        //DoubleRandomJump
        utility = (Random.Range(0, 8) - weight.w_Aggressive + weight.w_RareCardNotUsed) * w_Rare;
        result.Add(utility);
        //DoubleJumpBackWardsToPlayer
        utility = (weight.w_TooClose + (5 - weight.w_Aggressive) + Random.Range(-3, 3) + weight.w_ObsticalInBetween + weight.w_RareCardNotUsed) * w_Rare;
        result.Add(utility);
        return result;
    }

    private List<float> JumpUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //JumpfollowingPath
        utility = weight.w_PathTooLong + weight.w_ObsticalInBetween - weight.w_Injured + Random.Range(-3, 3); ;
        result.Add(utility);
        //JumpForAligning
        utility = weight.w_Aggressive + weight.w_WalkCanAlign - weight.w_Injured - weight.w_Aligned + 2 * weight.w_ObsticalInBetween;
        result.Add(utility);
        //RandomJump
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        //JumpBackWardsToPlayer
        utility = weight.w_TooClose + (5 - weight.w_Aggressive) + Random.Range(-3, 3) + weight.w_ObsticalInBetween;
        result.Add(utility);
        return result;
    }

    private List<float> WalkUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //WalkfollowingPath
        utility = weight.w_PathTooLong + weight.w_ObsticalInBetween - weight.w_Injured + Random.Range(-3, 3);
        result.Add(utility);
        //WalkForAligning
        utility = weight.w_Aggressive + weight.w_WalkCanAlign - weight.w_Injured - weight.w_Aligned;
        result.Add(utility);
        //RandomWalk
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        //WalkBackWardsToPlayer
        utility = weight.w_TooClose + (5 - weight.w_Aggressive) + Random.Range(-3, 3);
        result.Add(utility);
        return result;
    }

    public override void EnemyChooseACardToPlay()
    {
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        info.animator = Animator;
        info.animator.SetInteger("Damage", Health);
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

        switch (BehaviourCardID)
        {
            case 1:
                WalkFunc(BehaviourIndex, info);
                break;
            case 11:
                JumpFunc(BehaviourIndex, info);
                break;
            case 12:
                DoubleJumpFunc(BehaviourIndex, info);
                break;
            case 13:
                CryoliquidAttackFunc(BehaviourIndex, info);
                break;
            case 14:
                CryoliquidShootingFunc(BehaviourIndex, info);
                break;
        }
        if (info.card.rarity == Card.Rarity.rare)
            state.RareCardNotUsed++;
        else
            state.RareCardNotUsed = 0;
        if (info.card.rarity == Card.Rarity.legendary)
            state.LegendaryCardNotUsed++;
        else
            state.LegendaryCardNotUsed = 0;
        BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);
    }

    private void CryoliquidShootingFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        switch (behaviourIndex)
        {
            case (int)CryoliquidShootingBehavior.ShootAtEnemy:
                if (state.Aligned)
                {
                    if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x)
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x), 0));
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y)));
                }
                else
                {
                    if (Random.Range(0, 1) == 0)
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x), 0));
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y)));
                }
                break;
            case (int)CryoliquidShootingBehavior.ShootRandomly:
                if (Random.Range(0, 1) == 0)
                    info.Selection.Add(new Vector2(Random.Range(-5, 5), 0));
                else
                    info.Selection.Add(new Vector2(0, Random.Range(-5, 5)));
                break;
        }
    }

    private void CryoliquidAttackFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        switch (behaviourIndex)
        {
            case (int)CryoliquidAttackBehavior.AttackEnemy:
                if (state.Aligned)
                {
                    if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x)
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x), 0));
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y)));
                }
                else
                {
                    if (Random.Range(0, 1) == 0)
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x), 0));
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y)));
                }
                break;
            case (int)CryoliquidAttackBehavior.AttackRandomly:
                if (Random.Range(0, 1) == 0)
                    info.Selection.Add(new Vector2(Random.Range(-1, 1), 0));
                else
                    info.Selection.Add(new Vector2(0, Random.Range(-1, 1)));
                break;
        }
    }
    private void DoubleJumpFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new Vector2(0, 0);
        int randomValue = Random.Range(1, 3);
        switch (behaviourIndex)
        {
            case (int)JumpBehavior.JumpFollowingPath:
                for (int i = 0; i < 3; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else
                        break;
                }
                info.Selection.Add(result);
                break;
            case (int)JumpBehavior.JumpForAligning:
                float disx = Mathf.Abs(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x);
                float disy = Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disx < disy)
                {
                    if (disx <= 3)
                        info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    else
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) * 3, 0));
                }
                else
                {
                    if (disy <= 3)
                        info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) * 3));

                }
                break;
            case (int)JumpBehavior.RandomJump:
                int dir = Random.Range(0, 3);
                switch (dir)
                {
                    case 0:
                        info.Selection.Add(new Vector2(0, 1 * (int)Random.Range(1, 3)));
                        break;
                    case 1:
                        info.Selection.Add(new Vector2(0, -1 * (int)Random.Range(1, 3)));
                        break;
                    case 2:
                        info.Selection.Add(new Vector2(-1 * (int)Random.Range(1, 3), 0));
                        break;
                    case 3:
                        info.Selection.Add(new Vector2(1 * (int)Random.Range(1, 3), 0));
                        break;
                }
                break;
            case (int)JumpBehavior.JumpBackWardsToPLayer:
                Vector2 dirVec = BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position;
                if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
                {
                    info.Selection.Add(new Vector2(0, -Mathf.Sign(dirVec.y) * randomValue));
                    break;
                }
                else
                {
                    info.Selection.Add(new Vector2(-Mathf.Sign(dirVec.x) * randomValue, 0));
                    break;
                }
        }
    }
        private void JumpFunc(int behaviourIndex, Card.InfoForActivate info)
    {
        Vector2 result = new Vector2(0, 0);
        int randomValue = Random.Range(1, 2);
        switch (behaviourIndex)
        {
            case (int)JumpBehavior.JumpFollowingPath:
                for (int i = 0; i < 2; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else
                        break;
                }
                info.Selection.Add(result);
                break;
            case (int)JumpBehavior.JumpForAligning:
                float disx = Mathf.Abs(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x);
                float disy = Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disx < disy)
                {
                    if (disx <= 2)
                        info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    else
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) * 2, 0));
                }
                else
                {
                    if (disy <= 2)
                        info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) * 2));

                }
                break;
            case (int)JumpBehavior.RandomJump:
                int dir = Random.Range(0, 3);
                switch (dir)
                {
                    case 0:
                        info.Selection.Add(new Vector2(0, 1 * (int)Random.Range(1, 2)));
                        break;
                    case 1:
                        info.Selection.Add(new Vector2(0, -1 * (int)Random.Range(1, 2)));
                        break;
                    case 2:
                        info.Selection.Add(new Vector2(-1 * (int)Random.Range(1, 2), 0));
                        break;
                    case 3:
                        info.Selection.Add(new Vector2(1 * (int)Random.Range(1, 2), 0));
                        break;
                }
                break;
            case (int)JumpBehavior.JumpBackWardsToPLayer:
                Vector2 dirVec = BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position;
                if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
                {
                    info.Selection.Add(new Vector2(0, -Mathf.Sign(dirVec.y) * randomValue));
                    break;
                }
                else
                {
                    info.Selection.Add(new Vector2(-Mathf.Sign(dirVec.x) * randomValue, 0));
                    break;
                }
        }
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
    }
}
