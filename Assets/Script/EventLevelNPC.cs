using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EventLevelNPC : MonoBehaviour
{

    [SerializeField]
    GameObject dialogName;

    [SerializeField]
    GameObject dialogText;

    [SerializeField]
    GameObject continueButton;

    [SerializeField]
    GameObject optionGroup;

    [SerializeField]
    GameObject over;

    private void Start()
    {
        if (!GameData.enteredEventLevel)
            GameData.enteredEventLevel = true;
        else
            GameData.upgraded = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        dialogName.SetActive(true);
        dialogText.SetActive(true);
        continueButton.SetActive(true);
        //InputSystem.DisableDevice(Keyboard.current);
        GameObject.Find("Custodian").GetComponent<PlayerMovement>().Trigger();
    }

    public void Upgrade()
    {
        optionGroup.SetActive(true);
        continueButton.SetActive(false);
        //Debug.Log(GameData.Deck.Count);
    }

    public void Armor()
    {
        GameData.health += 5;
        GameData.currentState = GameData.state.WorldMap;
        SceneManager.LoadScene("WorldMap");
    }

    public void Energy()
    {
        GameData.Energy += 3;
        GameData.currentState = GameData.state.WorldMap;
        SceneManager.LoadScene("WorldMap");
    }

    public void GetCard()
    {
        int id = UnityEngine.Random.Range(8, 23);
        string name = "";
        switch (id)
        {
            case 8:
                name = "Photovoltaics";
                break;
            case 9:
                name = "Sleep";
                break;
            case 10:
                name = "Sleep";
                break;
            case 11:
                name = "Jump";
                break;
            case 12:
                name = "DoubleJump";
                break;
            case 13:
                name = "CryoliquidAttack";
                break;
            case 14:
                name = "CryoliquidShooting";
                break;
            case 15:
                name = "Bark";
                break;
            case 16:
                name = "RushAndBiteAttack";
                break;
            case 17:
                name = "EscapeInstinct";
                break;
            case 18:
                name = "GunShoot";
                break;
            case 19:
                name = "GasDash";
                break;
            case 20:
                name = "Reloading";
                break;
            case 21:
                name = "StormStrike";
                break;
            case 22:
                name = "ReverseElectrodes";
                break;
            case 23:
                name = "ElectricalShield";
                break;

        }
       
        GameObject obj = Instantiate(Resources.Load("Prefab/Card/" + name) as GameObject);
        obj.transform.localScale = new Vector3(1f, 1f, 0);

        /*Type t = Type.GetType(name);
        GameData.Deck.Add((Card)obj.GetComponent(t));*/

        GameData.SaveCard(GameData.GetCardNumber()+1, name);

        obj.transform.position = GameObject.Find("Custodian").transform.position;
        optionGroup.SetActive(false);
        over.SetActive(true);
    }

    public void Next()
    {
        GameData.currentState = GameData.state.WorldMap;
        SceneManager.LoadScene("WorldMap");
    }
}
