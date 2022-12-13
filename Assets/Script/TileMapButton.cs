using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapButton : MonoBehaviour
{
    public static Tilemap map;
    private static Card card;
    private static bool Selectable = false;
    void Start()
    {
        map = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();

    }
    public static void MakeSelectable(Card currentCard)
    {
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
        if (Input.GetMouseButtonDown(0) && Selectable)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector2 ClickPos = new Vector2(map.WorldToCell(pos).x, map.WorldToCell(pos).y);

            if (card.Info.direction.Contains(ClickPos))
            {
                card.TargetNum--;
                card.Info.Selection.Add(ClickPos-BattleData.playerData.position);
            }
        }
    }
}
