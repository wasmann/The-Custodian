using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalShield : Card
{
    public override string Name { get { return "ElectricalShield"; } }
    public override Rarity rarity { get { return Rarity.common; } }
    public override int Speed { get { return 4; } }
    public override int ID { get { return 23; } }
    public override IEnumerator Play()
    {
        BattleData.playerData.currentEnergy -= 1;
        UI.UpdatePlayerData();
        Info.direction.Add(BattleData.playerData.position);

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
            BattleData.playerData.buff.Shield = 3;
            UI.UpdatePlayerData();
        }
        else
        {
            int SelectionID = int.Parse(Info.otherInfo[0]);
            BattleData.EnemyData newData = BattleData.EnemyDataList[SelectionID];
            newData.buff.Shield = 3;
            BattleData.EnemyDataList[SelectionID] = newData;
            UI.UpdateEnemyData(SelectionID);
        }
    }

    private void Start()
    {
        TargetNum = 1;
        RangeNotation = "BuffNotation";
        SelectionNotation = "TargetSelection";

    }
    public override void ReSetTarget()
    {
        TargetNum = 1;
    }
}
