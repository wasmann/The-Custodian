using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk_Card : Card
{
    public override string Name { get { return "Walk"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 2; } }
    public override int ID { get { return 1; } }
    public override int TargetNum { get { return 1; } set { } }
    public override IEnumerator Play()
    {
        Info.owner_ID = 0;
        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
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
        BattleData.playerData.position = Info.Selection[0];
    }
}
