using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class FurnitureSpriteController : MonoBehaviour
    {
        private Dictionary<Furniture, GameObject> _furnitureGameObjectMap;

        private World World
        {
            get { return WorldController.Instance.World; }
        }

        private void Start()
        {
            _furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
        
            World.FurnitureCreated += OnFurnitureCreated;

            foreach (var furn in World.Furnitures)
            {
                OnFurnitureCreated(furn);
            }
        }

        public void OnFurnitureCreated(Furniture furn)
        {
            var furnitureGameObject = new GameObject();

            _furnitureGameObjectMap.Add(furn, furnitureGameObject);

            furnitureGameObject.name = furn.ObjectType + "_" + furn.Tile.X + "_" + furn.Tile.Y;
            furnitureGameObject.transform.position = new Vector3(furn.Tile.X, furn.Tile.Y, 0);
            furnitureGameObject.transform.SetParent(transform, true);

            var spriteRenderer = furnitureGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetSpriteForFurniture(furn);
            spriteRenderer.sortingLayerName = "Furniture";

            furn.FurnitureChanged += OnFurnitureChanged;
        }

        private void OnFurnitureChanged(Furniture furn)
        {
            if (_furnitureGameObjectMap.ContainsKey(furn) == false)
            {
                Debug.LogError("OnFurnitureChanged -- trying to change visuals for furniture not in our map");
                return;
            }

            var furnGameObject = _furnitureGameObjectMap[furn];

            furnGameObject.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
        }

        public Sprite GetSpriteForFurniture(Furniture furn)
        {
            string spriteName = furn.ObjectType;

            if (furn.LinksToNeighbor == false)
            {
                Debug.Log(spriteName);
                return SpriteManager.SpriteManagerInstance.GetSprite("Furniture", spriteName);
            }

            spriteName = furn.ObjectType + "_";

            int x = furn.Tile.X;
            int y = furn.Tile.Y;

            var t = World.GetTileAt(x, y + 1);
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType)
            {
                spriteName += "N";
            }
            t = World.GetTileAt(x + 1, y);
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType)
            {
                spriteName += "E";
            }
            t = World.GetTileAt(x, y - 1);
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType)
            {
                spriteName += "S";
            }
            t = World.GetTileAt(x - 1, y);
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType)
            {
                spriteName += "W";
            }
            spriteName += "_1";

            return SpriteManager.SpriteManagerInstance.GetSprite("Furniture", spriteName);
        }

        public Sprite GetSpriteForFurniture(string objectType)
        {
            var sprite = SpriteManager.SpriteManagerInstance.GetSprite("Furniture", objectType);

            if (sprite == null)
            {
                sprite = SpriteManager.SpriteManagerInstance.GetSprite("Furniture", objectType + "_");
            }

            return sprite;
        }
    }
}
