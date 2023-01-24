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

    private void Awake()
    {

        //PlayerPrefs.DeleteAll();
/*      GameData.SaveCard(1, "RunUp");
        GameData.SaveCard(2, "Walk");
        GameData.SaveCard(3, "RunDown");
        GameData.SaveCard(4, "RunLeft");*/
        //GameData.SaveCardNumber(4);
        GameData.LoadCard();
    }
    private void Start()
    {
        for(int i = 0; i < positions.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, positions[i].transform);
            if (i == 0)
                newButton.GetComponent<LevelButton>().levelText.text = "Tutorial";
            else if(i % 2 == 0)
                newButton.GetComponent<LevelButton>().levelText.text = "Event";
            else 
                newButton.GetComponent<LevelButton>().levelText.text = "Level" + i.ToString();

            if(i <= GameData.accessible)
            {
                int x = i;
                newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevelScene(x));
            }
            
        }

        deleteButton = GameObject.Find("Delete");
        deleteButton.SetActive(false);
    }


    public void LoadLevelScene(int id)
    {
        Debug.Log("Load level" + id);
        currentLevelID = id;
        GameData.currentState = GameData.state.Battle;
        //load scene
        if (id == 0)
            SceneManager.LoadScene("Tutorial");
        else if (id % 2 == 0&& GameData.enteredEventLevel==false)
        {
            GameData.enteredEventLevel = true;
            SceneManager.LoadScene("EventLevel");        
        }
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
            //deleteButton.SetActive(false);
            for (int i = 0; i < deckGrid.transform.childCount; ++i)
            {
                Destroy(deckGrid.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            deckPanel.SetActive(true);

            Debug.Log(GameData.health);
            Debug.Log(GameData.Deck.Count);

            foreach (Card card in GameData.Deck)
            {
                GameObject obj = Instantiate(Resources.Load("Prefab/Card/" + card.Name) as GameObject, deckGrid.transform);
                obj.transform.localScale = new Vector3(50, 50, 0);
            }

        }
        
    }

    public static void DeleteCard(Card card)
    {
        deleteButton.SetActive(true);
        readyToDelete = card;
    }

    public void Delete()
    {
        //GameData.Deck.Remove(readyToDelete);
        GameData.DeleteCard(readyToDelete.Name);
        ShowDeck();
        ShowDeck();
    }

    public void StartMenu()
    {
        SceneManager.LoadScene("Start");
    }
}
