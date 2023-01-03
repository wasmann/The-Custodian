using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    
    
    public TextAsset dialogDataFile;

    public SpriteRenderer spriteLeft;
    public SpriteRenderer spriteRight;

    public TMP_Text nameText;
    public TMP_Text dialogText;

    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    Dictionary<string, SpriteRenderer> imageDic = new Dictionary<string, SpriteRenderer>();

    public int dialogIndex;
    public string[] dialogRows;

    public Button nextButton;
    public GameObject optionButton;
    public Transform optionGroup;

    private void Awake()
    {
        imageDic["Custodian"] = sprites[0];
        imageDic["dog"] = sprites[1];
        imageDic["GoodLabWorker"] = sprites[2];
        imageDic["BadLabWorker"] = sprites[3];
    }
    // Start is called before the first frame update
    void Start()
    {
        ReadText(dialogDataFile);
        ShowDialogRow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
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
/*        foreach (var row in rows)
        {
            string[] cell = row.Split(';');
        }
              */  
    }

    public void ShowDialogRow()
    {
        for(int i = 0; i < dialogRows.Length; ++i)
        {
            string[] cells = dialogRows[i].Split(',');
            if(cells[0]=="#" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2], cells[4]);
                UpdateImage(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[5]);
                nextButton.gameObject.SetActive(true);
                break;
            
            //different branch
            }
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                nextButton.gameObject.SetActive(false);
                GenerateOption(i);
            }
            else if(cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
            {

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
            button.GetComponent<Button>().onClick.AddListener(delegate {
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

    public void OptionEffect(string _effect)
    {

    }
}
