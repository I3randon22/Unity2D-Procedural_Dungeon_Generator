using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Side
{
    Up = 1 << 0,          // 1
    Left = 1 << 1,        // 2
    Right = 1 << 2,       // 4
    Down = 1 << 3,        // 8
    Corner = 1 << 4,      // 16
    //Diagonal = 1 << 5,    // 32
    //Inner = 1 << 6        // 64
}
public class Node : ScriptableObject
{
    public bool isUsed;
    public Vector2Int position;
    public Vector2 pos;
    public Side side;
    public bool corner = false;
    public bool isFloor = false;
}
