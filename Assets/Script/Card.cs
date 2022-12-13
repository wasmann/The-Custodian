
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    //every card is a prefab and has its own script named card_cardname and this one is a superclass
    public abstract int ID { get; }
    public abstract string Name { get; }
    public abstract Rarity rarity { get; }
    public abstract int Speed { get; }

    public int TargetNum;
    public string RangeNotation;
    public string SelectionNotation;

    public InfoForActivate Info;

    public enum Rarity
    {
        basic,
        common,
        rare,
        epic,
        legendary,
    }

    public struct InfoForActivate
    {
        public List<Vector2> direction;
        public int owner_ID;// 0 for player
        public Card card;
        public List<Vector2> Selection;
        public List<string> otherInfo;
    }

    public abstract IEnumerator Play();
   
    public void UpdateData(int OwnerID,int CardID,InfoForActivate info)
    {
        if(OwnerID == 0)
        {
            BattleData.playerData.handCard.Remove(this);
            BattleData.playerData.discardPile.Add(this);// need test
            UI.UpdateHandCard();
            BattleLevelDriver.NewCardPlayed(this.Info);
        }
        else
        {
            BattleData.EnemyDataList[OwnerID].handCard.Remove(this);
            BattleData.EnemyDataList[OwnerID].discardPile.Add(this);
            BattleLevelDriver.NewCardPlayed(info);
        }
        
    }

    public abstract void Activate(InfoForActivate Info);
    public abstract void ReSetTarget();

    public static bool Contain(List<Card> cards,int ID)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            if(cards[i].ID == ID)
                return true;
        }
        return false;
    }

    void OnMouseDown()
    {
        if (BattleData.AbleToPalyCard == true)
        {
            BattleData.PlayingACard = true;
            BattleData.AbleToPalyCard = false;
            Info = new InfoForActivate();
            Info.owner_ID = 0;
            Info.direction = new List<Vector2>();
            Info.Selection = new List<Vector2>();
            Info.card = this;
            StartCoroutine(Play());
        }
        else
        {
            Debug.Log("Another card is now ready to play");
        }
    }


}