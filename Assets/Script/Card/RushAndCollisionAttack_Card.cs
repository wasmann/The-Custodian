using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushAndCollisionAttack_Card : Card
{

    public override string Name { get { return "Rush and Collision Attack"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 5; } }
    public override int ID { get { return 7; } }
    public override int TargetNum { get { return 1; } set { } }

    public override IEnumerator Play()
    {
        Info.owner_ID = 0;
        BattleData.playerData.currentEnergy -= 1;
        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(3, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-3, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 3));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -3));

        Notation.Add(this.transform.Find("RangeNotion").gameObject);
        Notation.Add(this.transform.Find("SelectionNotion").gameObject);
        BattleData.CardReadyToPlay = this;
        UI.ShowNotation(Notation);
        //assign the functionality to grids in info.direction
        yield return new WaitUntil(() => TargetNum == 0);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {







        this.BattleData.battleData.EnemyDataList.at(0).health -= 2 + (this.BattleData.battleData.playerData.position - info.direction.at(0)).Length();
        this.BattleData.battleData.playerData.position = info.Direction.at(0);
    }
}
