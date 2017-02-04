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

    public bool IsNeighbor(Tile tile, bool diagonalOkay = false)
    {
        return Mathf.Abs(X - tile.X) + Mathf.Abs(Y - tile.Y) == 1 ||
               (diagonalOkay && (Mathf.Abs(X - tile.X) == 1 && Mathf.Abs(Y - tile.Y) == 1));
    }

    public Tile[] GetNeighbors(bool diagonalOkay = false)
    {
        Tile[] neighbors;

        if (diagonalOkay == false)
        {
            neighbors = new Tile[4];
        }
        else
        {
            neighbors = new Tile[8];
        }

        Tile currentNeighbor;

        currentNeighbor = World.WorldInstance.GetTileAt(X, Y + 1);
        neighbors[0] = currentNeighbor;
        currentNeighbor = World.WorldInstance.GetTileAt(X + 1, Y);
        neighbors[1] = currentNeighbor;
        currentNeighbor = World.WorldInstance.GetTileAt(X, Y - 1);
        neighbors[2] = currentNeighbor;
        currentNeighbor = World.WorldInstance.GetTileAt(X - 1, Y);
        neighbors[3] = currentNeighbor;

        if (diagonalOkay)
        {
            currentNeighbor = World.WorldInstance.GetTileAt(X + 1, Y + 1);
            neighbors[4] = currentNeighbor;
            currentNeighbor = World.WorldInstance.GetTileAt(X + 1, Y - 1);
            neighbors[5] = currentNeighbor;
            currentNeighbor = World.WorldInstance.GetTileAt(X - 1, Y - 1);
            neighbors[6] = currentNeighbor;
            currentNeighbor = World.WorldInstance.GetTileAt(X - 1, Y + 1);
            neighbors[7] = currentNeighbor;
        }

        return neighbors;
    }

    public Tile North()
    {
        return World.WorldInstance.GetTileAt(X, Y + 1);
    }

    public Tile South()
    {
        return World.WorldInstance.GetTileAt(X, Y - 1);
    }

    public Tile East()
    {
        return World.WorldInstance.GetTileAt(X + 1, Y);
    }

    public Tile West()
    {
        return World.WorldInstance.GetTileAt(X - 1, Y);
    }
}
