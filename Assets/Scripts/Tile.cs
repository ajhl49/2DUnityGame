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
    public float movementCost
    {
        get
        {

            if (Type == TileType.Empty)
                return 0;   // 0 is unwalkable

            return 1 ;
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
    // Tells us if two tiles are adjacent.
    public bool IsNeighbour(Tile tile, bool diagOkay = false)
    {
        // Check to see if we have a difference of exactly ONE between the two
        // tile coordinates.  Is so, then we are vertical or horizontal neighbours.
        return
            Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1 ||  // Check hori/vert adjacency
            (diagOkay && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1)) // Check diag adjacency
            ;
    }
    public Tile[] GetNeighbours(bool diagOkay = false)
    {
        Tile[] ns;

        if (diagOkay == false)
        {
            ns = new Tile[4];   // Tile order: N E S W
        }
        else
        {
            ns = new Tile[8];   // Tile order : N E S W NE SE SW NW
        }

        Tile n;

        n = _world.GetTileAt(X, Y + 1);
        ns[0] = n;  // Could be null, but that's okay.
        n = _world.GetTileAt(X + 1, Y);
        ns[1] = n;  // Could be null, but that's okay.
        n = _world.GetTileAt(X, Y - 1);
        ns[2] = n;  // Could be null, but that's okay.
        n = _world.GetTileAt(X - 1, Y);
        ns[3] = n;  // Could be null, but that's okay.

        if (diagOkay == true)
        {
            n = _world.GetTileAt(X + 1, Y + 1);
            ns[4] = n;  // Could be null, but that's okay.
            n = _world.GetTileAt(X + 1, Y - 1);
            ns[5] = n;  // Could be null, but that's okay.
            n = _world.GetTileAt(X - 1, Y - 1);
            ns[6] = n;  // Could be null, but that's okay.
            n = _world.GetTileAt(X - 1, Y + 1);
            ns[7] = n;  // Could be null, but that's okay.
        }

        return ns;
    }

}
