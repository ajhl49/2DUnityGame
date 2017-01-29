using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class World
{
    private Tile[,] tiles;

    public int Width { get; private set; }

    public int Height { get; private set; }

    private Dictionary<string, InstalledObject> installedObjectPrototypes;

    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        tiles = new Tile[width,height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x,y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (width*height) + " tiles.");

        CreateInstalledObjectPrototypes();
    }

    private void CreateInstalledObjectPrototypes()
    {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        var wallPrototype = InstalledObject.CreatePrototype(
            "Wall",
            0,
            1,
            1);

        installedObjectPrototypes.Add("Wall", wallPrototype);
    }

    /// <summary>
    /// Function for testing out the system.
    /// </summary>
    public void RandomizeTiles()
    {
        Debug.Log("RandomizeTiles");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    tiles[x, y].Type = TileType.Empty;
                }
                else
                {
                    tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Tile (" + x  + "," + y + ") is out of range");
            return null;
        }
        return tiles[x, y];
    }
}
