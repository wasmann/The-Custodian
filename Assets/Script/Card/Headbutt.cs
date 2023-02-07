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
            GameObject obj = Instantiate(Resources.Load("Prefab/UI/Slash") as GameObject);
            obj.transform.position = ToolFunction.FromCoorinateToWorld(Info.Selection[0] + BattleData.playerData.position);
            Destroy(obj, 1f);
            UI.FlipCusto(Info.Selection[0].x);
            
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                if (BattleData.EnemyDataList[i].position == Info.Selection[0]+ BattleData.playerData.position)
                {
                    BattleData.EnemyData data = BattleData.EnemyDataList[i];
                    data.currentHealth -= 3;

                    GameObject obj2 = Instantiate(Resources.Load("Prefab/UI/Headbutt") as GameObject);
                    obj2.transform.position = ToolFunction.FromCoorinateToWorld(Info.Selection[0] + BattleData.playerData.position);
                    Destroy(obj2, 1f);

                    BattleData.EnemyDataList[i] = data;
                    UI.UpdateEnemyData(i);
                    return;
                }
            }
        }
        else
        {
            GameObject obj = Instantiate(Resources.Load("Prefab/UI/Slash") as GameObject);
            obj.transform.position = ToolFunction.FromCoorinateToWorld(Info.Selection[0] + BattleData.EnemyDataList[Info.owner_ID].position);
            Destroy(obj, 1f);
            if (BattleData.playerData.position == Info.Selection[0] + BattleData.EnemyDataList[Info.owner_ID].position)
            {
                GameObject obj2 = Instantiate(Resources.Load("Prefab/UI/Headbutt") as GameObject);
                //obj2.transform.position = ToolFunction.FromCoorinateToWorld(Info.Selection[0] + BattleData.playerData.position);
                obj2.transform.position = ToolFunction.FromCoorinateToWorld(BattleData.playerData.position);
                UI.FlipEnemy(Info.owner_ID, Info.Selection[0].x); //turn before attack
                Destroy(obj2, 1f);

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
