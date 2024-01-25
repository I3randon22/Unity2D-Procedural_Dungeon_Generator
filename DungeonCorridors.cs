using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DungeonCorridors : Dungeon
{
    public HashSet<Vector2Int>[] corridors;
    public List<Vector2Int> starters;
    public List<Vector2Int> path;
    public DungeonCorridors(Grid grid, RandomWalk randomWalk)
    {
        this.grid = grid;
        this.randomWalk = randomWalk;
        starters = new List<Vector2Int>();
    }
    public override void CreateCorridors()
    {
        corridors = new HashSet<Vector2Int>[grid.gridAmount-1];

        if (grid.algorithm == Algorithm.BOXROOM) BoxRoomCorridors();
        else
        {
            Corridors();
        }
    }

    void Corridors()
    {
        List<Vector2Int> canRemovePos = new List<Vector2Int>();

        // iterate over each room in the grid
        for (int i = 1; i < grid.gridAmount; i++)
        {
            // create a new set to store the corridor positions
            corridors[i - 1] = new HashSet<Vector2Int>();

            // find the path between the current and previous room
            path = AStar.FindPath(grid.floorPositions[i - 1].First(), grid.floorPositions[i].First());

            // iterate over each position in the path
            foreach (Vector2Int pos in path)
            {
                bool canRemove = false;

                // iterate over each floor position in the previous room
                foreach (Vector2Int floor in grid.floorPositions[i - 1])
                {
                    if (grid.floorPositions[i - 1] != null)
                    {
                        // if the current position is a floor position in the previous room, mark it for removal and the remaining elements
                        if (pos == floor)
                        {
                            canRemove = true;
                        }
                    }
                }

                if (canRemove) canRemovePos.Add(pos);
            }

            // remove any marked positions from the path
            path.RemoveAll(pos => canRemovePos.Contains(pos));

            // add the remaining positions in the path to the current room's floor positions
            grid.floorPositions[i].UnionWith(path);


            // if there are more rooms and multiple corridors are allowed, find the path between the current room and the next room
            if (i < grid.gridAmount - 1 && grid.multipleCorridors)
            {
                grid.floorPositions[i].UnionWith(AStar.FindPath(grid.startCorridor[i - 1].position, grid.floorPositions[i + 1].First()));
            }

            // add the path to the set of corridor positions for the current room
            corridors[i - 1].UnionWith(path);

            // clear the list of positions to remove for the next iteration
            canRemovePos.Clear();
        }

    }

    void BoxRoomCorridors()//Change to A*?
    {
        for (int gridNum = 0; gridNum < grid.gridAmount - 1; gridNum++)
        {
            int offset = Mathf.RoundToInt(Mathf.Abs((grid.startCorridor[gridNum].position - grid.endCorridor[gridNum]).magnitude));
            corridors[gridNum] = new HashSet<Vector2Int>();
            Side side = grid.startCorridor[gridNum].side;

            for (int i = 0; i <= offset; i++) //Create corridor
            {
                Node newNode = ScriptableObject.CreateInstance<Node>();

                switch (side)
                {
                    case Side.Left:
                        newNode.position = grid.startCorridor[gridNum].position + (Vector2Int.left * i);
                        break;

                    case Side.Right:
                        newNode.position = grid.startCorridor[gridNum].position + (Vector2Int.right * i);
                        break;

                    case Side.Up:
                        newNode.position = grid.startCorridor[gridNum].position + (Vector2Int.up * i);
                        newNode.isFloor = true;
                        break;

                    case Side.Down:
                        newNode.position = grid.startCorridor[gridNum].position + (Vector2Int.down * i);
                        break;
                }

                grid.floorPositions[gridNum].Add(newNode.position);
                corridors[gridNum].Add(newNode.position);

            }

        }
    }
}
