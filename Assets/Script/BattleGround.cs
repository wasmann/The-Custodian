using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
//using static UnityEditor.PlayerSettings;

public class BattleGround : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    TileBase clickedTile;
    bool needToSelect;
    public GameObject notion;
    public Card card;
    public Vector2 position;

    public void ActionNeeded(GameObject notion, Card card, Vector2 position)
    {
        this.notion = notion;
        this.card = card;
        map.SetColor(new Vector3Int((int)position.x, (int)position.y, 0), new Color(1, 0, 0, 0.5f));
        foreach (var pos in card.Info.direction)
        {
            map.SetColor(new Vector3Int((int)pos.x, (int)pos.y, 0), new Color(1, 0, 0, 0.2f));
        }
        needToSelect = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && needToSelect == true)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = map.WorldToCell(mousePosition);
            clickedTile = map.GetTile(cellPosition);
            position = new Vector2(cellPosition.x, cellPosition.y);
            if (card.Info.direction.Contains(position)) {
                card.Info.direction.Clear();
                card.Info.direction.Add(position);
                notion.SetActive(false);
                map.ClearAllTiles();
            }
        }
    }
}
