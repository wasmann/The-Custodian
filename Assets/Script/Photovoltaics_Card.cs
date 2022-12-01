using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photovoltaics_Card : Card
{
    public Photovoltaics_Card(BattleData battleData)
    {
        this.Name = "Photovoltaics";
        this.Rarity = .basic;
        this.Speed = 3;
        this.Range = 0;
        this.BattleCata = battleData;

    }
    public override void IsPlayed()
    {
        this.Info.card = this;
        this.Info.player_ID = 0;
        Instantiate(Notation);
    }

    public override void Acitvate(InfoForActivate info)
    {
        this.Info = info;
        this.BattleData.battleData.playerData.energy += 1;
        this.BattleData.battleData.playerData.health += 2;
    }
}
