using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    
    
    public TextAsset dialogDataFile;

    public SpriteRenderer spriteLeft;
    public SpriteRenderer spriteRight;

    public GameObject dialogName;
    public GameObject dialog;

    public TMP_Text nameText;
    public TMP_Text dialogText;

    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    Dictionary<string, SpriteRenderer> imageDic = new Dictionary<string, SpriteRenderer>();

    public int dialogIndex;
    public string[] dialogRows;

    public Button nextButton;
    public GameObject optionButton;
    public Transform optionGroup;

    public PlayableDirector director;
    //public TimelineAsset tutorial;

    //UI
    public GameObject armor;
    public GameObject energy;
    public GameObject attribute;
    public GameObject timeline;
    public GameObject pausebutton;
    public GameObject card;
    public GameObject panel;
    public GameObject headbutt;

    private void Awake()
    {
        //Time.timeScale = 0;
        //imageDic["Custodian"] = sprites[0];
        //imageDic["dog"] = sprites[1];

        PlayerPrefs.DeleteAll();
        GameData.SaveCard(1, "RunUp");
        GameData.SaveCard(2, "Walk");
        GameData.SaveCard(3, "RunDown");
        GameData.SaveCard(4, "RunLeft");
    }
    // Start is called before the first frame update
    void Start()
    {
        director = GetComponent<PlayableDirector>();
        ReadText(dialogDataFile);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string _name, string _text)
    {
        nameText.text = "Custodian";
        dialogText.text = _text;
    }

    public void UpdateImage(string _name, string _position)
    {
        if (_position == "left")
        {
            spriteLeft = imageDic[_name];
        }
        else
        {
            spriteRight = imageDic[_name];
        }
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n');
    }

    public void ShowDialogRow()
    {
        for(int i = 0; i < dialogRows.Length; ++i)
        {
            string[] cells = dialogRows[i].Split(',');
            if(cells[0]=="#" && int.Parse(cells[1]) == dialogIndex)
            {
                EnableDialog();
                UpdateText(cells[2], cells[4]);
                //UpdateImage(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[5]);
                nextButton.gameObject.SetActive(true);
                break;
            
            //different branch
            }
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                EnableDialog();
                nextButton.gameObject.SetActive(false);
                GenerateOption(i);
            }
            else if(cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
            {
                SceneManager.LoadScene("Level1");
            }
            else if(cells[0] == "*" && int.Parse(cells[1]) == dialogIndex)
            {
                EnableDialog();
                UpdateText(cells[2], cells[4]);

                dialogIndex = int.Parse(cells[5]);
                nextButton.gameObject.SetActive(true);
                OptionEffect(int.Parse(cells[6]));
                break;
                
            }
            else if(cells[0] == "@" && int.Parse(cells[1]) == dialogIndex)
            {
                
                DisableDialog();
                OptionEffect(int.Parse(cells[6]));
                dialogIndex = int.Parse(cells[5]);
                break;
            }
        }
    }

    public void OnClickNext()
    {
        ShowDialogRow();
    }

    public void GenerateOption(int _index)
    {
        //recursive options
        string[] cells = dialogRows[_index].Split(',');
        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, optionGroup);
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnOptionClick(int.Parse(cells[5]));
            });
            GenerateOption(_index + 1);
        }     
    }

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDialogRow();
        for(int i = 0; i < optionGroup.childCount; ++i)
        {
            Destroy(optionGroup.GetChild(i).gameObject);
        }
    }

    public void OptionEffect(int _effect)
    {
        switch (_effect)
        {
            case 1:
                armor.SetActive(true);
                energy.SetActive(true);
                attribute.SetActive(true);          
                break;

            case 2:
                card.SetActive(true);
                break;

            case 3:
                DisableDialog();
                
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                break;

            case 4:
                timeline.SetActive(true);
                break;

            case 5:
                DisableDialog();
                card.SetActive(false);
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                break;

            case 6:
                //DisableDialog();
                nextButton.gameObject.SetActive(false);
                Camera.main.transform.Translate(5, 0, 0);
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                break;

            case 7:
                SceneManager.LoadScene("Level1");
                break;

            case 8:
                DisableDialog();
                GameData.SaveCard(5, "Headbutt");
                dialogName.SetActive(false);
                dialog.SetActive(false);
                nextButton.gameObject.SetActive(false);
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                break;

            case 9:
                nextButton.gameObject.SetActive(false);
                pausebutton.SetActive(true);
                break;

            case 10:
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                break;
        }
    }

    public void pauseDirector()
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        ShowDialogRow();
    }

    public void EnableDialog()
    {
        dialogName.SetActive(true);
        dialog.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    public void DisableDialog()
    {
        dialogName.SetActive(false);
        dialog.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    public void OnClickPauseButton()
    {
        panel.SetActive(true);
        headbutt.SetActive(true);
        ShowDialogRow();
        pausebutton.SetActive(false);
    }

    public void Skip()
    {
        GameData.SaveCard(5, "Headbutt");
        SceneManager.LoadScene("Level1");
    }
}
