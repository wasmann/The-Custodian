using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoliquidAttack : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "CryoliquidAttack"; } }
    public override Rarity rarity { get { return Rarity.rare; } }
    public override int Speed { get { return 4; } }
    public override int ID { get { return 13; } }
    public override IEnumerator Play()
    {
        BattleData.playerData.currentEnergy -= 2;
        UI.UpdatePlayerData();
        Info.direction.Add(BattleData.playerData.position + new Vector2(2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 2));

        yield return new WaitForSeconds(0.1f);
        UI.ShowNotation(this);
        TileMapButton.MakeSelectable(this);
        yield return new WaitUntil(() => TargetNum == 0);
        TileMapButton.MakeUnSelectable();
        //BattleData.PlayingACard = false;
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        Audio.Play();
        Info.animator.SetTrigger("Attack");
        Info.Selection[0].Normalize();
        if (Info.owner_ID == 0)
        {
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                if (BattleData.EnemyDataList[i].position == Info.Selection[0] + BattleData.playerData.position)
                {
                    BattleData.EnemyData data = BattleData.EnemyDataList[i];
                    data.buff.Frozen += 2;
                    BattleData.EnemyDataList[i] = data;
                    UI.UpdateEnemyData(i);
                    return;
                }
            }
            Info.Selection[0] *= 2;
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                if (BattleData.EnemyDataList[i].position == Info.Selection[0] + BattleData.playerData.position)
                {
                    BattleData.EnemyData data = BattleData.EnemyDataList[i];
                    data.buff.Frozen += 2;
                    BattleData.EnemyDataList[i] = data;
                    UI.UpdateEnemyData(i);
                    return;
                }
            }
            Debug.Log("player cryoliquidattack miss");
        }
        else
        {
            if (BattleData.playerData.position == Info.Selection[0] + BattleData.EnemyDataList[Info.owner_ID].position|| BattleData.playerData.position == Info.Selection[0]*2 + BattleData.EnemyDataList[Info.owner_ID].position)
            {
                BattleData.playerData.currentHealth -= 2;
                UI.UpdatePlayerData();
            }
         }
    }

    private void Start()
    {
        TargetNum = 1;
        RangeNotation = "AttackNotation";
        SelectionNotation = "AttackSelection_LongRange";

    }
    public override void ReSetTarget()
    {
        TargetNum = 1;
    }
}
