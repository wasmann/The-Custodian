using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reloading : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "Reloading"; } }
    public override Rarity rarity { get { return Rarity.trash; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 20; } }
    public override IEnumerator Play()
    {
        Info.owner_ID = 0;
        BattleData.playerData.currentEnergy = 5;
        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        if (Info.owner_ID == 0)
        {
            //TODO:show animation 
            
            Debug.Log("player reloading");
        }
        else
        {
            Audio.Play();
            BattleData.EnemyData data = BattleData.EnemyDataList[Info.owner_ID];
            data.buff.Bullet = true;
            BattleData.EnemyDataList[Info.owner_ID] = data;
            UI.UpdateEnemyData(Info.owner_ID);
            Debug.Log("");
        }
    }
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
