using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Installed objects are things like Walls, Doors, Furniture, etc.
/// </summary>
public class Furniture
{
    public Tile Tile { get; protected set; }

    public string ObjectType { get; protected set; }

    private Action<Furniture> cbOnChanged;

    /// <summary>
    /// Multiplier for entities that are moving across the object. InstalledObjects with
    /// movementCosts of greater than 1F will slow down entities, whereas others with
    /// costs less than 1F (but greater than 0) will speed up. A movementCost of 0F means
    /// that the environment piece is impassable.
    /// </summary>
    private float _movementCost;

    private int _width;
    private int _height;

    public bool LinksToNeighbor { get; protected set; }

    private Func<Tile, bool> funcPositionValidation;

    protected Furniture()
    {
        LinksToNeighbor = false;
    }

    public static Furniture CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
    {
        var obj = new Furniture();

        obj.ObjectType = objectType;
        obj._movementCost = movementCost;
        obj._width = width;
        obj._height = height;
        obj.LinksToNeighbor = linksToNeighbor;

        obj.funcPositionValidation = obj.IsValidPosition;

        return obj;
    }

    public static Furniture PlaceInstance(Furniture prototype, Tile tile)
    {
        if (prototype.funcPositionValidation(tile) == false)
        {
            Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE.");
            return null;
        }

        var obj = new Furniture();

        obj.ObjectType = prototype.ObjectType;
        obj._movementCost = prototype._movementCost;
        obj._width = prototype._width;
        obj._height = prototype._height;
        obj.LinksToNeighbor = prototype.LinksToNeighbor;
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

            Tile currentTile;
            currentTile = tile.World.GetTileAt(x, y + 1);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.cbOnChanged(currentTile.Furniture);
            }

            currentTile = tile.World.GetTileAt(x + 1, y);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.cbOnChanged(currentTile.Furniture);
            }

            currentTile = tile.World.GetTileAt(x, y - 1);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.cbOnChanged(currentTile.Furniture);
            }

            currentTile = tile.World.GetTileAt(x - 1, y);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == obj.ObjectType)
            {
                currentTile.Furniture.cbOnChanged(currentTile.Furniture);
            }
        }

        return obj;
    }

    public void RegisterOnChangedCallback(Action<Furniture> callbackFunction)
    {
        cbOnChanged += callbackFunction;
    }

    public void UnregisterOnChangedCallback(Action<Furniture> callbackFunction)
    {
        cbOnChanged -= callbackFunction;
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
        // Make sure we have a pair of E/W or N/S walls
        return true;
    }
}
