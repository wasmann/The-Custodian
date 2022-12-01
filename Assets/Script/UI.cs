using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
   BattleData battleData;
   BattleLevelDriver battleLevelDriver;
   
   public void UpdateHandCard()// After drawing a new card, reorgnize the hand card(align right) and move a card from deck to hand at the most left side.
   {

   }

   public static void UpdateTimeLine()
   {

   }

    public void LoadSomethingWhenNewBattleLevelBegin()
   {

   }

   public void ShowNotation(GameObject notion,Vector2 characterpos)
   {
        // notion is used to show how is the range or attack damage of a card. For example move left can be arrow pointing left covering one grid.
        //this function will show the notation in the direction coresponding to the mouse and character position. For example if the mouse is at the top side of character, then the notion will placed at the top side of character.
   }

    public void MoveTimeLine(List<List<Card.InfoForActivate>> TimeLineSlots)
    {
        
    }
    public static void ShowDuplicationWin() { }
}
