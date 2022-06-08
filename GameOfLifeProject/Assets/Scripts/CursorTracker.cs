using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorTracker : MonoBehaviour
{
    Tilemap tilemap;
    private Grid grid;

    Vector3 currentTileCoord;

    public Transform selectionCube;

    private Vector3Int previousMousePos = new Vector3Int();

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponentInParent<Grid>();
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse over -> highlight tile
        Vector3Int mousePos = GetMousePosition();
        if (!mousePos.Equals(previousMousePos))
        {
        //    tilemap.SetTile(previousMousePos, null); // Remove old hoverTile
        //    //tilemap.SetTile(mousePos, hoverTile);
            previousMousePos = mousePos;
        }

        // Left mouse click -> add path tile
        if (Input.GetMouseButton(0))
        {
            tilemap.SetTile(mousePos, null);
        }

        // Right mouse click -> remove path tile
        //if (Input.GetMouseButton(1))
        //{
        //    pathMap.SetTile(mousePos, null);
        //}
    }

    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        return grid.WorldToCell(mouseWorldPos);
    }

}
