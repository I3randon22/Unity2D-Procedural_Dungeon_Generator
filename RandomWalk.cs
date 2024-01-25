using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalk : Dungeon
{
   // DungeonCorridors corridor;
    Vector2Int startPos = Vector2Int.zero;
    public HashSet<Vector2Int>[] randomWalkPath;
    public HashSet<Node>[] randomWalkNodes { get; set; }
    int retries;

    public RandomWalk(Grid grid, DungeonCorridors corridor, Painting painter)
    {
        this.grid = grid;
        this.corridor = corridor;
        this.painter = painter;
    }


    public void GenerateRandomWalk(int i)
    {
        randomWalkNodes = new HashSet<Node>[grid.gridAmount];
        randomWalkPath = new HashSet<Vector2Int>[grid.gridAmount];
        grid.pathLength = Random.Range(grid.floorPositions[i].Count / 2, grid.floorPositions[i].Count); //set path length to random range between half of grid size and grid size

        if (grid.floorPositions[i].Count < grid.pathLength)
        {
            Debug.LogError("Path Length is bigger than the floor length, please lower pathlength value");
            return;
        }

        randomWalkPath[i] = new HashSet<Vector2Int>();
        randomWalkNodes[i] = new HashSet<Node>();

        if (grid.randomSpawn)
        {
            startPos = grid.floorPositions[i].ElementAt(Random.Range(0, grid.floorPositions[i].Count));
        }
        else
        {
            startPos = grid.floorPositions.ElementAt(i).First();
        }


        if (!randomWalkPath[i].Contains(startPos))
        {
            randomWalkPath[i].Add(startPos); //adding start position to the list
        }

        var previousPos = startPos; // setting previous position the same as start position

        int maxRetries = 5000;
        int retries = 0;

        grid.pathLength = Mathf.Min(grid.pathLength, grid.grid[i].Count - 1); // restrict path length to the grid size

        for (int t = 0; t < grid.pathLength - 1; t++)
        {
            Vector2Int newPos = previousPos + Direction2D.GetRandomCardinalDirection(); // Moving one step from previous position in a random direction
            if (grid.floorPositions[i].Contains(newPos)) //if it conatins floor position and there is no duplicate
            {
                if (!randomWalkPath[i].Contains(newPos))
                {
                    randomWalkPath[i].Add(newPos); // add the new position to the hashset
                    previousPos = newPos;
                    retries = 0;
                }
                else
                {
                    previousPos = newPos;
                    retries++;
                    t--; // reduce the loop counter so that we repeat this iteration
                }
            }
            else
            {
                retries++;
                t--; // reduce the loop counter so that we repeat this iteration
            }

            if (retries >= maxRetries)
            {
                Debug.LogError("Too many failed attempts, please recheck code");
                return;
            }
        }

        //Clear grid and floor positions and replace with random walk for that room
        grid.grid[i].Clear();
        grid.floorPositions[i].Clear();
        grid.floorPositions[i].UnionWith(randomWalkPath[i]);
}

    
    public HashSet<Vector2Int>[] GetrandomWalkPath()
    {
        return randomWalkPath;
    }

    public void CreateNodes(HashSet<Vector2Int> positions, int gridIndex)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Node node = ScriptableObject.CreateInstance<Node>();
            node.position = positions.ElementAt(i);
            node.isFloor = true;
            node.isUsed = true;

            randomWalkNodes[gridIndex].Add(node);

            Debug.Log(randomWalkNodes[gridIndex].ElementAt(0).position);
            //StoreSides(node, positions, gridIndex);
        }
    }


    //void PathToCorridor(int i)
    //{
    //    Vector2Int closestPos = Vector2Int.zero;
    //    Vector2Int target = corridor.corridors[i][0].position;

    //    if (!randomWalkPath[i].Contains(target)) //if random walk does not contain path leading to corridor, create one
    //    {
    //        closestPos = NodeClosestToNode(target, i);
    //    }

    //    while(!randomWalkPath[i].Contains(target))
    //    {
    //        List<float> num = new List<float>();

    //        num.AddRange(new float[] { 0, 1, 2, 3 });

    //        int t = Random.Range(0, num.Count);

    //        switch(t)
    //        {
    //            case 0:
    //                if (closestPos.y < target.y && grid.floorPositions[i].Contains(closestPos + Vector2Int.up))
    //                {
    //                    closestPos += Vector2Int.up;
    //                    randomWalkPath[i].Add(closestPos);
    //                }
    //                else num.Remove(0);
    //                break;
    //            case 1:
    //                if (closestPos.y > target.y && grid.floorPositions[i].Contains(closestPos + Vector2Int.down))
    //                {
    //                    closestPos += Vector2Int.down;
    //                    randomWalkPath[i].Add(closestPos);
    //                }
    //                else num.Remove(1);
    //                break;
    //            case 2:
    //                if (closestPos.x < target.x && grid.floorPositions[i].Contains(closestPos + Vector2Int.right))
    //                {
    //                    closestPos += Vector2Int.right;
    //                    randomWalkPath[i].Add(closestPos);
    //                }
    //                else num.Remove(2);
    //                break;
    //            case 3:
    //                if (closestPos.x > target.x && grid.floorPositions[i].Contains(closestPos + Vector2Int.left))
    //                {
    //                    closestPos += Vector2Int.left;
    //                    randomWalkPath[i].Add(closestPos);
    //                }
    //                else num.Remove(3);
    //                break;
    //            default:
    //                break;
    //        }
            
    //    }
    //}

    //Vector2Int NodeClosestToNode(Vector2Int target, int gridNum)
    //{
    //    float previousDistance = 0;
    //    float distance;
    //    Vector2Int closestPos = Vector2Int.zero;
    //    foreach (Vector2Int pos in randomWalkPath[gridNum])
    //    {
    //        distance = Vector2Int.Distance(pos, target);
    //        if(previousDistance == 0) previousDistance = distance;
    //        if(distance < previousDistance) closestPos = pos;
    //    }

    //    return closestPos;
    //}

   
}
