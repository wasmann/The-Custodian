using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run_Card : Card
{
    public Run_Card(BattleData battleData)
    {
        this.Name = "Run";
        this.rarity = Rarity.basic;
        this.Speed = 3;
        this.Range = 3;
        this.battleCata = battleData;
        this.Notation = gameObject.transform.Find("notion").gameObject;

    }
    public override void IsPlayed()
    {
        this.Info.card = this;
        this.Info.player_ID = 0;
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(1, 0)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(-1, 0)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(0, 1)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(0, -1)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(2, 0)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(-2, 0)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(0, 2)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(0, -2)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(3, 0)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(-3, 0)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(0, 3)));
        this.Info.direction.Add(Vector2(BattleData.battleData.player.position + Vector2(0, -3)));
        GameObject notion=
        UI.ShowNotation(Notation, B)
    }

    public override void Acitvate(InfoForActivate info)
    {
        this.Info = info;
        this.BattleData.battleData.playerData.position = info.direction.at(0);
    }
}
