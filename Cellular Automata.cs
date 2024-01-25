using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : Dungeon
{
    Dictionary<Vector2Int, int> tiles;
    public CellularAutomata(Grid grid)
    {
        this.grid = grid;
    }

    public void GenerateCA(int i)
    {
        RandomizeGrid(i);

        for (int j = 0; j < 5; j++)
        {
            SmoothGrid();
        }

        grid.floorPositions[i].Clear();

        foreach (KeyValuePair<Vector2Int, int> cell in tiles)
        {
            if (cell.Value == 0) grid.floorPositions[i].Add(cell.Key);
        }
    }

    void RandomizeGrid(int index)
    {
        tiles = new Dictionary<Vector2Int, int>();
        foreach (Vector2Int pos in grid.floorPositions[index])
        {
            if (Random.Range(0, 100) < grid.wallPercent)
            {
                tiles.Add(pos, 1); // Wall tile
            }
            else
            {
                tiles.Add(pos, 0); // Floor tile
            }
        }
    }

    void SmoothGrid()
    {
        Dictionary<Vector2Int, int> newTiles = new Dictionary<Vector2Int, int>();

        foreach (KeyValuePair<Vector2Int, int> cell in tiles)
        {
            int wallNeighbours = CountNeighbors(cell.Key);

            if (wallNeighbours > 4) newTiles.Add(cell.Key, 1);
            else if (wallNeighbours < 4) newTiles.Add(cell.Key, 0);
            else newTiles.Add(cell.Key, cell.Value);
        }

        tiles = newTiles;
    }

    //Amount of wall tiles around the given position
    int CountNeighbors(Vector2Int pos)
    {
        int count = 0;

        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                Vector2Int neighborPos = new Vector2Int(x, y);
                if (neighborPos != pos && tiles.TryGetValue(neighborPos, out int tile) && tile == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
