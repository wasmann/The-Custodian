using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grids : MonoBehaviour
{
    public int x;
    public int y;
    public Vector2 pos;
    public Button button;
    public GameObject selectionNotion;
    public Card Targetcard;

    void start()
    {
        button = this.GetComponent<Button>();
        pos = new Vector2(x, y);
    }

    public void Selectable(GameObject SelectionNotion, Card card)
    {
        selectionNotion = SelectionNotion;
        Targetcard = card;
        button.onClick.AddListener(() => OnMouseEnter());
        
    }

    public void NotSelectable()
    {
        Targetcard = null;
        selectionNotion = null;
        button.onClick.RemoveAllListeners();
    }

    public void Select()
    {
        Targetcard.TargetNum--;
        NotSelectable();
    }

    public void OnMouseEnter()
    {
        selectionNotion.transform.position = this.transform.position;
        selectionNotion.SetActive(true);
    }
    public void OnMouseExit()
    {
        selectionNotion.SetActive(false);
    }
}
