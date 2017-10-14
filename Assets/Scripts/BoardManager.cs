using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Runtime.Serialization;
using System;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            this.minimum = min;
            this.maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(5, 9);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;

    /// <summary>
    /// list of possible positions to place walls, floors, items
    /// </summary>
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();
        for(int x = 0; x < columns-1; x++)
        {
            for(int y =0; y<rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        //we're building an edge around the active board area
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                var toInstantiate = RandomOf(floorTiles);// floorTiles[Random.Range(0, floorTiles.Length)];
                //do we need an outer wall?
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = RandomOf(outerWallTiles);
                var instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder); 
            }
        }
    }

    Vector3 RandomPosition()
    {
        var randomPosition = RandomOf(gridPositions);
        gridPositions.Remove(randomPosition);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //why +1 now?
        int objectCount = Random.Range(minimum, maximum+1);
        for(int i = 0; i < objectCount; i++)
        {
            var randomPosition = RandomPosition();
            var chosenTile = RandomOf(tileArray);
            Instantiate(chosenTile, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.maximum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

    private T RandomOf<T>(T[] items)
    {
        return items[Random.Range(0, items.Length)];
    }

    private T RandomOf<T>(List<T> items)
    {
        return items[Random.Range(0, items.Count)];
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
