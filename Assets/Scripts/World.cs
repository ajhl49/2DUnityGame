using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Random = UnityEngine.Random;

public class World
{
    public static World WorldInstance { get; protected set; }

    private Tile[,] tiles;

    public int Width { get; private set; }

    public int Height { get; private set; }
    
    public event Action<Tile> TileChanged;
    public event Action<Furniture> FurnitureCreated;

    public List<Furniture> Furnitures;

    private Dictionary<string, Furniture> furnitureObjectPrototypes;

    public World(int width = 100, int height = 100)
    {
        
        SetupWorld(width, height);

        Debug.Log("World created with " + (width*height) + " tiles.");

        CreateFurniturePrototypes();
    }

    private void SetupWorld(int width, int height)
    {
        WorldInstance = this;

        Width = width;
        Height = height;

        tiles = new Tile[width, height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(x, y);
                tiles[x, y].TileTypeChanged += OnTileChanged;
            }
        }

        CreateFurniturePrototypes();

        Furnitures = new List<Furniture>();
    }

    private void CreateFurniturePrototypes()
    {
        furnitureObjectPrototypes = new Dictionary<string, Furniture>();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Data");
        filePath = Path.Combine(filePath, "Furniture.xml");
        string furnitureXmlText = File.ReadAllText(filePath);

        XmlTextReader reader = new XmlTextReader(new StringReader(furnitureXmlText));

        int furnCount = 0;
        if (reader.ReadToDescendant("Furnitures"))
        {
            if (reader.ReadToDescendant("Furniture"))
            {
                do
                {
                    furnCount++;

                    Furniture furn = new Furniture();
                    furn.ReadXmlPrototype(reader);

                    furnitureObjectPrototypes[furn.ObjectType] = furn;
                } while (reader.ReadToNextSibling("Furniture"));
            }
            else
            {
                Debug.LogError("The furniture prototype definition file doesn't have any 'Furniture' elements.");
            }
        }
        else
        {
            Debug.LogError("Did not find a 'Furniture' element in the prototype definition file.");
        }

        Debug.Log("Furniture prototypes read: " + furnCount);
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

    public Furniture PlaceFurniture(string objectType, Tile tile)
    {
        // TODO: This function assumes 1x1 tiles -- change this later

        if (furnitureObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("furnitureObjectPrototypes doesn't contain a proto for key: " + objectType);
            return null;
        }

        var furn = Furniture.PlaceInstance(furnitureObjectPrototypes[objectType], tile);

        if (furn == null)
        {
            // Failed to place object -- most likely an installed object was already placed
            return null;
        }

        if (FurnitureCreated != null)
        {
            FurnitureCreated(furn);
        }

        return furn;
    }

    public void OnTileChanged(Tile t)
    {
        if (TileChanged == null)
            return;

        TileChanged(t);
    }

    public Furniture GetFurniturePrototype(string objectType)
    {
        if (furnitureObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("No furniture with type: " + objectType);
            return null;
        }

        return furnitureObjectPrototypes[objectType];
    }
}
