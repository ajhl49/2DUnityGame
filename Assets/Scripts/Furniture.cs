using System;
using System.Xml;
using UnityEngine;

/// <summary>
/// Installed objects are things like Walls, Doors, Furniture, etc.
/// </summary>
public class Furniture
{
    public Tile Tile { get; protected set; }

    public string ObjectType { get; protected set; }

    public string Name
    {
        get
        {
            return string.IsNullOrEmpty(_name) ? ObjectType : _name;
        }
        set { _name = value; }
    }

    public float MovementCost { get; protected set; }

    public event Action<Furniture> FurnitureChanged;

    private string _name;

    private int _width;
    private int _height;

    public bool LinksToNeighbor { get; protected set; }

    private Func<Tile, bool> _funcPositionValidation;

    public Furniture()
    {
        _funcPositionValidation = IsValidPosition;
        _height = 1;
        _width = 1;
    }

    protected Furniture(Furniture other)
    {
        ObjectType = other.ObjectType;
        Name = other.Name;
        MovementCost = other.MovementCost;
        _width = other._width;
        _height = other._height;
        LinksToNeighbor = other.LinksToNeighbor;

        if (other._funcPositionValidation != null)
            _funcPositionValidation = (Func<Tile, bool>) other._funcPositionValidation.Clone();
    }

    public virtual Furniture Clone()
    {
        return new Furniture(this);
    }

    public static Furniture PlaceInstance(Furniture prototype, Tile tile)
    {
        if (!prototype._funcPositionValidation(tile))
        {
            Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE.");
            return null;
        }

        var obj = prototype.Clone();
        obj.Tile = tile;

        if (!tile.PlaceFurniture(obj))
        {
            // For some reason, we weren't able to place the object on the tile
            // Usually because the tile was already occupied

            return null;
        }

        if (obj.LinksToNeighbor)
        {
            // This type of furniture links itself to its neighbors
            // Trigger neighbor's OnChangedCallback
            int x = tile.X;
            int y = tile.Y;

            var currentTile = World.WorldInstance.GetTileAt(x, y + 1);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.FurnitureChanged(currentTile.Furniture);
            }

            currentTile = World.WorldInstance.GetTileAt(x + 1, y);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.FurnitureChanged(currentTile.Furniture);
            }

            currentTile = World.WorldInstance.GetTileAt(x, y - 1);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.FurnitureChanged(currentTile.Furniture);
            }

            currentTile = World.WorldInstance.GetTileAt(x - 1, y);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.FurnitureChanged(currentTile.Furniture);
            }
        }

        return obj;
    }

    public bool IsValidPosition(Tile t)
    {
        // Make sure tile is type Floor
        if (t.Type != TileType.Floor)
        {
            return false;
        }

        // Make sure tile doesn't already have furniture
        if (t.Furniture != null)
        {
            return false;
        }

        return true;
    }

    public bool IsValidPosition_Door(Tile t)
    {
        if (IsValidPosition(t) == false)
            return false;

        return true;
    }

    public void ReadXmlPrototype(XmlReader readerParent)
    {
        ObjectType = readerParent.GetAttribute("objectType");

        var reader = readerParent.ReadSubtree();

        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "Name":
                    reader.Read();
                    Name = reader.ReadContentAsString();
                    break;
                case "MovementCost":
                    reader.Read();
                    MovementCost = reader.ReadContentAsFloat();
                    break;
                case "Width":
                    reader.Read();
                    _width = reader.ReadContentAsInt();
                    break;
                case "Height":
                    reader.Read();
                    _height = reader.ReadContentAsInt();
                    break;
                case "LinksToNeighbors":
                    reader.Read();
                    LinksToNeighbor = reader.ReadContentAsBoolean();
                    break;
            }
        }
    }
}
