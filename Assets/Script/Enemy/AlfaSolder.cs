using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlfaSolder : Enemy
{
    public override string EnemyName { get { return "AlfaSolder"; } }

    public List<Card> deck;
    public override List<Card> CardsDeck
    {
        get
        {
            return deck;//1,1,18,18,20,20,19 
            //1.walk : Basic
            //18.Gun shoot : Common
            //20.Reloading : Trash
            //19.Gas Dash : Rare
        }
    }
    public override int Health { get { return 8; } }

    public override int AttackMaxRange { get { return 5; } }
    public override int HandCardNum { get { return 2; } }


    float w_Common = 0.8f;
    float w_Basic = 1;
    float w_Rare = 0.7f;
    float w_Trash = 1;

    private State state;
    Weight weight;
    List<Vector2> pathToPlayer;
    private struct State
    {
        public int Aggressive;//from 0 to 5
        public bool AlignOrNot;
        public bool ObsticalInBetween;// wall but not some environment that can't pass
        public bool GunShootInRange;
        public bool WalkCanAlign;
        public int Injured;//from 0 to 2
        public bool Loaded;
        public bool HasGunShoot;
        public bool PathTooLong;
        public bool TooClose;
        public int RareCardNotUsed;
    }
    enum WalkBehavior
    {
        WalkFollowingPath,
        WalkForAligning,
        RandomWalk,
        WalkBackWardsToPlayer,
    }
    enum GunShootBehavior {
        ShootPlayer,
        ShootRandomly
    }
    enum ReloadingBehavior
    {
        justReload
    }
    enum GasDashBehavior
    {
        DashFollowingPath,
        DashForAligning,
        DashToAttack,
        DashBackWards,//Because he is range attacker.

    }
    private struct Weight
    {
        public int w_Aggressive;//0 to 5
        public int w_AlignOrNot;// -5 or 5
        public int w_ObsticalInBetween;
        public int w_Injured;
        public int w_Loaded;
        public int w_GunShootInRange;
        public int w_HasGunShoot;
        public int w_PathToLong;
        public int w_WalkCanAlign;
        public int w_TooClose;
        public int w_RareCardNotUsed;
    }

    internal void UpdateWeight()
    {
        weight.w_Aggressive = state.Aggressive;

        if (state.AlignOrNot)
            weight.w_AlignOrNot = 10;
        else
            weight.w_AlignOrNot = -5;

        if (state.GunShootInRange)
            weight.w_GunShootInRange = 8;
        else
            weight.w_GunShootInRange = -3;

        weight.w_Injured = state.Injured;

        if (state.ObsticalInBetween)
            weight.w_ObsticalInBetween = 10;
        else
            weight.w_ObsticalInBetween = 0;

        if (state.HasGunShoot)
            weight.w_HasGunShoot = 8;
        else
            weight.w_HasGunShoot = 0;

        if (state.Loaded)
            weight.w_Loaded = 5;
        else
            weight.w_Loaded = -10;

        if (state.WalkCanAlign)
            weight.w_WalkCanAlign = 3;
        else
            weight.w_WalkCanAlign = 0;

        if (state.TooClose)
            weight.w_TooClose = 5;
        else
            weight.w_TooClose = 0;

        if (state.PathTooLong)
            weight.w_PathToLong = 10;
        else
            weight.w_PathToLong = 0;

        weight.w_RareCardNotUsed = state.RareCardNotUsed * 3;
    }
    private void UpdateState()
    {
        //aggresive
        if (BattleData.playerData.currentHealth < (BattleData.playerData.maxHealth / 2))
            state.Aggressive = (int)(1 - BattleData.playerData.currentHealth / (BattleData.playerData.maxHealth / 2)) * 5 + 1;
        else
            state.Aggressive = 0;
        //AlignOrNot
        if (BattleData.playerData.position.x == BattleData.EnemyDataList[EnemyID].position.x || BattleData.playerData.position.y == BattleData.EnemyDataList[EnemyID].position.y)
            state.AlignOrNot = true;
        else
            state.AlignOrNot = false;
        //GunShootInRange
        if (state.AlignOrNot && Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position) <= 4)
            state.GunShootInRange = true;
        else
            state.GunShootInRange = false;
        //WalkCanAlign
        if (Mathf.Abs(BattleData.playerData.position.x- BattleData.EnemyDataList[EnemyID].position.x)==1|| Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) == 1)
            state.WalkCanAlign = true;
        else
            state.WalkCanAlign = false;
        //ObsticalInBetween;
        if (pathToPlayer.Count == Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position))
            state.ObsticalInBetween = false;
        else
            if (true)// some environment that movement cant pass but attack can pass 
                     //TODO
            state.ObsticalInBetween = true;
        else
            state.ObsticalInBetween = false;
        //Injured
        if (BattleData.EnemyDataList[EnemyID].currentHealth <= (BattleData.EnemyDataList[EnemyID].maxHealth / 3))
            state.Injured = (int)(1 - BattleData.EnemyDataList[EnemyID].currentHealth / (BattleData.EnemyDataList[EnemyID].maxHealth / 2)) * 2 + 1;
        else
            state.Injured = 0;
        //Loaded
        if (BattleData.EnemyDataList[EnemyID].buff.Bullet)
            state.Loaded = true;
        else
            state.Loaded = false;
        //HasReload;
        if (Card.Contain(BattleData.EnemyDataList[EnemyID].handCard, 18))
            state.HasGunShoot = true;
        else
            state.HasGunShoot = false;
        //PathTooLong;
        if (pathToPlayer.Count >= 8)
            state.PathTooLong = true;
        else
            state.PathTooLong = false;

        //TooClose;
        if (state.GunShootInRange && Vector2.Distance(BattleData.playerData.position, BattleData.EnemyDataList[EnemyID].position) <= 2)
            state.TooClose = true;
        else
            state.TooClose = false;
        //Dont update RareCardNotUsed here. 
    }

    private List<List<float>> CalculateAllUtilities(List<Card> handcards)
    {
        List<List<float>> result = new List<List<float>>();// The outer list indicates card, the inner list indicates ultility according to baviour
        for(int i = 0; i < handcards.Count; i++)
        {
            switch (handcards[i].ID)
            {
                case 1:
                    result.Add(WalkUtility());
                    break;
                case 18:
                    result.Add(GunShootUtility());
                    break;
                case 19:
                    result.Add(GasDashUtility());
                    break;
                case 20:
                    result.Add(ReloadUtility());
                    break;
            }
        } 
        return result;
    }

    private List<float> WalkUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //WalkfollowingPath
        utility = weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured + Random.Range(-3, 3);
        result.Add(utility);
        //WalkForAligning
        utility = weight.w_Aggressive + weight.w_WalkCanAlign + weight.w_Loaded + Random.Range(-3, 3) - weight.w_Injured-weight.w_AlignOrNot;
        result.Add(utility);
        //RandomWalk
        utility = Random.Range(0, 8) - weight.w_Aggressive;
        result.Add(utility);
        //WalkBackWardsToPlayer
        utility = weight.w_TooClose + (5 - weight.w_Aggressive) + Random.Range(-3, 3);
        result.Add(utility);
        return result;
    }

    private List<float> ReloadUtility()
    {
        int utility;
        List<float> result = new List<float>();
        //JustReloading
        utility = weight.w_HasGunShoot - weight.w_Loaded+Random.Range(-3, 3);
        result.Add(utility);
        return result;
    }

    private List<float> GunShootUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //ShootPlayer
        utility = (weight.w_Loaded + weight.w_Aggressive + weight.w_GunShootInRange - weight.w_PathToLong - weight.w_Injured + Random.Range(-5, 5)) * w_Common;
        result.Add(utility);
        //ShootRandomly
        utility = (weight.w_TooClose + weight.w_Injured + weight.w_Loaded + weight.w_Aggressive + Random.Range(-5, 0)) * w_Common;
        result.Add(utility);
        return result;
    }
    private List<float> GasDashUtility()
    {
        float utility;
        List<float> result = new List<float>();
        //DashfollowingPath
        utility = (weight.w_PathToLong + weight.w_ObsticalInBetween - weight.w_Injured + Random.Range(-3,3)+weight.w_RareCardNotUsed) * w_Rare;
        result.Add(utility);
        //DashForAligning
        utility = (weight.w_Aggressive + weight.w_WalkCanAlign + weight.w_Loaded + Random.Range(-3, 3) - weight.w_Injured - weight.w_AlignOrNot+ weight.w_RareCardNotUsed) * w_Rare;
        result.Add(utility);
        //DashAttack
        utility = (weight.w_RareCardNotUsed+weight.w_AlignOrNot+weight.w_TooClose+ weight.w_Aggressive + Random.Range(-3, 3)) * w_Rare;
        result.Add(utility);
        //DashBackWardsToPlayer
        utility = (weight.w_TooClose + (5 - weight.w_Aggressive) + Random.Range(-3, 5)+ weight.w_RareCardNotUsed) *w_Rare;
        result.Add(utility);
        return result;
    }

    public override void EnemyChooseACardToPlay()
    {
        Card.InfoForActivate info = new Card.InfoForActivate();
        info.owner_ID = EnemyID;
        UpdateState();
        UpdateWeight();
        List<List<float>> BehaviourUtility;
        BehaviourUtility=CalculateAllUtilities(BattleData.EnemyDataList[EnemyID].handCard);

        int BehaviourIndex=0;
        int BehaviourCardID=0;
        float maxUtility=-9999;
        for(int i = 0; i < BehaviourUtility.Count; i++)
        {
            for(int j=0;j< BehaviourUtility[i].Count; j++)
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
            case 18:
                GunShootFunc(BehaviourIndex, info);
                break;
            case 19:
                DashFunc(BehaviourIndex, info);
                break;
            case 20:
                ReloadFunc(BehaviourIndex, info);
                break;
        }
        if (info.card.rarity == Card.Rarity.rare)
            state.RareCardNotUsed++;
        else
            state.RareCardNotUsed = 0;
        BattleLevelDriver.NewCardPlayed(info);
        UpdatePiles(info.card);
    }

    private void WalkFunc(int BehaviourID,  Card.InfoForActivate info )
    {
        switch (BehaviourID)
        {
            case (int)WalkBehavior.WalkFollowingPath:
                info.Selection.Add(pathToPlayer[0]);
                break;

            case (int)WalkBehavior.WalkForAligning:
                if (BattleData.playerData.position.x != BattleData.EnemyDataList[EnemyID].position.x)
                {
                    if(BattleData.playerData.position.x> BattleData.EnemyDataList[EnemyID].position.x)
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
                        info.Selection.Add(new Vector2(0,1));
                        break;
                    }
                    else
                    {
                        info.Selection.Add(new Vector2(0,-1));
                        break;
                    }
                }

            case (int)WalkBehavior.RandomWalk:
                int dir = Random.Range(0, 3);
                switch (dir){
                    case 0:
                        info.Selection.Add(new Vector2(0,1));
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
    private void DashFunc(int BehaviourID, Card.InfoForActivate info)
    {
        Vector2 result=new Vector2(0,0);
        int randomValue;
        switch (BehaviourID)
        {
            case (int)GasDashBehavior.DashFollowingPath:              
                for(int i = 0; i < 5; i++)
                {
                    if (pathToPlayer[i] == pathToPlayer[0])
                        result += pathToPlayer[0];
                    else
                        break;
                }
                info.Selection.Add(result);
                break;

            case (int)GasDashBehavior.DashForAligning:
                float disx = Mathf.Abs(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x);
                float disy = Mathf.Abs(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y);
                if (disx < disy)
                {
                    if (disx <= 5) 
                        info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    else
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) * 5,0));
                }
                else
                {
                    if (disy <= 5)
                        info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) * 5));
                    
                }
                break;

            case (int)GasDashBehavior.DashBackWards:
                randomValue = Random.Range(1, 5);
                Vector2 dirVec = BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position;
                if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
                {
                    info.Selection.Add(new Vector2(0, -dirVec.y / Mathf.Abs(dirVec.y) * randomValue));
                    break;
                }
                else
                {
                    info.Selection.Add(new Vector2(-dirVec.x / Mathf.Abs(dirVec.x) * randomValue, 0));
                    break;
                }

            case (int)GasDashBehavior.DashToAttack:
                if (state.AlignOrNot)
                {
                    if(BattleData.playerData.position.x== BattleData.EnemyDataList[EnemyID].position.x)
                        info.Selection.Add(new Vector2(Mathf.Sign(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x) * 5, 0));
                    else
                        info.Selection.Add(new Vector2(0, Mathf.Sign(BattleData.playerData.position.y - BattleData.EnemyDataList[EnemyID].position.y) * 5));
                }
                else//random dash
                {
                    
                    if(System.Convert.ToBoolean(Random.Range(0, 1))){
                        info.Selection.Add(new Vector2(Random.Range(-5, 5), 0));
                    }
                    else
                    {
                        info.Selection.Add(new Vector2(0,Random.Range(-5, 5)));
                    }
                }
                break;
        }
    }
    private void ReloadFunc(int BehaviourID, Card.InfoForActivate info)
    {
        Debug.Log("enemy Reloading");
    }
    private void GunShootFunc(int BehaviourID, Card.InfoForActivate info)
    {
        switch (BehaviourID)
        {
            case (int)GunShootBehavior.ShootPlayer:
                if (state.AlignOrNot)
                {
                    info.Selection.Add(BattleData.playerData.position - BattleData.EnemyDataList[EnemyID].position);
                    break;
                }
                else
                {
                    if (Random.Range(0, 1) == 0)
                       info.Selection.Add(new Vector2(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x,0));
                    else
                       info.Selection.Add(new Vector2(0,BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x));
                    break;
                }
            case (int)GunShootBehavior.ShootRandomly:
                if (Random.Range(0, 1) == 0)
                    info.Selection.Add(new Vector2(BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x, 0));
                else
                    info.Selection.Add(new Vector2(0, BattleData.playerData.position.x - BattleData.EnemyDataList[EnemyID].position.x));
                break;
        }
    }
}
