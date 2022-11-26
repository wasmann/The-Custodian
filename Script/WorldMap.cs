using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    int currentLevelID;
    // all the levels are a button attached as children of worldmap obj
    // when clicking the button of level, call loadscene to that level.
    // Each level is paused at the first, player should click the start button to call BattleLevelDriver.BeginABattleLevel(levelID)

}
