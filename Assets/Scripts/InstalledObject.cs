using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Installed objects are things like Walls, Doors, Furniture, etc.
/// </summary>
public class InstalledObject
{
    /// <summary>
    /// The base tile of the object. The InstalledObject sits on the tile, but may be larger or
    /// smaller than the width/height of the base tile.
    /// </summary>
    private Tile tile;

    /// <summary>
    /// The objectType will be queried by the visual system to determine what sprite to
    /// render on the screen.
    /// </summary>
    private string _objectType;

    /// <summary>
    /// Multiplier for entities that are moving across the object. InstalledObjects with
    /// movementCosts of greater than 1F will slow down entities, whereas others with
    /// costs less than 1F (but greater than 0) will speed up. A movementCost of 0F means
    /// that the environment piece is impassable.
    /// </summary>
    private float _movementCost;

    private int _width;
    private int _height;

    protected InstalledObject()
    {
        
    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        var obj = new InstalledObject();

        obj._objectType = objectType;
        obj._movementCost = movementCost;
        obj._width = width;
        obj._height = height;

        return obj;
    }

    public static InstalledObject PlaceInstance(InstalledObject prototype, Tile tile)
    {
        var obj = new InstalledObject();

        obj._objectType = prototype._objectType;
        obj._movementCost = prototype._movementCost;
        obj._width = prototype._width;
        obj._height = prototype._height;
        obj.tile = tile;

        if (!tile.PlaceObject(obj))
        {
            // For some reason, we weren't able to place the object on the tile
            // Usually because the tile was already occupied

            return null;
        }

        return obj;
    }
}
