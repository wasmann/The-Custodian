using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public abstract string EnemyName { get; }
    public abstract List<Card> CardsDeck { get; }
    public abstract int Health { get; }
    public abstract int AttackMaxRange { get; }
    public abstract int HandCardNum { get; }
    public int EnemyID;

    public Discard dicardManager;
    public abstract void EnemyChooseACardToPlay(); 

    public void UpdatePiles(Card card)
    {
        BattleData.EnemyData data = BattleData.EnemyDataList[EnemyID];
        data.handCard.Remove(card);
        data.discardPile.Add(card);

        if (data.drawPile.Count== 0)
        {
            data.drawPile = data.discardPile;
            data.discardPile= new List<Card>();
        }
        data.handCard.Add(data.drawPile[0]);
        data.drawPile.RemoveAt(0);
    }

    private void Start()
    {
        dicardManager = GameObject.Find("GameManager").GetComponent<Discard>();
    }

}
