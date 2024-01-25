using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newSpawnerData", menuName = "Data/Dungeon/Painter")]
public class Data_DungeonPainter : ScriptableObject
{
    public TileBase floor;
    public TileBase empty;


    //[Header("Walls")]
    public TileBase floorTile, wallTop, wallSideRight, wallSiderLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight,
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft, wallTop_Two, wallTop_Three, wallTop_Four;

    [SerializeField] bool hasPaintDelay;
    [SerializeField] int paintDelay;

    public GameObject torch;
}
