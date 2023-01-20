using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingLightning : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "FallingLightning"; } }
    public override Rarity rarity { get { return Rarity.common; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 25; } }
    public override IEnumerator Play()
    {

        //NEED TO WRITE A FUNCTION TO ADD DIAMOND RANGE
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) <= 5 && (i != 0 && j != 0))
                {
                    Info.direction.Add(BattleData.playerData.position + new Vector2(i, j));
                }
            }
        }

        ///TODO!!!!!!!!!!!!!!!!!!!!!!!
        ///
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
        Info.animator.SetTrigger("Attack");
        int random = (int)Random.Range(0, 10);
        if(random >= 70)
        {
            int randomNeighbor = (int)Random.Range(0, 3);
            switch (randomNeighbor)
            {
                case 0:
                    Info.Selection[0] += new Vector2(1, 0);
                    break;
                case 1:
                    Info.Selection[0] += new Vector2(-1, 0);
                    break;
                case 2:
                    Info.Selection[0] += new Vector2(0,1);
                    break;
                case 3:
                    Info.Selection[0] += new Vector2(0,-1);
                    break;
                default:
                    break;
            }
        }
        if (Info.owner_ID == 0)
        {
            for (int i = 1; i < BattleData.EnemyDataList.Count + 1; i++)
            {
                if (BattleData.EnemyDataList[i].position == Info.Selection[0])
                {
                    BattleData.EnemyData data = BattleData.EnemyDataList[i];
                    data.currentHealth -= 2;
                    BattleData.EnemyDataList[i] = data;
                    UI.UpdateEnemyData(i);
                    return;
                }
            }
        }
        else
        {
           

            if (BattleData.playerData.position == Info.Selection[0])
            {
                BattleData.playerData.currentHealth -= 2;
                UI.UpdatePlayerData();
            }
        }
    }
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
