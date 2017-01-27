using UnityEngine;

namespace Controllers
{
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; protected set; }

        public Sprite FloorSprite;

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

            // Create a GameObject for each of our tiles, so they show visually.

            for (int x = 0; x < World.Width; x++)
            {
                for (int y = 0; y < World.Height; y++)
                {
                    var tileData = World.GetTileAt(x, y);

                    var tileGo = new GameObject();
                    tileGo.name = "Tile_" + x + "_" + y;
                    tileGo.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                    tileGo.transform.SetParent(this.transform, true);

                    // Add a sprite renderer, but don't bother setting a sprite
                    // because all of the tiles are empty right now
                    tileGo.AddComponent <SpriteRenderer>();

                    tileData.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tileGo); } );
                }
            }

            World.RandomizeTiles();
        }
	
        // Update is called once per frame
        void Update ()
        {
        }

        private void OnTileTypeChanged(Tile tileData, GameObject tileGameObject)
        {
            if (tileData.Type == Tile.TileType.Floor)
            {
                tileGameObject.GetComponent<SpriteRenderer>().sprite = FloorSprite;
            }
            else if (tileData.Type == Tile.TileType.Empty)
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
