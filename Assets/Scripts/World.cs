using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class World
{
    public static World WorldInstance { get; protected set; }

    private Tile[,] _tiles;

    public int Width { get; private set; }

    public int Height { get; private set; }
    
    public event Action<Tile> TileChanged;
    public event Action<Furniture> FurnitureCreated;

    public List<Furniture> Furnitures;

    private Dictionary<string, Furniture> _furnitureObjectPrototypes;

    public World(int width = 100, int height = 100)
    {
        
        SetupWorld(width, height);

        CreateFurniturePrototypes();
    }

    private void SetupWorld(int width, int height)
    {
        WorldInstance = this;

        Width = width;
        Height = height;

        _tiles = new Tile[width, height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _tiles[x, y] = new Tile(x, y);
                _tiles[x, y].TileTypeChanged += OnTileChanged;
            }
        }

        CreateFurniturePrototypes();

        Furnitures = new List<Furniture>();
    }

    

    private void CreateFurniturePrototypes()
    {
        _furnitureObjectPrototypes = new Dictionary<string, Furniture>();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Data");
        filePath = Path.Combine(filePath, "Furniture.xml");
        string furnitureXmlText = File.ReadAllText(filePath);

        var reader = new XmlTextReader(new StringReader(furnitureXmlText));

        int furnCount = 0;
        if (reader.ReadToDescendant("Furnitures"))
        {
            if (reader.ReadToDescendant("Furniture"))
            {
                do
                {
                    furnCount++;

                    var furn = new Furniture();
                    furn.ReadXmlPrototype(reader);

                    _furnitureObjectPrototypes[furn.ObjectType] = furn;
                } while (reader.ReadToNextSibling("Furniture"));
            }
            else
            {
                Debug.LogError("The furniture prototype definition file doesn't have any 'Furniture' elements.");
            }
        }
        else
        {
            Debug.LogError("Did not find a 'Furnitures' element in the prototype definition file.");
        }

        Debug.Log("Furniture prototypes read: " + furnCount);
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Tile (" + x  + "," + y + ") is out of range");
            return null;
        }
        return _tiles[x, y];
    }

    public Furniture PlaceFurniture(string objectType, Tile tile)
    {

        if (!_furnitureObjectPrototypes.ContainsKey(objectType))
        {
            Debug.LogError("furnitureObjectPrototypes doesn't contain a proto for key: " + objectType);
            return null;
        }

        var furn = Furniture.PlaceInstance(_furnitureObjectPrototypes[objectType], tile);

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
        if (_furnitureObjectPrototypes.ContainsKey(objectType)) return _furnitureObjectPrototypes[objectType];
        Debug.LogError("No furniture with type: " + objectType);
        return null;
    }
}
