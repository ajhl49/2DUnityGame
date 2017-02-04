using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class TileSpriteController : MonoBehaviour
    {

        private Dictionary<Tile, GameObject> _tileGameObjectMap;

        private World _world
        {
            get { return WorldController.Instance.World; }
        }

        // Use this for initialization
        void Start () {
            _tileGameObjectMap = new Dictionary<Tile, GameObject>();

            for (int x = 0; x < _world.Width; x++)
            {
                for (int y = 0; y < _world.Height; y++)
                {
                    var tileData = _world.GetTileAt(x, y);

                    var tileGameObject = new GameObject();

                    _tileGameObjectMap.Add(tileData, tileGameObject);

                    tileGameObject.name = "Tile_" + x + "_" + y;
                    tileGameObject.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                    tileGameObject.transform.SetParent(transform, true);

                    var spriteRenderer = tileGameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = SpriteManager.SpriteManagerInstance.GetSprite("Tile", "Empty");
                    spriteRenderer.sortingLayerName = "Floor";

                    OnTileChanged(tileData);
                }
            }

            _world.TileChanged += OnTileChanged;
        }

        private void OnTileChanged(Tile tileData)
        {
            if (_tileGameObjectMap.ContainsKey(tileData) == false)
            {
                Debug.LogError("tileGameObjectMap doesn't contain the tileData");
                return;
            }

            var tileGameObject = _tileGameObjectMap[tileData];

            if (tileGameObject == null)
            {
                Debug.LogError("tileGameObjectMap returned null GameObject");
                return;
            }

            if (tileData.Type == TileType.Floor)
                tileGameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.SpriteManagerInstance.GetSprite("Tile", "construction_floors_1");
            else if (tileData.Type == TileType.Empty)
                tileGameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.SpriteManagerInstance.GetSprite("Tile", "Empty");
            else
                Debug.LogError("OnTileTypeChanged -- Unrecognized tile type");
        }
    }
}
