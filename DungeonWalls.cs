using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonWalls : Dungeon
{
    public DungeonWalls(HashSet<Vector2Int>[] floorPositions, Painting painter, Grid grid)
    {
        this.painter = painter;
        this.grid = grid;
    }

    public void CreateWalls(HashSet<Vector2Int>[] floorPos)
    {
        if (floorPos == null) return; // Exit if input is null

        // Make one HashSet of all floor positions
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (HashSet<Vector2Int> list in floorPos) floorPositions.UnionWith(list);

        // Find wall and corner positions
        HashSet<Vector2Int> wallPositions = FindWallsInDirections(floorPositions, Direction2D.directionCardinalList);
        HashSet<Vector2Int> cornerPositions = FindWallsInDirections(floorPositions, Direction2D.directionDiagonalList);

        // Create walls
        CreateBasicWalls(wallPositions, floorPositions);
        CreateCornerWalls(cornerPositions, floorPositions);
    }

    private HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        // Initialize a new HashSet to store the wall positions
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        // Iterate through each floor position and each direction
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                // Calculate the neighbor position
                var neighbourPosition = position + direction;

                // Check if the neighbor position is a wall and add it to the HashSet if it is
                if (!floorPositions.Contains(neighbourPosition)) wallPositions.Add(neighbourPosition);
            }
        }

        // Return the HashSet of wall positions
        return wallPositions;
    }

    private string BuildNeighborBinaryType(List<Vector2Int> directionList, HashSet<Vector2Int> floorPositions, Vector2Int position)
    {
        // Initialize an empty string to store the binary type of the neighbors
        string neighborsBinaryType = "";

        // Iterate through each direction in the list and add 1 or 0 to the string depending on whether the neighbor position is a floor position
        foreach (var direction in directionList) neighborsBinaryType += (floorPositions.Contains(position + direction)) ? "1" : "0";

        // Return the binary type string
        return neighborsBinaryType;
    }

    private void CreateCornerWalls(HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        // Iterate through each corner wall position and paint it using the binary type string of its neighbors
        foreach (var position in cornerWallPositions) painter.PaintSingleCornerWall(position, BuildNeighborBinaryType(Direction2D.eightDirectionsList, floorPositions, position));
    }

    private void CreateBasicWalls(HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        // Iterate through each basic wall position and paint it using the binary type string of its neighbors
        foreach (var position in basicWallPositions) painter.PaintSingleBasicWall(position, BuildNeighborBinaryType(Direction2D.directionCardinalList, floorPositions, position));
    }



    //private void SetWalls(HashSet<Vector2Int> wallPositions, HashSet<Vector2Int> floorPositions)
    //{

    //    foreach (var position in wallPositions)
    //    {
    //        Side wallType = GetWallType(floorPositions, position);
    //        Debug.Log(wallType);
    //        painter.PaintTile(wallType, position);
    //    }
    //}

    //private void SetCornerWalls(HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    //{
    //    foreach (var position in cornerWallPositions)
    //    {
    //        Side wallType = GetWallType(floorPositions, position);

    //        painter.PaintTile(wallType, position);
    //    }
    //}

    //private HashSet<Vector2Int> FindWallsInDirection(HashSet<Vector2Int> floorPos, List<Vector2Int> directionList)
    //{
    //    HashSet<Vector2Int> wallPos = new HashSet<Vector2Int>();

    //    foreach(Vector2Int pos in floorPos)
    //    {
    //        foreach(Vector2Int direction in directionList)
    //        {
    //            Vector2Int neighbourPos = pos + direction;
    //            if(!floorPos.Contains(neighbourPos))
    //            {
    //                //painter.PaintTile(GetSide(direction), neighbourPos);
    //                wallPos.Add(neighbourPos);
    //            }
    //        }
    //    }
    //    return wallPos;
    //}

    //Side GetWallType(HashSet<Vector2Int> floorPositions, Vector2Int position)
    //{
    //    bool hasTopFloor = floorPositions.Contains(position + Vector2Int.up);
    //    bool hasBottomFloor = floorPositions.Contains(position + Vector2Int.down);
    //    bool hasLeftFloor = floorPositions.Contains(position + Vector2Int.left);
    //    bool hasRightFloor = floorPositions.Contains(position + Vector2Int.right);

    //    switch ((hasTopFloor ? 1 : 0) | (hasBottomFloor ? 2 : 0) | (hasLeftFloor ? 4 : 0) | (hasRightFloor ? 8 : 0))
    //    {
    //        case 0:
    //            return 0;
    //        case 1:
    //            return Side.Down;
    //        case 2:
    //            return Side.Up;
    //        case 4:
    //            return Side.Right;
    //        case 8:
    //            return Side.Left;
    //        case 3:
    //            return Side.Down | Side.Up;
    //        case 5:
    //            return Side.Down | Side.Right;
    //        case 6:
    //            return Side.Up | Side.Right;
    //        case 9:
    //            return Side.Down | Side.Left;
    //        case 10:
    //            return Side.Up | Side.Left;
    //        case 12:
    //            return Side.Right | Side.Left;
    //        case 7:
    //            return Side.Down | Side.Up | Side.Right;
    //        case 11:
    //            return Side.Down | Side.Up | Side.Left;
    //        case 13:
    //            return Side.Down | Side.Left | Side.Right;
    //        case 14:
    //            return Side.Up | Side.Left | Side.Right;
    //        case 15:
    //            return Side.Down | Side.Up | Side.Left | Side.Right;
    //        default:
    //            return 0;
    //    }
    //}

    //Side GetSide(Vector2Int pos)
    //{
    //    if (pos == Vector2Int.up) return Side.Up;
    //    if (pos == Vector2Int.down) return Side.Down;
    //    if (pos == Vector2Int.left) return Side.Left;
    //    if (pos == Vector2Int.right) return Side.Right;

    //    return 0;
    //}
}
