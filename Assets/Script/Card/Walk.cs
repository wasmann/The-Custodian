using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : Card
{
    public override string Name { get { return "Walk"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 2; } }
    public override int ID { get { return 1; } }
    public override IEnumerator Play()
    {
        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
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
        if (Info.owner_ID == 0)
        {
            BattleData.playerData.position += Info.Selection[0];
            UI.UpdatePlayerData();
        }
        else
        {
            BattleData.EnemyData newData = BattleData.EnemyDataList[Info.owner_ID];
            newData.position += Info.Selection[0];
            BattleData.EnemyDataList[Info.owner_ID] = newData;
            UI.UpdateEnemyData(Info.owner_ID);
        }
    }
    private void Start()
    {
        TargetNum = 1;
        RangeNotation = "MovementNotation";
        SelectionNotation = "ArrowSelection";

    }

    public override void ReSetTarget()
    {
        TargetNum = 1;
    }
}
