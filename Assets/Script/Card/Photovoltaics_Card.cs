using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photovoltaics_Card : Card
{

    public override string Name { get { return "Photovoltaics"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 8; } }
    public override int TargetNum { get { return 0; } set { } }
    public override IEnumerator Play()
    {
        Info.owner_ID = 0;
        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        if(Info.owner_ID == 0)
        {
            BattleData.playerData.currentEnergy += 1;
            BattleData.playerData.currentHealth += 2;
        }
        else
        {
            BattleData.EnemyData newData = BattleData.EnemyDataList[Info.owner_ID];
            newData.currentHealth +=2;
            BattleData.EnemyDataList[Info.owner_ID] = newData;
        }


    }
}
