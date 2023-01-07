using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeInstinct : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "EscapeInstinct"; } }
    public override Rarity rarity { get { return Rarity.legendary   ; } }
    public override int Speed { get { return 1; } }
    public override int ID { get { return 17; } }
    public override IEnumerator Play()
    {
        BattleData.playerData.currentEnergy -= 1;
        UI.UpdatePlayerData();
        Info.direction.Add(BattleData.playerData.position + new Vector2(1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(-1, 0));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, 1));
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
        int random = (int)Random.Range(1, 3);
        Info.Selection[0] *=random;
        StartCoroutine(Animate(Info.animator));
        if (Info.owner_ID == 0)
        {
            BattleData.playerData.position += Info.Selection[0];
            UI.UpdatePlayerData();
        }
        else
        {
            Audio.Play();
            BattleData.EnemyData newData = BattleData.EnemyDataList[Info.owner_ID];
            newData.position += Info.Selection[0];
            BattleData.EnemyDataList[Info.owner_ID] = newData;
            UI.UpdateEnemyData(Info.owner_ID);
        }
    }
    public IEnumerator Animate(Animator animator)
    {
        animator.SetBool("Walk", true);
        yield return new WaitForSeconds(0);
        animator.SetBool("Walk", false);
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
