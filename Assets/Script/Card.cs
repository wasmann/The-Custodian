
using System;
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
    public string Effect;

    public InfoForActivate Info;

    public enum Rarity
    {
        trash,
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
        public Animator animator;
        public int animationSeconds;
        public String animationParameter;
    }

    public abstract IEnumerator Play();
   
    public void UpdateData(int OwnerID,int CardID,InfoForActivate info)
    {
        if(OwnerID == 0)
        {
            BattleData.playerData.handCard.Remove(this);
            BattleData.playerData.discardPile.Add(this);// need test
            BattleLevelDriver.NewCardPlayed(this.Info);
            Deck.DrawCard();
            if (this.transform.childCount == 1)
                Destroy(this.transform.GetChild(0).gameObject);

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
    Coroutine playcoroutine = null;
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameData.currentState == GameData.state.WorldMap)
            {
                if(this.transform.childCount == 0)
                {
                    WorldMap.DeleteAllBorder();
                    GameObject obj = Instantiate(Resources.Load("Prefab/UI/Border") as GameObject, this.transform);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder;
                    WorldMap.DeleteCard(this);
                }
               
            }
            else
            {
                if (UI.waitForDuplicate)
                {
                    if (this.transform.parent.name == "DuplicationGrid")
                    {
                        UI.FinishDuplicate(this, this.rarity);
                    }

                }
                else
                {
                    if (this.transform.childCount == 0 && BattleData.AbleToPalyCard)
                    {
                        GameObject obj = Instantiate(Resources.Load("Prefab/UI/Border") as GameObject, this.transform);
                        obj.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder;
                    }
                        
                    if (BattleData.AbleToPalyCard == true)
                    {
                        ReSetTarget();
                        // BattleData.PlayingACard = true;
                        BattleData.AbleToPalyCard = false;
                        Info = new InfoForActivate();
                        Info.owner_ID = 0;
                        Info.animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
                        Info.direction = new List<Vector2>();
                        Info.Selection = new List<Vector2>();
                        Info.card = this;
                        playcoroutine=StartCoroutine(Play());
                    }
                    else
                    {
                        Debug.Log("Another card is now ready to play");
                    }
                }

            }
        }
        else if (Input.GetMouseButtonDown(1))//Right click
        {
            if (this.transform.childCount == 1)
                Destroy(this.transform.GetChild(0).gameObject);

            if(GameData.currentState == GameData.state.WorldMap)
            {
                WorldMap.Unselect();
            }
            else if (BattleData.AbleToPalyCard == false)
            {
                TileMapButton.MakeUnSelectable();
                BattleData.AbleToPalyCard = true;
                StopCoroutine(playcoroutine);
            }
        }
    }

    void OnMouseEnter()
    {
        this.transform.localScale *= 2;
        this.transform.position = this.transform.position + new Vector3(0, this.GetComponent<SpriteRenderer>().bounds.size.y/4, 0);
        this.GetComponent<SpriteRenderer>().sortingOrder = 4;
    }
    void OnMouseExit()
    {
        
        this.transform.localScale /= 2;
        this.transform.position = this.transform.position - new Vector3(0, this.GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }
}