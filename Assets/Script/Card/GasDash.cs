using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasDash : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "GasDash"; } }
    public override Rarity rarity { get { return Rarity.rare; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 19; } }
    public override IEnumerator Play()
    {
        BattleData.playerData.currentEnergy -= 2;
        UI.UpdatePlayerData();
        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(3, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(4, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(5, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-2, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-3, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-4, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-5, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -3));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -4));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -5));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 3));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 4));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 5));


        yield return new WaitForSeconds(0.1f);
        UI.ShowNotation(this);
        TileMapButton.MakeSelectable(this);
        yield return new WaitUntil(() => TargetNum == 0);
        TileMapButton.MakeUnSelectable();
        //BattleData.PlayingACard = false;
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        Audio.Play();
        //TODO : should check environment first! need more implementation
        List<Vector2> path = new List<Vector2>();
        Vector2 normalVec = Info.Selection[0] / Info.Selection[0].magnitude;
        for (int i = 1; i < Info.Selection[0].magnitude; i++)
        {
            path.Add(BattleData.playerData.position + normalVec);
        }
        if (Info.owner_ID == 0)
        {
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                for(int j = 0; j < path.Count; j++)
                {
                    if (BattleData.EnemyDataList[i].position == path[j])
                    {
                        BattleData.EnemyData data = BattleData.EnemyDataList[i];
                        data.currentHealth -= 2;
                        BattleData.EnemyDataList[i] = data;
                        UI.UpdateEnemyData(i);
                    }
                }
            }
            BattleData.playerData.position += Info.Selection[0];
            UI.UpdatePlayerData();
        }
        else
        {
            StartCoroutine(Animate(Info.animator));
            for (int j = 0; j < path.Count; j++)
            {
                if(BattleData.playerData.position == path[j])
                {
                    BattleData.playerData.currentHealth -= 2;
                    UI.UpdatePlayerData();
                    break;
                }
            }
            BattleData.EnemyData data = BattleData.EnemyDataList[Info.owner_ID];
            data.position += Info.Selection[0];
            BattleData.EnemyDataList[Info.owner_ID] = data;
            UI.UpdateEnemyData(Info.owner_ID);
        }
    }

    public IEnumerator Animate(Animator animator)
    {
        animator.SetBool("GasDash", true);
        yield return new WaitForSeconds(0);
        animator.SetBool("GasDash", false);
    }

    private void Start()
    {
        TargetNum = 1;
        RangeNotation = "MovementNotation";
        SelectionNotation = "ArrowSelection";

    }
    public override void ReSetTarget()
    {
        TargetNum = 1;
    }
}