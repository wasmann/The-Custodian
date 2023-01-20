using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMap : MonoBehaviour
{
    public int currentLevelID;
    public BattleLevelDriver battleLevelDriver;
    public GameObject[] positions;
    public GameObject buttonPrefab;
    public GameObject buttonParent;

    [SerializeField]
    public GameObject deckPanel;

    [SerializeField]
    public GameObject deckGrid;

    public static GameObject deleteButton;

    public static Card readyToDelete;


    private void Start()
    {
        for(int i = 0; i < positions.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, positions[i].transform);
            if (i == 0)
                newButton.GetComponent<LevelButton>().levelText.text = "Tutorial";
            else
                newButton.GetComponent<LevelButton>().levelText.text = "Level" + i.ToString();
            int x = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevelScene(x));
        }

        deleteButton = GameObject.Find("Delete");
    }


    public void LoadLevelScene(int id)
    {
        Debug.Log("Load level" + id);
        currentLevelID = id;
        GameData.currentState = GameData.state.Battle;
        //load scene
        if(id == 0)
            SceneManager.LoadScene("Tutorial");
        else
            SceneManager.LoadScene("Level" + id.ToString());
    }


    // all the levels are a button attached as children of worldmap obj
    // when clicking the button of level, call loadscene to that level.
    // Each level is paused at the first, player should click the start button to call BattleLevelDriver.BeginABattleLevel(levelID)

    public void ShowDeck()
    {
        if (deckPanel.activeInHierarchy)
        {
            deckPanel.SetActive(false);
            deleteButton.SetActive(false);
            for (int i = 0; i < deckPanel.transform.childCount; ++i)
            {
                Destroy(deckPanel.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            deckPanel.SetActive(true);

            /*            foreach (Card card in GameData.Deck)
                        {
                            GameObject obj = Instantiate(Resources.Load("Prefab/UI/Damage") as GameObject, deckGrid.transform);
                            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                        }*/

            GameObject obj = Instantiate(Resources.Load("Prefab/Card/Walk") as GameObject, deckGrid.transform);
            obj.transform.localScale = new Vector3(50, 50, 0);

            GameObject obj2 = Instantiate(Resources.Load("Prefab/Card/RunUp") as GameObject, deckGrid.transform);
            obj2.transform.localScale = new Vector3(50, 50, 0);

            GameObject obj3 = Instantiate(Resources.Load("Prefab/Card/RunDown") as GameObject, deckGrid.transform);
            obj3.transform.localScale = new Vector3(50, 50, 0);

            GameObject obj4 = Instantiate(Resources.Load("Prefab/Card/RunLeft") as GameObject, deckGrid.transform);
            obj4.transform.localScale = new Vector3(50, 50, 0);

            GameObject obj5 = Instantiate(Resources.Load("Prefab/Card/Headbutt") as GameObject, deckGrid.transform);
            obj5.transform.localScale = new Vector3(50, 50, 0);
        }
        
    }

    public static void DeleteCard(Card card)
    {
        deleteButton.SetActive(true);
        readyToDelete = card;
    }

    public static void Delete()
    {
        GameData.Deck.Remove(readyToDelete);
    }
}
