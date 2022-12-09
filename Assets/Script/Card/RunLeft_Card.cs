using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunLeft_Card : Card
{   public override string Name { get { return "Run Left"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 4; } }
    public override int TargetNum { get { return 1; } set { } }
    public override IEnumerator Play()
    {
        Info.owner_ID = 0;
        //here need more implementation about "not allowing to walk on occupied grid"
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-3, 0));
        Notation.Add(this.transform.Find("RangeNotation").gameObject);
        Notation.Add(this.transform.Find("SelectionNotation").gameObject);
        // BattleData.CardReadyToPlay = this;
        UI.ShowNotation(Notation, Info);
        //assign the functionality to grids in info.direction
        yield return new WaitUntil(() => TargetNum == 0);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        BattleData.playerData.position = Info.Selection[0];
    }

}
