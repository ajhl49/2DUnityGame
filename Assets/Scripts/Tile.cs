using System;

public class Tile {

    public enum TileType { Empty, Floor}

    private TileType _type = TileType.Empty;

    private Action<Tile> _cbTileTypeChanged;

    // Reference to any object that isn't nailed down and can be picked up (wrench, gun, etc.)
    private LooseObject looseObject;
    // Single object that is installed on the tile, like a door, bookshelf, etc.
    private InstalledObject installedObject;

    private World _world;
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
}
