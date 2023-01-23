using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbutt : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "Headbutt"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 4; } }
    public override int ID { get { return 6; } }
    public override IEnumerator Play()
    {

        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
        yield return new WaitForSeconds(0.1f);
        UI.ShowNotation(this);
        TileMapButton.MakeSelectable(this);
        yield return new WaitUntil(() => TargetNum == 0);
        TileMapButton.MakeUnSelectable();
      //  BattleData.PlayingACard = false;
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        Audio.Play();
        if (Info.owner_ID == 0)
        {
            Info.animator.SetTrigger("Attack");
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                if (BattleData.EnemyDataList[i].position == Info.Selection[0]+ BattleData.playerData.position)
                {
                    BattleData.EnemyData data = BattleData.EnemyDataList[i];
                    data.currentHealth -= 3;
                    BattleData.EnemyDataList[i] = data;
                    UI.UpdateEnemyData(i);
                    return;
                }
            }
        }
        else
        {
            if (BattleData.playerData.position == Info.Selection[0] + BattleData.EnemyDataList[Info.owner_ID].position)
            {
                BattleData.playerData.currentHealth -= 3;
                UI.UpdatePlayerData();
            }
        }
    }

    private void Start()
    {
        TargetNum = 1;
        RangeNotation = "AttackNotation";
        SelectionNotation = "AttackSelection";

    }
    public override void ReSetTarget()
    {
        TargetNum = 1;
    }
}
