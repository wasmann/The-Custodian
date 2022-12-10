using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMap : MonoBehaviour
{
    public int currentLevelID;
    public BattleLevelDriver battleLevelDriver;

    [SerializeField]
    Button[] levels;

    private void Start()
    {
        for(int i = 0; i < levels.Length; i++)
        {
            levels[i].onClick.AddListener(() => LoadLevelScene(i + 1));
        }
    }

    public void LoadLevelScene(int id)
    {
        currentLevelID = id;
        battleLevelDriver.BeginABattleLevel(id);
        battleLevelDriver.Paused = true;
        GameData.currentState = GameData.state.Battle;
        //load scene
    }


    // all the levels are a button attached as children of worldmap obj
    // when clicking the button of level, call loadscene to that level.
    // Each level is paused at the first, player should click the start button to call BattleLevelDriver.BeginABattleLevel(levelID)

}
