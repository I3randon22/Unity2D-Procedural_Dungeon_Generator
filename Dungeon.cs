using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class DungeonSet
{
    public bool hasCompleted;
    public bool hasUnlocked;
    public List<Items> resourcesSpawn;
}

public class Dungeon : MonoBehaviour
{
    public Grid grid;
    [SerializeField] protected Data_DungeonSpawner dataSpawner;
    [SerializeField] protected Data_DungeonPainter dataPainter;
    [HideInInspector] protected DungeonLogic logic;
    [HideInInspector] protected DungeonWalls walls;
    [HideInInspector] protected DungeonSpawner spawner;
    [HideInInspector] protected DungeonCorridors corridor;
    [HideInInspector] protected RandomWalk randomWalk;
    [HideInInspector] protected CellularAutomata cellularAutomata;
    [HideInInspector] protected BSP bSP;
    [HideInInspector] protected Painting painter;

    [SerializeField] protected Tilemap tileMap;
    [SerializeField] protected Tilemap tileMapCollider;
    [SerializeField] protected Tilemap tileMapColliderHalf;
    [SerializeField] protected bool hasDoors;
    [SerializeField] protected int maxFloors = 1;
    [SerializeField] protected int maxEnemies = 1;
    [SerializeField] protected int decorSpawnRate = 20;
    public HashSet<Vector2Int>[] floorPositions;

    private void OnEnable()
    {
        GameEvents.instance.OnGridCreated += RunProceduralGeneration;
        GameEvents.instance.OnDungeonCreated += spawner.SpawnObjects;
        GameEvents.instance.OnEnemyDeath += logic.CheckEnemies;
        GameEvents.instance.OnDungeonProgress += logic.DungeonCompletion;
    }

    private void OnDisable()
    {
        GameEvents.instance.OnGridCreated -= RunProceduralGeneration;
        GameEvents.instance.OnDungeonCreated -= spawner.SpawnObjects;
        GameEvents.instance.OnEnemyDeath -= logic.CheckEnemies;
        GameEvents.instance.OnDungeonProgress -= logic.DungeonCompletion;
    }

    private void Awake()
    {
        grid = GetComponent<Grid>();
        logic = new DungeonLogic(grid, hasDoors, maxFloors);
        painter = new Painting(grid, randomWalk, corridor, tileMap, tileMapCollider, tileMapColliderHalf, dataPainter);
        randomWalk = new RandomWalk(grid, corridor, painter);
        walls = new DungeonWalls(floorPositions, painter, grid);
        corridor = new DungeonCorridors(grid, randomWalk);
        spawner = new DungeonSpawner(grid, logic, dataSpawner, hasDoors, painter, maxEnemies, corridor);
        bSP = new BSP(grid);
        cellularAutomata = new CellularAutomata(grid);

        Debug.Log("painter in dungeon: " + painter);
        painter.DeleteTiles();
        painter.tileMap = tileMap;
    }

    public void RunProceduralGeneration()
    {
        for (int i = 0; i < grid.gridAmount; i++)
        {
            switch (grid.algorithm)
            {
                case Algorithm.BOXROOM:
                    maxFloors = 3;
                    break;
                case Algorithm.RANDOMWALK:
                    maxFloors = 5;
                    randomWalk.GenerateRandomWalk(i);
                    break;
                case Algorithm.BSP:
                    maxFloors = 3;
                    bSP.GenerateBSP(i);
                    break;
                case Algorithm.CellularAutomata:
                    maxFloors = 4;
                    cellularAutomata.GenerateCA(i);
                    break;
                case Algorithm.RANDOM:
                    maxFloors = 6;
                    RandomAlgorithm(i);
                    break;
                default:
                    break;
            }
        }

        logic.maxFloors = maxFloors;
        corridor.CreateCorridors();    
        walls.CreateWalls(grid.floorPositions);
        painter.PaintDungeon();

        // Make one HashSet of all floor positions
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        foreach (HashSet<Vector2Int> list in grid.floorPositions) floorPositions.UnionWith(list);

        AStar.floorPositions = floorPositions;

        //Remove corridors for spawning
        for (int i = 0; i < grid.gridAmount - 1; i++)
        {
            grid.floorPositions[i + 1].ExceptWith(corridor.corridors[i]);
        }

        GameEvents.instance.DungeonCreated();
    }

    private void RandomAlgorithm(int i)
    {
        Algorithm[] algorithms = (Algorithm[])System.Enum.GetValues(typeof(Algorithm));

        // Select a random index from the array
        int randomIndex = UnityEngine.Random.Range(0, algorithms.Length);

        // Retrieve the corresponding enum value
        Algorithm randomAlgorithm = algorithms[randomIndex];
        Debug.Log("Room: " + i + " Algorithm: " + randomAlgorithm);

        switch (randomAlgorithm)
        {
            case Algorithm.BOXROOM:
                break;

            case Algorithm.RANDOMWALK:
                randomWalk.GenerateRandomWalk(i);
                break;
            case Algorithm.BSP:
                bSP.GenerateBSP(i);
                break;
            case Algorithm.CellularAutomata:
                cellularAutomata.GenerateCA(i);
                break;
            case Algorithm.RANDOM:
                RandomAlgorithm(i);
                break;
            default:
                break;
        }
    }

    public GameObject Spawn(GameObject obj, Vector2 pos)
    { 
        return Instantiate(obj, pos, Quaternion.identity);
    }

    public virtual void CreateCorridors() { }
    public virtual void CheckEnemies() { }
    public virtual void PaintDungeon() { }

  

    private void OnDrawGizmos()
    {
        //if (grid != null)
        //{
        //    for (int i = 0; i < grid.gridAmount; i++)
        //    {
        //        Gizmos.color = Color.white;
        //        foreach (Vector2Int pos in grid.floorPositions[i])
        //        {
        //            Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0), new Vector3(grid.nodeSize, grid.nodeSize, grid.nodeSize));
        //        }

        //        if (corridor != null && corridor.path[i] != null)
        //        {
        //            Gizmos.color = Color.red;
        //            Gizmos.DrawCube(new Vector3(corridor.path[i].x, corridor.path[i].y, 0), new Vector3(grid.nodeSize, grid.nodeSize, grid.nodeSize));                 
        //        }
        //    }
        //}

        if(corridor.corridors != null)
        {
            Gizmos.color = Color.green;
            for(int i = 0; i < corridor.corridors.Length; i++)
            {
                for(int j = 0; j < corridor.corridors[i].Count; j++)
                {
                    Gizmos.DrawCube(new Vector3(corridor.corridors[i].ElementAt(j).x, corridor.corridors[i].ElementAt(j).y, 0), new Vector3(grid.nodeSize, grid.nodeSize, grid.nodeSize));
                }
            }
        }
    }
}

public static class Direction2D
{
    public static List<Vector2Int> directionCardinalList = new List<Vector2Int>
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, 0) //LEFT
    };

    public static List<Vector2Int> directionDiagonalList = new List<Vector2Int>
    {
        new Vector2Int(1,1), //UP-RIGHT
        new Vector2Int(1,-1), //RIGHT-DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 1) //LEFT-UP
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,1), //UP-RIGHT
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(1,-1), //RIGHT-DOWN
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0), //LEFT
        new Vector2Int(-1, 1) //LEFT-UP

    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return directionCardinalList[Random.Range(0, directionCardinalList.Count)];
    }

    
}
