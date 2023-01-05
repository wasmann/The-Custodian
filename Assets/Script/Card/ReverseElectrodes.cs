using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseElectrodes : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "ReverseElectrodes"; } }
    public override Rarity rarity { get { return Rarity.epic; } }
    public override int Speed { get { return 5; } }
    public override int ID { get { return 22; } }
    public override IEnumerator Play()
    {
        BattleData.playerData.currentEnergy -= 3;
        UI.UpdatePlayerData();

        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        Audio.Play();
        if (Info.owner_ID == 0)
        {
            BattleData.playerData.buff.ReverseEle += 1;
            UI.UpdatePlayerData();
        }
        else
        {
            BattleData.EnemyData newData = BattleData.EnemyDataList[Info.owner_ID];
            newData.buff.ReverseEle += 1;
            BattleData.EnemyDataList[Info.owner_ID] = newData;
            UI.UpdateEnemyData(Info.owner_ID);
        }
    }

    private void Start()
    {
        TargetNum =0;
        RangeNotation = "BuffNotation";
        SelectionNotation = "TargetSelection";

    }
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
