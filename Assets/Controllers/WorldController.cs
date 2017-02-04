using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

namespace Controllers
{
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; protected set; }

        public Sprite FloorSprite;

        private Dictionary<Tile, GameObject> tileGameObjectMap;

        public World World { get; protected set; }

        // Use this for initialization
        void Start ()
        {
            if (Instance != null)
            {
                Debug.LogError("There should never be two world controllers.");
            }
            Instance = this;

            // Create a world with Empty tiles
            World = new World();

            tileGameObjectMap = new Dictionary<Tile, GameObject>();

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
            World.update(Time.deltaTime);
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
        
    }
}
