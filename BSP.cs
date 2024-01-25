using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSPNode
{
    public BSPNode leftChild;
    public BSPNode rightChild;
    public RectInt bounds;
    public bool isLeaf;
    public Room room;
}

// A room in the dungeon
public class Room
{
    public RectInt bounds;
    public List<Vector2Int> doors;
}

public class BSP : Dungeon
{
    int currentGridIndex = 0;
  // The maximum depth of the BSP tree

    public BSP(Grid grid)
    {
        this.grid = grid;
    }

    public void GenerateBSP(int i)
    {
       
        if (grid.depth > grid.maxDepth) grid.depth = grid.maxDepth;
        currentGridIndex = i;
        grid.grid[i].Clear();
        grid.floorPositions[i].Clear();

        // Generate the BSP tree
        BSPNode root = new BSPNode();
        root.bounds = grid.gridSize[i];
        SplitBSP(root, grid.depth);

        // Generate rooms for each leaf node in the tree
        List<Room> rooms = new List<Room>();
        GetLeafRooms(root, rooms);


        // Fill in the rooms with floors
        foreach (Room room in rooms)
        {
            for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
            {
                for (int y = room.bounds.yMin; y < room.bounds.yMax; y++)
                {
                    grid.floorPositions[i].Add(new Vector2Int(x, y));
                }
            }
        }

        //// Add doors between adjacent rooms
        //foreach (Room room in rooms)
        //{
        //    foreach (Vector2Int door in room.doors)
        //    {
        //        grid.floorPositions[i].Add(door);
        //    }
        //}

    }

    // Split a BSP node recursively until the stopping criterion is met
    private void SplitBSP(BSPNode node, int depth)
    {
        if (depth >= grid.maxDepth)
        {
            node.isLeaf = true;
            return;
        }

        // Choose a random line or plane to split the node
        bool isVerticalSplit = Random.value < 0.5f;
        int splitPosition = isVerticalSplit ? Random.Range(node.bounds.xMin, node.bounds.xMax) :
            Random.Range(node.bounds.yMin, node.bounds.yMax);

        // Create the child nodes
        node.leftChild = new BSPNode();
        node.rightChild = new BSPNode();

        // Set the bounds of the child nodes
        if (isVerticalSplit)
        {
            node.leftChild.bounds = new RectInt(node.bounds.xMin, node.bounds.yMin, splitPosition - node.bounds.xMin, node.bounds.height);
            node.rightChild.bounds = new RectInt(splitPosition, node.bounds.yMin, node.bounds.xMax - splitPosition, node.bounds.height);
        }
        else
        {
            node.leftChild.bounds = new RectInt(node.bounds.xMin, node.bounds.yMin, node.bounds.width, splitPosition - node.bounds.yMin);
            node.rightChild.bounds = new RectInt(node.bounds.xMin, splitPosition, node.bounds.width, node.bounds.yMax - splitPosition);
        }

        // Recursively split the child nodes
        SplitBSP(node.leftChild, depth + 1);
        SplitBSP(node.rightChild, depth + 1);


    }

    //// Get the leaf nodes of the BSP tree and generate rooms for them
    //private void GetLeafRooms(BSPNode node, List<Room> rooms)
    //{
    //    if (node.isLeaf)
    //    {
    //        Room room = new Room();
    //        room.bounds = new RectInt(node.bounds.xMin + 1, node.bounds.yMin + 1, node.bounds.width - 2, node.bounds.height - 2);
    //        room.doors = new List<Vector2Int>();
    //        rooms.Add(room);
    //        node.room = room;

    //        // Create doors between adjacent rooms
    //        if (rooms.Count > 1)
    //        {
    //            Room previousRoom = rooms[rooms.Count - 2];
    //            CreateDoor(previousRoom, room);
    //        }
    //    }
    //    else
    //    {
    //        GetLeafRooms(node.leftChild, rooms);
    //        GetLeafRooms(node.rightChild, rooms);
    //    }
    //}

    // Get the leaf nodes of the BSP tree and generate rooms for them
    private void GetLeafRooms(BSPNode node, List<Room> rooms)
    {
        if (node.isLeaf)
        {
            Room room = new Room();
            room.bounds = new RectInt(node.bounds.xMin + 1, node.bounds.yMin + 1, node.bounds.width - 2, node.bounds.height - 2);
            //room.doors = new List<Vector2Int>();
            rooms.Add(room);
            node.room = room;

            // Connect doors between adjacent rooms with a path
            if (rooms.Count > 1)
            {
                Room previousRoom = rooms[rooms.Count - 2];
                CreatePath(previousRoom, room);
            }
        }
        else
        {
            GetLeafRooms(node.leftChild, rooms);
            GetLeafRooms(node.rightChild, rooms);
        }
    }

    // Create a path between two rooms
    private void CreatePath(Room roomA, Room roomB)
    {
        // Choose a random point in each room
        Vector2Int pointA = new Vector2Int(Random.Range(roomA.bounds.xMin, roomA.bounds.xMax), Random.Range(roomA.bounds.yMin, roomA.bounds.yMax));
        Vector2Int pointB = new Vector2Int(Random.Range(roomB.bounds.xMin, roomB.bounds.xMax), Random.Range(roomB.bounds.yMin, roomB.bounds.yMax));

        grid.floorPositions[currentGridIndex].UnionWith(AStar.FindPath(pointA, pointB));
        // Create a straight path between the points
        //Vector2Int direction = pointB - pointA;
        //int distance = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        //direction = new Vector2Int(Mathf.RoundToInt((float)direction.x / distance), Mathf.RoundToInt((float)direction.y / distance));
        //Vector2Int currentPoint = pointA;
        //for (int i = 0; i <= distance; i++)
        //{
        //    grid.floorPositions[currentGridIndex].Add(currentPoint);
        //    currentPoint += direction;
        //}
    }

    // Create a door between two rooms
    private void CreateDoor(Room roomA, Room roomB)
    {
        Vector2Int doorPosition = new Vector2Int(0, 0);

        // Choose a side of the room to place the door
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Top
                doorPosition = new Vector2Int(Random.Range(roomA.bounds.xMin, roomA.bounds.xMax), roomA.bounds.yMax);
                break;
            case 1: // Bottom
                doorPosition = new Vector2Int(Random.Range(roomA.bounds.xMin, roomA.bounds.xMax), roomA.bounds.yMin - 1);
                break;
            case 2: // Left
                doorPosition = new Vector2Int(roomA.bounds.xMin - 1, Random.Range(roomA.bounds.yMin, roomA.bounds.yMax));
                break;
            case 3: // Right
                doorPosition = new Vector2Int(roomA.bounds.xMax, Random.Range(roomA.bounds.yMin, roomA.bounds.yMax));
                break;
        }

        // Add the door position to both rooms
        roomA.doors.Add(doorPosition);
        roomB.doors.Add(doorPosition);
    }

    void CreateCorridor()
    {
        
    }
}


