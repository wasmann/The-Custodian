using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapButton : MonoBehaviour
{
    public static Tilemap map;
    private static Card card;
    private static bool Selectable = false;
    public static GameObject selectionNotation;

    void Start()
    {
        map = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();

    }
    public static void MakeSelectable(Card currentCard)
    {
        if (currentCard.SelectionNotation != "None")
        {
            selectionNotation = GameObject.Instantiate(Resources.Load("Prefab/Notation/" + currentCard.SelectionNotation) as GameObject);
            selectionNotation.SetActive(false) ;
        }
        card = currentCard;
        Selectable = true;
    }

    public static void MakeUnSelectable()
    {
        Selectable = false;
        UI.DeleteNotation();
    }
    void Update()
    {
        if (Selectable)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector2 ClickPos = new Vector2(map.WorldToCell(pos).x, map.WorldToCell(pos).y);
            //show selection Notation
            if (card.Info.direction.Contains(ClickPos)) { 
                selectionNotation.SetActive(true);
                selectionNotation.transform.position = ToolFunction.FromCoorinateToWorld(ClickPos);
                selectionNotation.transform.SetParent(UI.NotationList.transform,false);
                if (Input.GetMouseButtonDown(0))
                {
                    card.TargetNum--;
                    card.Info.Selection.Add(ClickPos - BattleData.playerData.position);
                    GameObject.Destroy(selectionNotation);
                }
            }
            else
            {
                selectionNotation.SetActive(false);
            }
        }
    }
}
