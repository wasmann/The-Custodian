using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAndDebuff : MonoBehaviour
{
   public struct Buff
   {
        public bool sleep;
        public int Frozen;
        public bool Fear;
        public bool Courage;
        public int Shield;
        public bool Bullet;
        public int ReverseEle;
   }

   public static Buff CreateANewState()
   {
        Buff returnValue = new Buff();
        returnValue.sleep = false;
        returnValue.Frozen = 0;
        returnValue.Fear = false;
        returnValue.Courage = false;
        returnValue.Shield = 0;
        returnValue.ReverseEle = 0;
        returnValue.Bullet = true;
        return returnValue;
   }

   public void AwakeByAttack(int ID)
   {
        if (ID == 0)//PLAYER
        {
            BattleData.playerData.buff.sleep = false;
            for(int i = 0; i < BattleLevelDriver.TimeLineSlots.Count; i++)
            {
                for(int j = 0; j < BattleLevelDriver.TimeLineSlots[i].Count;)
                {
                    if (BattleLevelDriver.TimeLineSlots[i][j].card.Name=="Sleep"&& BattleLevelDriver.TimeLineSlots[i][j].owner_ID == 0)
                    {
                        BattleLevelDriver.TimeLineSlots[i][j].card.Activate(BattleLevelDriver.TimeLineSlots[i][j]);
                        BattleData.AbleToPalyCard = true;
                        BattleLevelDriver.TimeLineSlots[i].RemoveAt(j);
                        UI.UpdateTimeLine(BattleLevelDriver.TimeLineSlots);
                        return;
                    }
                }
            }
        }
        else //Other Enemy
        {
            BattleData.EnemyData newdata = new BattleData.EnemyData();
            newdata = BattleData.EnemyDataList[ID];
            newdata.buff.sleep=false;
            BattleData.EnemyDataList[ID]= newdata;
            for (int i = 0; i < BattleLevelDriver.TimeLineSlots.Count; i++)
            {
                for (int j = 0; j < BattleLevelDriver.TimeLineSlots[i].Count;)
                {
                    if (BattleLevelDriver.TimeLineSlots[i][j].card.Name == "Sleep" && BattleLevelDriver.TimeLineSlots[i][j].owner_ID == ID)
                    {
                        BattleLevelDriver.TimeLineSlots[i][j].card.Activate(BattleLevelDriver.TimeLineSlots[i][j]);
                        BattleLevelDriver.TimeLineSlots[i].RemoveAt(j);
                        BattleData.NewCard.Add(BattleLevelDriver.TimeLineSlots[i][j].card);
                        BattleData.EnemyDataList[BattleLevelDriver.TimeLineSlots[i][j].owner_ID].enemy.EnemyChooseACardToPlay();
                        UI.UpdateTimeLine(BattleLevelDriver.TimeLineSlots);
                        return;
                    }
                }
            }
        }
    }
}
