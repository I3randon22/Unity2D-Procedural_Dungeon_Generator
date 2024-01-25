using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DungeonLogic : Dungeon
{
    public List<GameObject>[] enemies;
    public GameObject[] doors;
    public static int currentFloors = 1;
    GameManager game;
    public static PlayerData tempData;

    public DungeonLogic(Grid grid, bool hasDoors, int maxFloors)
    {
        this.grid = grid;
        this.hasDoors = hasDoors;
        this.maxFloors = maxFloors;
        game = GameManager.instance;
    }
    public void Setup()
    {
        enemies = new List<GameObject>[grid.gridAmount];
        if(hasDoors) doors = new GameObject[grid.gridAmount-1];

        for (int i = 0; i < grid.gridAmount; i++)
        {
            enemies[i] = new List<GameObject>();
        }

        if (tempData == null && currentFloors == 1)
        {
            tempData = new PlayerData();
            GameManager.instance.SetData(tempData, GameManager.playerData); // Set temp data to current data when dungeon starts
        }
    }

    //Checks if all enemies in that room is dead, if so open the door in that room
    public override void CheckEnemies()
    {
        if (hasDoors)
        {
            base.CheckEnemies();
            for (int i = 0; i < grid.gridAmount; i++)
            {
                if (enemies[i].Contains(GameEvents.instance.GetCurrentEnemyDeath()))
                {
                    enemies[i].Remove(GameEvents.instance.GetCurrentEnemyDeath());
                    Destroy(GameEvents.instance.GetCurrentEnemyDeath().gameObject);
                    if (enemies[i].Count <= 0)
                    {
                        if (doors != null && doors[i] != null)
                        {
                            Destroy(doors[i].gameObject);
                            Debug.Log(doors[i].name + "Door open");
                        }

                        return;
                    }
                }
            }
        }
    }

    public void DungeonCompletion()
    {
        GameManager.instance.SaveAll(); //Save current data (if player dies or exists application, the temp data will be loaded and this will get wiped)


        currentFloors++; //increment the dungeon floor you are on
        Debug.Log("MaxFloors: " + maxFloors);
        if (currentFloors > maxFloors) //Change scene to reward scene where player opens golden chest to get rewards
        {
            //Set dungeon to completed and unlock next dungeon if it exists
            if(game.dungeonSet[GameManager.instance.currentDungeon] != null) game.dungeonSet[GameManager.instance.currentDungeon].hasCompleted = true;
            if (game.dungeonSet.Count > GameManager.instance.currentDungeon + 1) game.dungeonSet[GameManager.instance.currentDungeon + 1].hasUnlocked = true;
            currentFloors = 1;

            GameManager.instance.SaveAll(); //Save current data

            //Heal Player
            Health playerHealth = GameEvents.Player.GetComponent<Health>();
            playerHealth.ResetHealth();
            playerHealth.ReceieveHealth(playerHealth.GetMaxHealth());

            //Load hub scene
            SceneManager.LoadScene(0);
            Debug.Log("Dungeon Complete!!");
            return;
        }

        
        //Restart current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
