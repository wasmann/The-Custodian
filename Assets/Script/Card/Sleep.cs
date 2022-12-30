using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : Card
{

    public override string Name { get { return "Sleep"; } }
    public override Rarity rarity { get { return Rarity.trash; } }
    public override int Speed { get { return 10; } }
    public override int ID { get { return 10; } }
    public override IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        if (Info.owner_ID == 0)
        {
            //remove zZZ
            BattleData.playerData.buff.sleep = false;
            Debug.Log("player awake");
        }
        else
        {
            //remove zZZ

            BattleData.EnemyData newdata = new BattleData.EnemyData();
            newdata = BattleData.EnemyDataList[Info.owner_ID];
            newdata.buff.sleep = false;
            BattleData.EnemyDataList[Info.owner_ID] = newdata;
            Debug.Log("Enemy awake");
        }
    }
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
