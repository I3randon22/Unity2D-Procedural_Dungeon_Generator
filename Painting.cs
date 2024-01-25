using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;

public class Painting : Dungeon
{
    [SerializeField] bool hasPaintDelay;
    [SerializeField] int paintDelay;
    public List<Vector2Int> topFloors;
    GameObject torchObj;

    public Painting(Grid grid, RandomWalk randomWalk, DungeonCorridors corridor, Tilemap tileMap, Tilemap tileMapCollider, Tilemap tileMapColliderHalf, Data_DungeonPainter dataPainter) 
    {
        this.grid = grid;
        this.corridor = corridor;
        this.tileMap = tileMap;
        this.tileMapCollider = tileMapCollider;
        this.tileMapColliderHalf = tileMapColliderHalf;
        this.dataPainter = dataPainter;
        this.randomWalk = randomWalk;
        topFloors = new List<Vector2Int>();
    }

    public void Paint(Vector2Int pos, Side side)
    {
        var tilePos = tileMap.LocalToCell((Vector3Int)pos);
      
        switch(side)
        {
            case Side.Up:
                //tileMap.SetTile(tilePos, dataPainter.wallUp);
                break;
            default:
                tileMap.SetTile(tilePos, dataPainter.floor);
                break;
        }

    }

    public override void PaintDungeon()
    {
        if (hasPaintDelay)
        {
            StartCoroutine(PaintDelay(paintDelay));
        }
        else
        {
            for (int i = 0; i < grid.gridAmount; i++)
            {
                //Paint floors
                foreach (var position in grid.floorPositions[i])
                {
                    Paint(position, 0);
                }
            }
        }

    }

    IEnumerator PaintDelay(float timer)
    {
        for (int i = 0; i < grid.gridAmount; i++)
        {
            foreach (Node node in grid.grid[i])
            {
                PaintTile(node.side, node.position);
                yield return new WaitForSeconds(timer);
            }
        }

    }

   
    internal void PaintSingleBasicWall(Vector2Int pos, string binaryType)
    {
        Debug.Log("Tilemap: " + tileMap);
        
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        var tilePos = tileMap.LocalToCell((Vector3Int)pos);
        TileBase tile = null;
        bool half = false;

        if (WallBinaryTypes.wallTop.Contains(typeAsInt))
        {
            int index = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 9f));

