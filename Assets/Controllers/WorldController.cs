using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; protected set; }

        public Sprite FloorSprite;

        private Dictionary<Tile, GameObject> tileGameObjectMap;

        private Dictionary<InstalledObject, GameObject> installedObjectMap;

        private Dictionary<string, Sprite> installedObjectSprites;

        public World World { get; protected set; }

        // Use this for initialization
        void Start ()
        {
            installedObjectSprites = new Dictionary<string, Sprite>();
            Sprite[] constructionWallSprites = Resources.LoadAll<Sprite>("Sprites/SS13Assets/icons/turf/construction_walls");

            foreach (var sprite in constructionWallSprites)
            {
                installedObjectSprites[sprite.name] = sprite;
            }

            if (Instance != null)
            {
                Debug.LogError("There should never be two world controllers.");
            }
            Instance = this;

            // Create a world with Empty tiles
            World = new World();

            World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

            tileGameObjectMap = new Dictionary<Tile, GameObject>();
            installedObjectMap = new Dictionary<InstalledObject, GameObject>();

            // Create a GameObject for each of our tiles, so they show visually.

            for (int x = 0; x < World.Width; x++)
            {
                for (int y = 0; y < World.Height; y++)
                {
                    var tileData = World.GetTileAt(x, y);

                    var tileGo = new GameObject();

                    tileGameObjectMap.Add(tileData, tileGo);

                    tileGo.name = "Tile_" + x + "_" + y;
                    tileGo.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                    tileGo.transform.SetParent(transform, true);

                    // Add a sprite renderer, but don't bother setting a sprite
                    // because all of the tiles are empty right now
                    tileGo.AddComponent <SpriteRenderer>();
                    
                    tileData.RegisterTileTypeChangedCallback( OnTileTypeChanged );
                }
            }

            World.RandomizeTiles();
        }
	
        // Update is called once per frame
        void Update ()
        {
        }

        private void DestroyAllTileGameObjects()
        {
            // Called during level/floor changes
            // All GameObjects, but not the tiles, must be destroyed

            while (tileGameObjectMap.Count > 0)
            {
                var tileData = tileGameObjectMap.Keys.First();
                var tileGo = tileGameObjectMap[tileData];

                tileGameObjectMap.Remove(tileData);

                tileData.UnregisterRegisterTileTypeChangedCallback( OnTileTypeChanged );

                Destroy(tileGo);
            }
        }

        private void OnTileTypeChanged(Tile tileData)
        {

            if (tileGameObjectMap.ContainsKey(tileData) == false)
            {
                Debug.LogError("tileGameObjectMap doesn't contain the tileData -- was the tile added to the dictionary?");
                return;
            }

            var tileGameObject = tileGameObjectMap[tileData];

            if (tileGameObject == null)
            {
                Debug.LogError("tileGameObjectMap returned null GameObject -- was the tile added to the dictionary? Was an unregister performed?");
                return;
            }

            if (tileData.Type == TileType.Floor)
            {
                tileGameObject.GetComponent<SpriteRenderer>().sprite = FloorSprite;
            }
            else if (tileData.Type == TileType.Empty)
            {
                tileGameObject.GetComponent<SpriteRenderer>().sprite = null;
            }
            else
            {
                Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
            }
        }

        public Tile GetTileAtWorldCoord(Vector3 coord)
        {
            int x = Mathf.FloorToInt(coord.x);
            int y = Mathf.FloorToInt(coord.y);

            return World.GetTileAt(x, y);
        }

        public void OnInstalledObjectCreated(InstalledObject installedObject)
        {
            // Create a visual GameObject linked to this data.

            // TODO: Does not consider multi-tile objects nor rotated objects

            var gameObject = new GameObject();
            
            installedObjectMap.Add(installedObject, gameObject);

            gameObject.name = installedObject.ObjectType + "_" + installedObject.Tile.X + "_" + installedObject.Tile.Y;
            gameObject.transform.position = new Vector3(installedObject.Tile.X, installedObject.Tile.Y, 0);
            gameObject.transform.SetParent(transform, true);

            // Currently assuming it to be a wall sprite
            gameObject.AddComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(installedObject);

            installedObject.RegisterOnChangedCallback(OnInstalledObjectChanged);
        }

        private Sprite GetSpriteForInstalledObject(InstalledObject installedObject)
        {
            if (installedObject.LinksToNeighbor == false)
            {
                return installedObjectSprites[installedObject.ObjectType];
            }

            // Otherwise, the sprite name is more complicated

            string spriteName = installedObject.ObjectType + "_";

            // Check for neighbors North, South, East, West
            int x = installedObject.Tile.X;
            int y = installedObject.Tile.Y;

            Tile currentTile;
            currentTile = World.GetTileAt(x, y + 1);
            if (currentTile != null && currentTile.InstalledObject != null && currentTile.InstalledObject.ObjectType == installedObject.ObjectType)
            {
                spriteName += "N";
            }

            currentTile = World.GetTileAt(x + 1, y);
            if (currentTile != null && currentTile.InstalledObject != null && currentTile.InstalledObject.ObjectType == installedObject.ObjectType)
            {
                spriteName += "E";
            }

            currentTile = World.GetTileAt(x, y - 1);
            if (currentTile != null && currentTile.InstalledObject != null && currentTile.InstalledObject.ObjectType == installedObject.ObjectType)
            {
                spriteName += "S";
            }

            currentTile = World.GetTileAt(x - 1, y);
            if (currentTile != null && currentTile.InstalledObject != null && currentTile.InstalledObject.ObjectType == installedObject.ObjectType)
            {
                spriteName += "W";
            }

            spriteName += "_1";

            if (installedObjectSprites.ContainsKey(spriteName) == false)
            {
                Debug.LogError("GetSpriteForInstalledObject -- No sprites with name: " + spriteName);
                return null;
            }

            return installedObjectSprites[spriteName];
        }

        private void OnInstalledObjectChanged(InstalledObject installedObject)
        {
            Debug.LogError("OnInstalledObjectChanged -- NOT IMPLEMENTED");
        }
    }
}
