using System;
using UnityEngine;


public enum TileType { Empty, Floor }

public class Tile {
    
    private TileType _type = TileType.Empty;

    private Action<Tile> _cbTileTypeChanged;

    // Reference to any object that isn't nailed down and can be picked up (wrench, gun, etc.)
    private LooseObject looseObject;
    // Single object that is installed on the tile, like a door, bookshelf, etc.
    private InstalledObject installedObject;

    public World _world;
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
            if (_cbTileTypeChanged != null && oldType != _type) _cbTileTypeChanged(this);
        }
    }

    public Tile(World world, int x, int y)
    {
        _world = world;
        X = x;
        Y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        _cbTileTypeChanged += callback;
    }

    public void UnregisterRegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        if (_cbTileTypeChanged != null) _cbTileTypeChanged -= callback;
    }

    public bool PlaceObject(InstalledObject objectInstance)
    {
        if (objectInstance == null)
        {
            // We are uninstalling the current installed object
            installedObject = null;
            return true;
        }

        if (installedObject != null)
        {
            Debug.LogError("Trying to assign an installed object to a tile that already has one!");
            return false;
        }

        installedObject = objectInstance;
        return true;
    }
    //if two tiles are adjacent
    public bool IsNeighbour(Tile tile, bool DiagOK = false)
    {
        if (this.X == tile.X && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
            return true;
        if (this.Y == tile.Y && (this.X == tile.X + 1 || this.X == tile.X - 1))
            return true;
        if (DiagOK)
        {
            if (this.X == tile.X+1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
                return true;
            if (this.X == tile.X-1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
                return true;
        }

        return false;

    }
}
