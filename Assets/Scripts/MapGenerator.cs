using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using NaughtyAttributes;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] private string seed;
    [SerializeField] private bool useRandomSeed;


    [SerializeField] private int timesSmooth;

    [SerializeField, Range(0, 100)] private int randomFillPercent;

    [SerializeField] private int width;
    [SerializeField] private int height;

    private List<float[,]> maps = new List<float[,]>();

    [SerializeField] private int borderSize;

    [SerializeField] private float mountainSize;

    private int majorityRange;

    private Terrain terrain;

    private void Start()
    {

        terrain = GetComponent<Terrain>();

        GenerateMap(SmoothMap44);
    }

    /// <summary>
    /// Generate the map and smooth it out according the cellular automata rule
    /// </summary>
    /// <param name="smoothMethod"></param>
    private void GenerateMap(Action smoothMethod)
    {
        // Empty the map list to receive the maps of n iterations
        maps = new List<float[,]>();

        // Initalize an empty map to fill it out
        float [,] map = new float[width, height];

        maps.Add(map);

        // Give a random value to the majority range to keep it consistent throught out all iterations
        majorityRange = UnityEngine.Random.Range(38, 45);

        // Randomly fill the map according to the fill percentage
        RandomFillMap();

        // Invoke the iteration method "timsSmooth" times defined by the user in the inspector
        for (int i = 0; i < timesSmooth; i++)
        {
            smoothMethod.Invoke();
        }

        GenerateHeights();
    }

    /// <summary>
    /// Randomly fill map with randomFillPercent FillPercent
    /// </summary>
    private void RandomFillMap()
    {
        if(useRandomSeed) 
        {
            seed = (System.DateTime.Now.Millisecond * System.DateTime.Now.Second).ToString();
        }

        System.Random rng = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {  
                maps[0][x, y] = (rng.Next(0, 100) < randomFillPercent) ? 1 : 0;  
            }
        }

 
        
    }

    /////////////////// BUTTONS ///////////////////////////////////////////////
    [Button]
    private void GenerateSmoothMap44() => GenerateMap(SmoothMap44);
    [Button]
    private void GenerateSmoothMap45() => GenerateMap(SmoothMap45);
    [Button]
    private void GenerateSmoothMapMajority() => GenerateMap(SmoothMapMajority);

    [Button]
    private void GenerateSmoothMapWalledCities() => GenerateMap(SmoothMapWalledCities);
    [Button]
    private void GenerateSmoothMapDiamoeba() => GenerateMap(SmoothMapDiamoeba);
    [Button]
    private void GenerateSmoothMapCoral() => GenerateMap(SmoothMapCoral);
    [Button]
    private void GenerateSmoothMapHighLife() => GenerateMap(SmoothMapHighLife);
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 45 Rules
    /// If it has 5 or more neighbours it becomes alive
    /// If it has 3 or less neighbours it dies
    /// If it has 4 it stays as is
    /// </summary>
    private void SmoothMap44()
    {
        float[,] map = new float[width,height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = maps[maps.Count - 1][x, y]; 
            }
        }

        maps.Add(map);

        for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float neighbourWallTiles = GetNeighboursCount(x, y, 1);

                    if (neighbourWallTiles > 4)
                    {
                        maps[maps.Count - 1][x, y] = 1;
                    }
                    else if (neighbourWallTiles < 4)
                        maps[maps.Count - 1][x, y] = 0;
             
                }
            }

        
    }

    /// <summary>
    /// 45 Rules
    /// If it has 5 or more neighbours it becomes alive
    /// If it has 4 or less neighbours it dies
    /// </summary>
    private void SmoothMap45()
    {
        float[,] map = new float[width, height];

        maps.Add(map);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float neighbourWallTiles = GetNeighboursCount(x, y, 1);

                if (neighbourWallTiles > 4)
                {
                    maps[maps.Count - 1][x, y] = 1;
                }
                else if (neighbourWallTiles < 5)
                    maps[maps.Count - 1][x, y] = 0;

            }
        }

        
    }

    /// <summary>
    /// Majority rules
    /// http://www.mirekw.com/ca/rullex_lgtl.html#Majority
    /// </summary>
    private void SmoothMapMajority()
    {
        float[,] map = new float[width, height];

        maps.Add(map);

        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float neighbourWallTiles = GetNeighboursCount(x, y, 4);

                if (neighbourWallTiles >= majorityRange)
                {
                    maps[maps.Count - 1][x, y] = 1;
                }
                else if (neighbourWallTiles < majorityRange)
                    maps[maps.Count - 1][x, y] = 0;

            }
        }

        
    }

    /// <summary>
    /// Diamoeba rules
    /// http://www.mirekw.com/ca/rullex_life.html#Diamoeba
    /// </summary>
    private void SmoothMapDiamoeba()
    {
        float[,] map = new float[width, height];

        maps.Add(map);



        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float neighbourWallTiles = GetNeighboursCount(x, y, 1);

                if (maps[maps.Count - 2][x, y] == 1 && (neighbourWallTiles >= 5 && neighbourWallTiles <= 8))
                {
                    maps[maps.Count - 1][x, y] = 1;
                }
                else if (maps[maps.Count - 2][x, y] == 0 && (neighbourWallTiles >= 5 && neighbourWallTiles <= 8 || neighbourWallTiles == 3))
                    maps[maps.Count - 1][x, y] = 1;
                else
                {
                    maps[maps.Count - 1][x, y] = 0;
                }

            }
        }


    }

    /// <summary>
    /// Coral rules
    /// http://www.mirekw.com/ca/rullex_life.html#Coral
    /// </summary>
    private void SmoothMapCoral()
    {
        float[,] map = new float[width, height];

        maps.Add(map);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float neighbourWallTiles = GetNeighboursCount(x, y, 1);

                if (maps[maps.Count - 2][x, y] == 1 && (neighbourWallTiles >= 4 && neighbourWallTiles <= 8))
                {
                    maps[maps.Count - 1][x, y] = 1;
                }
                else if (maps[maps.Count - 2][x, y] == 0 && (neighbourWallTiles == 3))
                    maps[maps.Count - 1][x, y] = 1;
                else
                {
                    maps[maps.Count - 1][x, y] = 0;
                }

            }
        }


    }

    /// <summary>
    /// High Life rules
    /// http://www.mirekw.com/ca/rullex_life.html#HighLife
    /// </summary>
    private void SmoothMapHighLife()
    {
        float[,] map = new float[width, height];

        maps.Add(map);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float neighbourWallTiles = GetNeighboursCount(x, y, 1);

                if (maps[maps.Count - 2][x, y] == 1 && (neighbourWallTiles == 2 || neighbourWallTiles == 3))
                {
                    maps[maps.Count - 1][x, y] = 1;
                }
                else if (maps[maps.Count - 2][x, y] == 0 && (neighbourWallTiles == 3 || neighbourWallTiles == 6))
                    maps[maps.Count - 1][x, y] = 1;
                else
                {
                    maps[maps.Count - 1][x, y] = 0;
                }

            }
        }


    }

    /// <summary>
    /// Walled Cities rules
    /// http://www.mirekw.com/ca/rullex_life.html#WalledCities
    /// </summary>
    private void SmoothMapWalledCities()
    {
        float[,] map = new float[width, height];

        maps.Add(map);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float neighbourWallTiles = GetNeighboursCount(x, y, 1);

                if (maps[maps.Count - 2][x, y] == 1 && (neighbourWallTiles >= 2 && neighbourWallTiles <= 5))
                {
                    maps[maps.Count - 1][x, y] = 1;
                }
                else if (maps[maps.Count - 2][x, y] == 0 && (neighbourWallTiles >= 4 && neighbourWallTiles <= 8))
                    maps[maps.Count - 1][x, y] = 1;
                else
                {
                    maps[maps.Count - 1][x, y] = 0;
                }

            }
        }



    }

    /// <summary>
    /// Get the amount of neighbours for the cell (gridX, gridY) on a "range" sized radius
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private float GetNeighboursCount(int gridX, int gridY, int range)
    {
        float wallCount = 0;
        
        for (int neighbourX = gridX - range;  neighbourX <= gridX + range; neighbourX ++)
        {
            for (int neighbourY = gridY - range; neighbourY <= gridY + range; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                { 
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += maps[maps.Count - 2][neighbourX, neighbourY];
                    }
                }
            }
        }

        return wallCount;
    }

    /// <summary>
    /// Generate the height map for the terrain
    /// </summary>
    public void GenerateHeights()
    {

        // Remove the first map which is just a map filled with randomFillPercent % of active cells
        maps.RemoveAt(0);

        float[,] heights = new float[width, height];

        // Initalize everything as 0 to flatten out the terrain
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heights[i, j] = 0;
            }
        }

        // Flatten terrain
        terrain.terrainData.SetHeights(0, 0, heights);

        // Sum all the numbers of each cell of every map we have giving numbers from 0 to timesSmooth
        foreach (float[,] map in maps)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heights[x, y] += map[x,y];
                }
            }
        }

        // Multiply by mountainSize
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] *= mountainSize;
            } 
        }

        // Initalize a bordered map if user defines a borderSize in the inspector
        float[,] borderedMap = new float[width, height];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {

                if (x <= borderSize || x > width - borderSize || y <= borderSize || y > height - borderSize)
                {
                    borderedMap[x, y] = timesSmooth * mountainSize;
                }
                else
                {
                    borderedMap[x, y] = heights[x, y];
                }
            }
        }

        // Set the heights to the height map
        terrain.terrainData.SetHeights(0, 0, borderedMap);
    }

    // Uncomment this code to see each map in a gizmos, 
    // reduce size of width height to 100 because 500 will 
    // make it very laggy (250000 cubes), if what you want is to see the map iterations

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < maps.Count; i ++)
    //    {
    //        if (maps[i] != null)
    //        {
    //            for (int x = 0; x < width; x++)
    //            {
    //                for (int y = 0; y < height; y++)
    //                { 

    //                    if (maps[i][x, y] == 0)
    //                        Gizmos.color = Color.black;

    //                    if (maps[i][x, y] == 1)
    //                        Gizmos.color = Color.cyan;

    //                    Vector3 pos = new Vector3(-width / 2 + x + 0.5f - (i * 150) + 1000, -height / 2 + y + 0.5f, 0f);

    //                    Gizmos.DrawCube(pos, Vector3.one);
    //                }
    //            }
    //        }
    //    }
        
    //}
}
