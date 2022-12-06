using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbutt_Card : Card
{   
    public override string Name { get { return "Headbutt"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 4; } }
    public override int ID { get { return 6; } }
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
        BattleData.CardReadyToPlay=this;
        UI.ShowNotation(Notation);
        //assign the functionality to grids in info.direction
        yield return new WaitUntil(() => TargetNum == 0);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
       for(int i= 1; i < BattleData.EnemyDataList.Count+1; i++)
       {
            if (BattleData.EnemyDataList[i].position == Info.Selection[0])
            {
                BattleData.EnemyData data= BattleData.EnemyDataList[i];
                data.currentHealth -= 3;
                BattleData.EnemyDataList[i] = data;
            }
       }
    }
}
