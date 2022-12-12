using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMap : MonoBehaviour
{
    public int currentLevelID;
    public BattleLevelDriver battleLevelDriver;
    public GameObject[] positions;
    public GameObject buttonPrefab;
    public GameObject buttonParent;

    private void Start()
    {
        for(int i = 0; i < positions.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, positions[i].transform);
            if (i == 0)
                newButton.GetComponent<LevelButton>().levelText.text = "Tutorial";
            else
                newButton.GetComponent<LevelButton>().levelText.text = "Level" + i.ToString();
            newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevelScene(i));
        }
    }


    public void LoadLevelScene(int id)
    {
        Debug.Log("Load level" + id);
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
