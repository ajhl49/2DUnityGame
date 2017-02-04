using System;
using UnityEngine;

public enum TileType { Empty, Floor }

public class Tile
{
    private const float BaseMovementCost = 1f;

    public event Action<Tile> TileTypeChanged;

    public Furniture Furniture { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public TileType Type
    {
        get { return _type; }
        set
        {
            var oldType = _type;
            _type = value;
            // Call the callback and let things know we've changed.
            if (TileTypeChanged != null && oldType != _type) TileTypeChanged(this);
        }
    }

    public float MovementCost
    {
        get
        {
            if (Type == TileType.Empty)
                return 0;

            if (Furniture == null)
                return BaseMovementCost;

            return BaseMovementCost * Furniture.MovementCost;
        }
    }

    private TileType _type = TileType.Empty;

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool PlaceFurniture(Furniture objectInstance)
    {
        if (objectInstance == null)
        {
            // We are uninstalling the current installed object
            Furniture = null;
            return true;
        }

        if (objectInstance.IsValidPosition(this) == false)
        {
            Debug.LogError("Furniture assignment not valid.");
            return false;
        }

        Furniture = objectInstance;
        return true;
    }
}
