using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonSpawner : Dungeon
{
    List<Vector2> usedPos;
    Vector2 startPos;

    public DungeonSpawner(Grid grid, DungeonLogic logic, Data_DungeonSpawner dataSpawner, bool hasDoors, Painting painter, int maxEnemies, DungeonCorridors corridor)
    {
        this.grid = grid;
        this.logic = logic;
        this.dataSpawner = dataSpawner;
        this.hasDoors = hasDoors;
        this.painter = painter;
        this.maxEnemies = maxEnemies;
        this.corridor = corridor;
        Debug.Log(corridor);
    }

    public void SpawnObjects()
    {
        Debug.Log("Spawning");
        usedPos = new List<Vector2>();      
        logic.Setup();


        if (grid.floorPositions != null)
        {
            RemoveCorridor();

            for (int i = 0; i < grid.gridAmount; i++)
            {
                if (i > 0) SpawnEnemies(i);
                if (hasDoors) SpawnDoors(i);
                SpawnChests(i);
                SpawnDecor(i);
            }
            SpawnPlayer();
            SpawnStairs();
        }
    }

    void RemoveCorridor()
    {
        List<Vector2Int> neighbours = Direction2D.eightDirectionsList;
        List<Vector2Int> positions = new List<Vector2Int>();

        if (corridor.corridors != null)
        {
            for (int i = 0; i < corridor.corridors.Length; i++)
            {
                foreach (Vector2Int pos in corridor.corridors[i])
                {
                    foreach (Vector2Int neighbour in neighbours)
                    {
                        positions.Add(pos + neighbour);
                    }
                }
            }

            //Remove corridor for spawning
            for (int i = 0; i < grid.floorPositions.Length; i++)
            {
                for (int j = 0; j < corridor.corridors.Length; j++)
                {
                    grid.floorPositions[i].ExceptWith(corridor.corridors[j]);
                }
            }
            foreach (HashSet<Vector2Int> floorPositions in grid.floorPositions) //remove all neighbours of corridor
            {
                floorPositions.ExceptWith(positions);
            }
        }
    }

    void SpawnDecor(int i)
    {   
        // Check that floor positions exist for index i
        if (grid.floorPositions[i] != null)
        {
            int iterations = grid.floorPositions[i].Count / 2;

            for (int j = 0; j < iterations; j++)
            {
                // Generate a random number between 0 and 1
                float chance = Random.value;
                // Only proceed if chance is greater than or equal to 0.4 (10% chance of spawning)
                if (usedPos.Count < grid.floorPositions[i].Count && chance <= 0.1f)
                {
                    Debug.Log("Spawning Decor");
                    int index = Mathf.RoundToInt(Random.Range(0.1f, 5));
                    do //grab random position that is not used
                    {
                        startPos = grid.floorPositions[i].ElementAt(Random.Range(0, grid.floorPositions[i].Count));

                    } while (usedPos.Contains(startPos));
                    usedPos.Add(startPos); //add to used

                    //grab random decor to place
                    switch (index)
                    {
                        case 1:
                            startPos.y += 0.3f;
                            Spawn(dataSpawner.barrelDecor, startPos);
                            break;
                        case 2:
                            startPos.y += 0.3f;
                            Spawn(dataSpawner.crateDecor, startPos);
                            break;
                        case 3:
                            startPos.y -= 0.3f;
                            Spawn(dataSpawner.pillarDecor, startPos);
                            break;
                        case 4:
                            startPos.y += 0.3f;
                            Spawn(dataSpawner.skullDecor, startPos);
                            break;
                        case 5:
                            startPos.y += 0.1f;
                            Spawn(dataSpawner.ironOreObj, startPos);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    void SpawnDoors(int i)
    {
        GameObject door;
        if (i > 0 && logic.enemies[i-1].Count > 0)
        {
            Node node = grid.GetNodeFromPosition(grid.startCorridor[i - 1].position + Vector2Int.up, grid.grid);

            if(node.side == 0)
            {
                door = Spawn(dataSpawner.doorVerticalObj, Vector2.one * grid.startCorridor[i - 1].position);
            }
            else
            {
                door = Spawn(dataSpawner.doorHorizontalObj, Vector2.one * grid.startCorridor[i - 1].position);
            }
            logic.doors[i-1] = door;
        }

    }

    //Spawn player to random position in first room
    void SpawnPlayer()
    {
        Vector2 startPos = grid.floorPositions[0].ElementAt(Random.Range(0, grid.floorPositions[0].Count));
        usedPos.Add(startPos);
        GameEvents.Player.transform.position = startPos;
    }

    void SpawnChests(int i)
    {
        // Generate a random number between 0 and 1
        float chance = Random.value;

        // Only proceed if chance is greater than or equal to 0.4 (40% chance of spawning)
        if (chance <= 0.5f)
        {
            // Check that floor positions exist for index i
            if (grid.floorPositions[i] != null)
            {
                // Find all positions in floor positions that also exist in top floors
                HashSet<Vector2Int>[] matchingFloors = new HashSet<Vector2Int>[grid.floorPositions.Length];
                for (int j = 0; j < grid.floorPositions.Length; j++)
                {
                    matchingFloors[j] = new HashSet<Vector2Int>(grid.floorPositions[j].Intersect(painter.topFloors));
                }

                // If there are top floors to place chests on, proceed
                if (painter.topFloors.Count > 0 && usedPos.Count < matchingFloors[i].Count)
                {
                    // Keep trying to find a random position until one is found that hasn't been used yet
                    do
                    {
                        // Select a random position from the matching floors for index i
                        startPos = matchingFloors[i].ElementAt(Random.Range(0, matchingFloors[i].Count));
                    } while (usedPos.Contains(startPos));

                    // Add the position to the used positions list and adjust the height
                    usedPos.Add(startPos);
                    startPos.y += 0.7f;

                    // Spawn the chest object at the position
                    Spawn(dataSpawner.chestObj, startPos);
                }
            }
        }

    }

    void SpawnEnemies(int i)
    {
        int max = 2;
        if (GameManager.instance.currentDungeon > 0)
        {
            max = 2 + i;
            if (max > maxEnemies) max = maxEnemies;
        }
        else
        {
            if (DungeonLogic.currentFloors < 2) max = 1;
        }
        
       

        for (int j = 0; j < max; j++) //How many iterations
        {
            float chance = Random.Range(0f, 1f);
            Debug.Log("Chance: " + chance);
            if (usedPos.Count < grid.floorPositions[i].Count && chance <= 0.2f + (i * 0.1f)) //increase spawn chance by 10% per room
            {

                do //grab random position
                {
                    startPos = grid.floorPositions[i].ElementAt(Random.Range(0, grid.floorPositions[i].Count));

                } while (usedPos.Contains(startPos));
                usedPos.Add(startPos);
                GameObject enemy = Spawn(dataSpawner.enemyOneObj, startPos);
                enemy.name = "Enemy: " + i;
                logic.enemies[i].Add(enemy);
            }
        }
    }

    void SpawnStairs()
    {     
        Vector2 startPos = grid.floorPositions[grid.floorPositions.Length - 1].ElementAt(Random.Range(0, grid.floorPositions[grid.floorPositions.Length - 1].Count - 1));
        usedPos.Add(startPos);
        GameObject stairs = Spawn(dataSpawner.stairsObj, startPos);
    }
}