            switch(index)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    tile = dataPainter.wallTop;
                    break;
                case 6:
                    tile = dataPainter.wallTop_Two;
                    break;
                case 7:
                    tile = dataPainter.wallTop_Three;
                    break;
                case 8:
                    tile = dataPainter.wallTop_Four;
                    break;
                case 9:
                    tile = dataPainter.wallTop;
                    torchObj = Spawn(dataPainter.torch, new Vector2(pos.x, pos.y));
                    break;
                default:
                    tile = dataPainter.wallTop;
                    break;
            }

            topFloors.Add(pos + Vector2Int.down);
        }
        else if (WallBinaryTypes.wallSideRight.Contains(typeAsInt))
        {
            tile = dataPainter.wallSideRight;
        }
        else if (WallBinaryTypes.wallSideLeft.Contains(typeAsInt))
        {
            tile = dataPainter.wallSiderLeft;
        }
        else if (WallBinaryTypes.wallBottm.Contains(typeAsInt))
        {
            tile = dataPainter.wallBottom;
            tileMapColliderHalf.SetTile(tilePos, dataPainter.empty);
            half = true;
        }
        else if (WallBinaryTypes.wallFull.Contains(typeAsInt))
        {
            tile = dataPainter.floor;
        }

        if (tile != null)
        {
            if(tile != dataPainter.floor)
            {
                if (!half) tileMapCollider.SetTile(tilePos, dataPainter.empty);
                else
                {
                    tileMapCollider.SetTile(tilePos, null);
                    tileMapColliderHalf.SetTile(tilePos, dataPainter.empty);
                }
            }
          
            tileMap.SetTile(tilePos, tile);
        }
          
    }

    internal void PaintSingleCornerWall(Vector2Int pos, string binaryType)
    {
        int typeASInt = Convert.ToInt32(binaryType, 2);
        var tilePos = base.tileMap.LocalToCell((Vector3Int)pos);
        bool half = false;
        TileBase tile = null;

        if (WallBinaryTypes.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tile = dataPainter.wallInnerCornerDownLeft;
            half = true;
        }
        else if (WallBinaryTypes.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tile = dataPainter.wallInnerCornerDownRight;
            half = true;
        }
        else if (WallBinaryTypes.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = dataPainter.wallDiagonalCornerDownLeft;
        }
        else if (WallBinaryTypes.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = dataPainter.wallDiagonalCornerDownRight;
        }
        else if (WallBinaryTypes.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tile = dataPainter.wallDiagonalCornerUpRight;
        }
        else if (WallBinaryTypes.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tile = dataPainter.wallDiagonalCornerUpLeft;
        }
        else if (WallBinaryTypes.wallFullEightDirections.Contains(typeASInt))
        {
            tile = dataPainter.wallFull;
        }
        else if (WallBinaryTypes.wallBottmEightDirections.Contains(typeASInt))
        {
            tile = dataPainter.wallBottom;
            half = true;
        }

        if (tile != null)
        {
            if (half)
            {
                tileMapCollider.SetTile(tilePos, null);
                tileMapColliderHalf.SetTile(tilePos, tile);
            }
            else
            {
                tileMapCollider.SetTile(tilePos, tile);
            }

            tileMap.SetTile(tilePos, tile);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 0.1f, 0);


        foreach (Collider2D collider in colliders)
        {
            Destroy(collider.gameObject);
        }
    }

    public void DeleteTiles()
    {
        tileMap.ClearAllTiles();
    }

    public void PaintTile(Side side, Vector2Int pos)
    {
        var tilePos = tileMap.LocalToCell((Vector3Int)pos);
        TileBase tile = null;

        //switch (side)
        //{
        //    case Side.Left:
        //        tile = dataPainter.wallLeft;
        //        break;

        //    case Side.Right:
        //        tile = dataPainter.wallRight;
        //        break;

        //    case Side.Down:
        //        half = true;
        //        tile = dataPainter.wallDown;
        //        break;

        //    case Side.Up:
        //        tile = dataPainter.wallUp;
        //        break;

        //    case Side.Up | Side.Right:
        //        tile = dataPainter.wallUp_Right;
        //        break;

        //    case Side.Up | Side.Left:
        //        tile = dataPainter.wallUp_Left;
        //        break;

        //    case Side.Up | Side.Right | Side.Corner:
        //        tile = dataPainter.wallUp_RightCorner;
        //        break;

        //    case Side.Up | Side.Left | Side.Corner:
        //        tile = dataPainter.wallUp_LeftCorner;
        //        break;

        //    case Side.Down | Side.Left:
        //        tile = dataPainter.wallDown_Left;
        //        half = true;
        //        break;

        //    case Side.Down | Side.Right:
        //        tile = dataPainter.wallDown_Right;
        //        half = true;
        //        break;

        //    case Side.Down | Side.Right | Side.Corner:
        //        tile = dataPainter.wallDown_RightCorner;
        //        break;

        //    case Side.Down | Side.Left | Side.Corner:
        //        tile = dataPainter.wallDown_LeftCorner;
        //        break;

        //    default:
        //        tile = dataPainter.floor;
        //        break;
        //}

        ////if not floor
        //if(side != 0)
        //{
        //    if(!half) tileMapCollider.SetTile(tilePos, dataPainter.empty);
        //    else tileMapColliderHalf.SetTile(tilePos, dataPainter.empty); half = false;
        //}

        //if(tile != null) tileMap.SetTile(tilePos, tile);

    }

}
