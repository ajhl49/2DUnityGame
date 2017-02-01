using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class World
{
    private Tile[,] tiles;

    public int Width { get; private set; }

    public int Height { get; private set; }

    private Action<InstalledObject> cbInstalledObjectCreated;

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

        var wallPrototype = InstalledObject.CreatePrototype("station_wall_black", 0, 1, 1, true);

        installedObjectPrototypes.Add("station_wall_black", wallPrototype);
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

    public void PlaceInstalledObject(string objectType, Tile tile)
    {
        // TODO: This function assumes 1x1 tiles -- change this later

        if (installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes doesn't contain a proto for key: " + objectType);
            return;
        }

        var installedObject = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], tile);

        if (installedObject == null)
        {
            // Failed to place object -- most likely an installed object was already placed
            return;
        }

        if (cbInstalledObjectCreated != null)
        {
            cbInstalledObjectCreated(installedObject);
        }
    }

    public void RegisterInstalledObjectCreated(Action<InstalledObject> callbackFunction)
    {
        cbInstalledObjectCreated += callbackFunction;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callbackFunction)
    {
        cbInstalledObjectCreated -= callbackFunction;
    }
}
