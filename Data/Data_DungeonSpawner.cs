using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newSpawnerData", menuName = "Data/Dungeon/Spawners")]
public class Data_DungeonSpawner : ScriptableObject
{
    [SerializeField] public GameObject doorHorizontalObj;
    [SerializeField] public GameObject doorVerticalObj;
    [SerializeField] public GameObject playerObj;
    [SerializeField] public GameObject chestObj;
    [SerializeField] public GameObject enemyOneObj;
    [SerializeField] public GameObject stairsObj;
    [SerializeField] public GameObject skullDecor;
    [SerializeField] public GameObject crateDecor;
    [SerializeField] public GameObject barrelDecor;
    [SerializeField] public GameObject pillarDecor;
    [SerializeField] public GameObject ironOreObj;
}
