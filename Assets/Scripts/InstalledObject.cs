using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Installed objects are things like Walls, Doors, Furniture, etc.
/// </summary>
public class InstalledObject
{
    public Tile Tile { get; protected set; }

    public string ObjectType { get; protected set; }

    private Action<InstalledObject> cbOnChanged;

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

    protected InstalledObject()
    {
        LinksToNeighbor = false;
    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
    {
        var obj = new InstalledObject();

        obj.ObjectType = objectType;
        obj._movementCost = movementCost;
        obj._width = width;
        obj._height = height;
        obj.LinksToNeighbor = linksToNeighbor;

        return obj;
    }

    public static InstalledObject PlaceInstance(InstalledObject prototype, Tile tile)
    {
        var obj = new InstalledObject();

        obj.ObjectType = prototype.ObjectType;
        obj._movementCost = prototype._movementCost;
        obj._width = prototype._width;
        obj._height = prototype._height;
        obj.LinksToNeighbor = prototype.LinksToNeighbor;
        obj.Tile = tile;

        if (!tile.PlaceObject(obj))
        {
            // For some reason, we weren't able to place the object on the tile
            // Usually because the tile was already occupied

            return null;
        }

        return obj;
    }

    public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunction)
    {
        cbOnChanged += callbackFunction;
    }

    public void UnregisterOnChangedCallback(Action<InstalledObject> callbackFunction)
    {
        cbOnChanged -= callbackFunction;
    }
}
