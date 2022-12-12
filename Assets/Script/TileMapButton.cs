using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapButton : MonoBehaviour
{
    public Tilemap tilemap;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10)) ;
            Vector3Int CellPos = tilemap.WorldToCell(pos);
            Debug.Log(CellPos);
        }
    }
}
