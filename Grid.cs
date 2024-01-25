using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//FIX RIGHT SIDE ISSUE
public enum Algorithm
{
    BOXROOM,
    RANDOMWALK,
    BSP,
    CellularAutomata,
    RANDOM
};

public class Grid : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] [Range(5, 100)] int gridSizeMax = 10; 
    [SerializeField] [Range(5, 100)] int gridSizeMin = 10;
    [SerializeField] public int gridAmount = 5;
    [SerializeField] [Range(0.1f, 1)] internal float nodeSize = 0.9f;
    [SerializeField] [Range(2, 10)] int minOffset = 2;
    [SerializeField] [Range(2, 10)] int maxOffset = 10;
    [SerializeField] public Algorithm algorithm;
    [SerializeField] Vector2 startPos;
    public int currentOffset;
    internal int currentGridSize;
    public HashSet<Vector2Int>[] floorPositions { get; set; }
    public HashSet<Vector2Int>[] gridPositions { get; set; }
    public List<Node>[] grid { get; set; }
    List<Node>[] gridSides;
    HashSet<Vector2Int>[] gridSide;
    public List<Vector2Int> endCorridor { get; set; }
    public List<Node> startCorridor { get; set; }
    System.Random rng;

    public List<RectInt> gridSize;

    [Header("RandomWalk")]
    [SerializeField] public int pathLength = 10;
    [SerializeField] public bool randomSpawn;
    [SerializeField] public bool multipleCorridors;
    [SerializeField] [Range(1, 100)] public int iterations;

    [Header("BSP")]
    public int depth;
    public int maxDepth = 4;
    internal RectInt size;

    [Header("Cellular Automata")]
    public int wallPercent;

    // Start is called before the first frame update
    void Awake()
    {
        rng = new System.Random();
        //GenerateGrid(gridAmount);
    }

    private void Start()
    {
       // GenerateGrid(gridAmount);
    }

    public void SetGridVariables(int gridSizeMin, int gridSizeMax, int gridAmount, int minOffset, int maxOffset, Algorithm algorithm)
    {
        this.gridSizeMax = gridSizeMax;
        this.gridSizeMin = gridSizeMin;
        this.gridAmount = gridAmount;
        this.minOffset = minOffset;
        this.maxOffset = maxOffset;
        this.algorithm = algorithm;

        if (DungeonLogic.currentFloors > 1)
        {
            currentGridSize = this.gridSizeMin;
            int size = 2 * DungeonLogic.currentFloors - 1;
            if (size > 10) size = 10;
            this.gridSizeMin = (currentGridSize + size);
            this.gridSizeMax = (currentGridSize + size);
            this.gridAmount = gridAmount + 1;
        }
    }

    public void SetRandomWalk(bool randomSpawn, bool multipleCorridors)
    {
        this.randomSpawn = randomSpawn;
        this.multipleCorridors = multipleCorridors;
    }

    public void SetBSP(int depth, int maxDepth)
    {
        this.depth = depth;
        this.maxDepth = maxDepth;
    }

    public void SetCA(int wallPercent)
    {
        this.wallPercent = wallPercent;
    }

    public void GenerateGrid(int grids)
    {
        grid = new List<Node>[grids];
        floorPositions = new HashSet<Vector2Int>[grids];
        gridPositions = new HashSet<Vector2Int>[grids];
        startCorridor = new List<Node>();
        endCorridor = new List<Vector2Int>();
        gridSides = new List<Node>[grids];

        gridSide = new HashSet<Vector2Int>[grids];
        gridSize = new List<RectInt>();

        int xStart = Mathf.RoundToInt(startPos.x);
        int yStart = Mathf.RoundToInt(startPos.y);


        for (int i = 0; i < grids; i++)
        {
            grid[i] = new List<Node>();
            floorPositions[i] = new HashSet<Vector2Int>();
            gridPositions[i] = new HashSet<Vector2Int>();
            gridSides[i] = new List<Node>();

            gridSide[i] = new HashSet<Vector2Int>();

            int gridSizeX = UnityEngine.Random.Range(gridSizeMin, gridSizeMax);
            int gridSizeY = UnityEngine.Random.Range(gridSizeMin, gridSizeMax);


            if (i != 0)
            {
                Vector2Int offset = GetOffsetNode(new Vector2Int(gridSizeX, gridSizeY), i - 1);
                xStart = offset.x;
                yStart = offset.y;
            }


            gridSize.Add(new RectInt(xStart, yStart, gridSizeX, gridSizeY));

            bool exitFlag = false;

            //BoxRoom
            for (int x = xStart; x < gridSizeX + xStart && !exitFlag; x++)
            {
                for (int y = yStart; y < gridSizeY + yStart && !exitFlag; y++)
                {
                    Node node = ScriptableObject.CreateInstance<Node>();
                    node.position = new Vector2Int(x, y);
                    node.isFloor = true;
                    node.isUsed = true;

                    if (i != 0 && CheckForDuplicate(node))
                    {
                        Debug.Log("Duplicate: " + node.position);
                        grid[i].Clear();
                        gridSides[i].Clear();
                        gridSize.RemoveAt(i);
                        startCorridor.RemoveAt(i - 1);
                        endCorridor.RemoveAt(i - 1);
                        i -= 1;
                        exitFlag = true;
                        break;
                    }

                    grid[i].Add(node);
                    StoreSides(node, xStart, yStart, gridSizeX, gridSizeY, i);

                    if (node.side == 0)
                    {
                        floorPositions[i].Add(node.position);

                    }

                }
            }

            currentGridSize = gridSizeX;
        }

        
        GameEvents.instance.GridCreated();
    }

    public bool CheckForDuplicate(Node currentNode)
    {
        var combinedGrid = grid.Where(l => l != null).SelectMany(l => l); // Concatenate non-null lists in the grid array

        foreach (Node n in combinedGrid)
        {
            if (n.position == currentNode.position)
            {
                return true;
            }
        }

        return false;
    }



    //Set all sides and corners
    void StoreSides(Node node, int xStart, int yStart, int gridSizeX, int gridSizeY, int gridNum)
    {
        //Set if node is an outside node on grid
        if (node.position.x == xStart) node.side |= Side.Left;                      //Left Side
        if (node.position.x == (gridSizeX + xStart - 1)) node.side |= Side.Right;   //Right side
        if (node.position.y == yStart) node.side |= Side.Down;                      //Down Side
        if (node.position.y == (gridSizeY + yStart - 1)) node.side |= Side.Up;      //Up side

        //Set Corner
        int flagNum = 0;

        foreach (Side e in Enum.GetValues(typeof(Side)))
        {
            if (node.side.HasFlag(e))
            {
                flagNum++;
                node.isFloor = false;
            }

        }

        if (flagNum > 1) node.side |= Side.Corner;

        if (!node.side.HasFlag(Side.Corner) && flagNum > 0) gridSides[gridNum].Add(node);
    }

 

    // Get the offset for the next grid placement
    Vector2Int GetOffsetNode(Vector2Int size, int gridNum)
    {
        currentOffset = rng.Next(minOffset, maxOffset); //Get Offset
        //Grab a random side node
        int index = rng.Next(0, gridSides[gridNum].Count);
        startCorridor.Add(gridSides[gridNum][index]);
        Node node = gridSides[gridNum][index];
        Vector2Int newStartPosition = new Vector2Int(node.position.x, node.position.y);
        Vector2Int endPos;

        //Adding offset to startnode position to get new position
        //Find the bottom left corner...
        //right = topleft = startNode.position.y - size.y
        //down = top right = startNode.position - size
        //left = bottom right = startNode.position.x - size.x
        //top = bottom left
        switch (node.side)
        {
            case Side.Up:
                endPos = new Vector2Int(newStartPosition.x, newStartPosition.y + currentOffset);
                newStartPosition.y += currentOffset;
                newStartPosition.x -= rng.Next(2, size.x - 3);

                break;
            case Side.Down:
                endPos = new Vector2Int(newStartPosition.x, newStartPosition.y - currentOffset);
                newStartPosition = new Vector2Int(newStartPosition.x + rng.Next(3, size.x - 1), (newStartPosition.y - currentOffset) + 1) - size;

                break;
            case Side.Left:
                endPos = new Vector2Int(newStartPosition.x - currentOffset, newStartPosition.y);
                newStartPosition.x = (newStartPosition.x - currentOffset) - size.x + 1;
                newStartPosition.y -= rng.Next(2, size.y - 3);

                break;
            case Side.Right:
                endPos = new Vector2Int(newStartPosition.x + currentOffset, newStartPosition.y);
                newStartPosition = new Vector2Int((newStartPosition.x + currentOffset), (newStartPosition.y - size.y) + rng.Next(3, size.y - 1));

                break;
            default:
                return newStartPosition;
        }
        endCorridor.Add(endPos);
        return newStartPosition;
    }

    
    public Node GetNodeFromPosition(Vector2Int pos, List<Node>[] nodes)
    {
        for (int i = 0; i < gridAmount; i++)
        {
            if (i < nodes.Length && nodes[i] != null)
            {
                foreach (Node node in nodes[i])
                {
                    if (node.position == pos)
                        return node;
                }
            }
        }

        return ScriptableObject.CreateInstance<Node>();
    }

    //------------------------------------------------------------------------------------------------------------------------------
    //Vector2Int GetOffsetNodePos(Vector2Int size, int xStart, int yStart, int gridSizeX, int gridSizeY, int gridNum)
    //{
    //    int currentOffset = rng.Next(minOffset, maxOffset); //Get Offset
    //                                                        //Grab a random side node
    //    int index = rng.Next(0, gridSide[gridNum].Count);
    //    //startCorridor.Add(gridSide[gridNum][index]);
    //    Vector2Int node = gridSide[gridNum].ElementAt(index);
    //    Vector2Int newStartPosition = new Vector2Int(node.x, node.y);
    //    Vector2Int endPos;

    //    //Adding offset to startnode position to get new position
    //    //Find the bottom left corner...
    //    //right = topleft = startNode.y - size.y
    //    //down = top right = startNode - size
    //    //left = bottom right = startNode.x - size.x
    //    //top = bottom left

    //    if (node.x == xStart)                      //Left Side
    //    {
    //        floorPositions[gridNum].Remove(newStartPosition + Vector2Int.right);
    //        endPos = new Vector2Int(newStartPosition.x - currentOffset, newStartPosition.y);
    //        newStartPosition.x = (newStartPosition.x - currentOffset) - size.x + 1;
    //        newStartPosition.y -= rng.Next(2, size.y - 3);
    //    }
    //    else if (node.x == (gridSizeX + xStart - 1))   //Right side
    //    {
    //        floorPositions[gridNum].Remove(newStartPosition + Vector2Int.left);
    //        endPos = new Vector2Int(newStartPosition.x + currentOffset, newStartPosition.y);
    //        newStartPosition = new Vector2Int((newStartPosition.x + currentOffset), (newStartPosition.y - size.y) + rng.Next(3, size.y - 1));
    //    }
    //    else if (node.y == yStart)                     //Down Side
    //    {
    //        floorPositions[gridNum].Remove(newStartPosition + Vector2Int.up);
    //        endPos = new Vector2Int(newStartPosition.x, newStartPosition.y - currentOffset);
    //        newStartPosition = new Vector2Int(newStartPosition.x + rng.Next(3, size.x - 1), (newStartPosition.y - currentOffset) + 1) - size;
    //    }
    //    else if (node.y == (gridSizeY + yStart - 1))      //Up side
    //    {
    //        floorPositions[gridNum].Remove(newStartPosition + Vector2Int.down);
    //        endPos = new Vector2Int(newStartPosition.x, newStartPosition.y + currentOffset);
    //        newStartPosition.y += currentOffset;
    //        newStartPosition.x -= rng.Next(2, size.x - 3);
    //    }
    //    else
    //    {
    //        return newStartPosition;
    //    }


    //    endCorridor.Add(endPos);
    //    Debug.Log("COUNT" + startCorridor.Count);
    //    return newStartPosition;
    //}

    ////Set all sides and corners
    //void StoreSidesPos(Vector2Int node, int xStart, int yStart, int gridSizeX, int gridSizeY, int gridNum)
    //{
    //    //Set if node is an outside node on grid
    //    if (node.x == xStart) gridSide[gridNum].Add(node);                          //Left Side
    //    if (node.x == (gridSizeX + xStart - 1)) gridSide[gridNum].Add(node);   //Right side
    //    if (node.y == yStart) gridSide[gridNum].Add(node);                      //Down Side
    //    if (node.y == (gridSizeY + yStart - 1)) gridSide[gridNum].Add(node);      //Up side

    //    if((node.x == xStart && node.y == yStart) || (node.x == xStart && node.y == gridSizeY + yStart - 1) || (node.y == yStart && node.x == gridSizeX + xStart - 1) || (node.x == xStart + gridSizeX - 1 && node.y == yStart + gridSizeY - 1))
    //    {
    //       gridSide[gridNum].Remove(node);
    //    }


    //}

    //public bool CheckForDuplicatePos(Vector2Int currentPos)
    //{
    //    var combinedPositions = gridPositions.Where(l => l != null).SelectMany(l => l); // Concatenate non-null lists in the grid array

    //    foreach (Vector2 n in combinedPositions)
    //    {
    //        if (n == currentPos)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}
}
