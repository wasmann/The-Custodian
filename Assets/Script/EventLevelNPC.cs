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
    }

    public void Armor()
    {
        GameData.health += 5;
        SceneManager.LoadScene("WorldMap");
    }

    public void Energy()
    {
        GameData.Energy += 3;
        SceneManager.LoadScene("WorldMap");
    }

    public void GetCard()
    {
        SceneManager.LoadScene("WorldMap");
    }
}
