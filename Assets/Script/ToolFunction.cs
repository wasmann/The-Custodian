using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToolFunction : MonoBehaviour
{
    public static Tilemap map;
    public static Vector3 FromCoorinateToWorld(Vector2 cooridinate)
    {
        return map.GetCellCenterWorld(new Vector3Int((int)cooridinate.x, (int)cooridinate.y, 0));
    }
    private void Start()
    {
        map = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();

    }
}
