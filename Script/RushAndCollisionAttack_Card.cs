using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushAndCollisionAttack_Card : Card
{
    public RushAndCollisionAttack_Card(BattleData battleData)
    {
        this.name = "Rush and Collision Attack";
        this.rarity = .basic;
        this.speed = 5;
        this.range = 3; //initialize with maximum number, player can choose to play less after show notation
        this.battleCata = battleData;

    }

    public override void IsPlayed()
    {
        this.Info.card = this;
        this.Info.player_ID = 0;
        this.battleData.battleData.playerData.energy -= 1;
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
        Instantiate(Notation);
    }

    public override void Acitvate(InfoForActivate info)
    {
        this.Info = info;
        this.BattleData.battleData.EnemyDataList.at(0).health -= 2 + (this.BattleData.battleData.playerData.position - info.direction.at(0)).Length();
        this.BattleData.battleData.playerData.position = info.Direction.at(0);
    }
}
