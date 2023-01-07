using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunDown : Card
{
    [SerializeField] private AudioSource Audio;
    public override string Name { get { return "RunDown"; } }
    public override Rarity rarity { get { return Rarity.basic; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 3; } }
    public override IEnumerator Play()
    {
        //here need more implementation about "not allowing to walk on occupied grid"
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -1));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -2));
        Info.direction.Add(BattleData.playerData.position + new Vector2(0, -3));
        yield return new WaitForSeconds(0.1f);
        UI.ShowNotation(this);
        TileMapButton.MakeSelectable(this);
        yield return new WaitUntil(() => TargetNum == 0);
        TileMapButton.MakeUnSelectable();
       // BattleData.PlayingACard = false;
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        Audio.Play();
        StartCoroutine(Animate(Info.animator));
        if (Info.owner_ID == 0)
        {
            BattleData.playerData.position += Info.Selection[0];
            UI.UpdatePlayerData();
        }
        else
        {
            BattleData.EnemyData newData = BattleData.EnemyDataList[Info.owner_ID];
            newData.position += Info.Selection[0];
            BattleData.EnemyDataList[Info.owner_ID] = newData;
            UI.UpdateEnemyData(Info.owner_ID);
        }
    }
    private IEnumerator Animate(Animator animator)
    {
        animator.SetBool("Run", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("Run", false);

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

