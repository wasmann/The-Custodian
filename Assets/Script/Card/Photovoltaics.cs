using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photovoltaics : Card
{
    [SerializeField] private AudioSource Audio;

    public override string Name { get { return "Photovoltaics"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 8; } }
    public override IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        Audio.Play();
        if (Info.owner_ID == 0)
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
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
