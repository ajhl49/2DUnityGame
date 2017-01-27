using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class World
{
    Tile[,] tiles;
    private int _width;
    private int _height;

    public int Width
    {
        get { return _width; }
    }

    public int Height
    {
        get { return _height; }
    }

    public World(int width = 100, int height = 100)
    {
        this._width = width;
        this._height = height;

        tiles = new Tile[width,height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x,y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (width*height) + " tiles.");
    }

    public void RandomizeTiles()
    {
        Debug.Log("RandomizeTiles");
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    tiles[x, y].Type = Tile.TileType.Empty;
                }
                else
                {
                    tiles[x, y].Type = Tile.TileType.Floor;
                }
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x > _width || x < 0 || y > _height || y < 0)
        {
            Debug.LogError("Tile (" + x  + "," + y + ") is out of range");
            return null;
        }
        return tiles[x, y];
    }
}
