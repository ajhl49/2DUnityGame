using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; protected set; }

        public Sprite FloorSprite;
        public Sprite EmptySprite;

        private Dictionary<Tile, GameObject> tileGameObjectMap;

        private Dictionary<Furniture, GameObject> furnitureObjectMap;

        private Dictionary<string, Sprite> furnitureObjectSprites;

        public World World { get; protected set; }

        // Use this for initialization
        void Start ()
        {
            LoadSprites();

            if (Instance != null)
            {
                Debug.LogError("There should never be two world controllers.");
            }
            Instance = this;

            // Create a world with Empty tiles
            World = new World();

            World.RegisterInstalledObjectCreated(OnFurnitureCreated);

            tileGameObjectMap = new Dictionary<Tile, GameObject>();
            furnitureObjectMap = new Dictionary<Furniture, GameObject>();

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

            // Center the Camera
            Camera.main.transform.position = new Vector3(World.Width/2, World.Height/2, Camera.main.transform.position.z);

            //World.RandomizeTiles();
        }

        private void LoadSprites()
        {
            furnitureObjectSprites = new Dictionary<string, Sprite>();
            Sprite[] constructionWallSprites = Resources.LoadAll<Sprite>("Sprites/SS13Assets/icons/turf/construction_walls");

            foreach (var sprite in constructionWallSprites)
            {
                furnitureObjectSprites[sprite.name] = sprite;
            }
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

        public void OnFurnitureCreated(Furniture furniture)
        {
            // Create a visual GameObject linked to this data.

            // TODO: Does not consider multi-tile objects nor rotated objects

            var furnitureGameObject = new GameObject();
            
            furnitureObjectMap.Add(furniture, furnitureGameObject);

            furnitureGameObject.name = furniture.ObjectType + "_" + furniture.Tile.X + "_" + furniture.Tile.Y;
            furnitureGameObject.transform.position = new Vector3(furniture.Tile.X, furniture.Tile.Y, 0);
            furnitureGameObject.transform.SetParent(transform, true);

            // Currently assuming it to be a wall sprite
            furnitureGameObject.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furniture);

            furniture.RegisterOnChangedCallback(OnFurnitureChanged);
        }

        private Sprite GetSpriteForFurniture(Furniture furniture)
        {
            if (furniture.LinksToNeighbor == false)
            {
                return furnitureObjectSprites[furniture.ObjectType];
            }

            // Otherwise, the sprite name is more complicated

            string spriteName = furniture.ObjectType + "_";

            // Check for neighbors North, South, East, West
            int x = furniture.Tile.X;
            int y = furniture.Tile.Y;

            Tile currentTile;
            currentTile = World.GetTileAt(x, y + 1);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == furniture.ObjectType)
            {
                spriteName += "N";
            }

            currentTile = World.GetTileAt(x + 1, y);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == furniture.ObjectType)
            {
                spriteName += "E";
            }

            currentTile = World.GetTileAt(x, y - 1);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == furniture.ObjectType)
            {
                spriteName += "S";
            }

            currentTile = World.GetTileAt(x - 1, y);
            if (currentTile != null && currentTile.Furniture != null && currentTile.Furniture.ObjectType == furniture.ObjectType)
            {
                spriteName += "W";
            }

            spriteName += "_1";

            if (furnitureObjectSprites.ContainsKey(spriteName) == false)
            {
                Debug.LogError("GetSpriteForFurniture -- No sprites with name: " + spriteName);
                return null;
            }

            return furnitureObjectSprites[spriteName];
        }

        private void OnFurnitureChanged(Furniture furniture)
        {

            // Make sure the furniture's graphics are correct.

            if (furnitureObjectMap.ContainsKey(furniture) == false)
            {
                Debug.LogError("OnFurnitureChanged -- trying to change visuals for furniture not in our map");
                return;
            }

            var furnitureGameObject = furnitureObjectMap[furniture];
            furnitureGameObject.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furniture);
        }
    }
}
