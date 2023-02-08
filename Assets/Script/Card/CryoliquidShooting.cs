using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoliquidShooting : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "CryoliquidShooting"; } }
    public override Rarity rarity { get { return Rarity.legendary; } }
    public override int Speed { get { return 6;} }
    public override int ID { get { return 14; } }
    public override IEnumerator Play()
    {
        BattleData.playerData.currentEnergy -= 4;
        UI.UpdatePlayerData();
        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(3, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(4, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(5, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-3, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-4, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-5, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -3));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -4));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -5));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 3));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 4));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 5));


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
        Info.Selection[0].Normalize();
        if (Info.owner_ID == 0)
        {
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                if (BattleData.EnemyDataList[i].position == Info.Selection[0] + BattleData.playerData.position ||
                    BattleData.EnemyDataList[i].position == Info.Selection[0]*2 + BattleData.playerData.position||
                    BattleData.EnemyDataList[i].position == Info.Selection[0]*3 + BattleData.playerData.position||
                    BattleData.EnemyDataList[i].position == Info.Selection[0]*4 + BattleData.playerData.position||
                    BattleData.EnemyDataList[i].position == Info.Selection[0]*5 + BattleData.playerData.position)
                {
                    BattleData.EnemyData data = BattleData.EnemyDataList[i];
                    //data.buff.Frozen +=4;
                    data.currentHealth -= 3;
                    BattleData.EnemyDataList[i] = data;
                    UI.UpdateEnemyData(i);
                }
            }
            Debug.Log("player CryoliquidShooting miss");
        }
        else
        {
            if (BattleData.playerData.position == Info.Selection[0] + BattleData.EnemyDataList[Info.owner_ID].position 
                || BattleData.playerData.position == Info.Selection[0] * 2 + BattleData.EnemyDataList[Info.owner_ID].position
                || BattleData.playerData.position == Info.Selection[0] * 3 + BattleData.EnemyDataList[Info.owner_ID].position
                || BattleData.playerData.position == Info.Selection[0] * 4 + BattleData.EnemyDataList[Info.owner_ID].position
                || BattleData.playerData.position == Info.Selection[0] * 5 + BattleData.EnemyDataList[Info.owner_ID].position)
            {
                Info.animator.SetTrigger("Shoot");
                BattleData.playerData.currentHealth -= 3;
                //BattleData.playerData.buff.Frozen += 4;
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
