using System.Collections.Generic;
using System.IO;
using System.Xml;
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

            PlaceRooms();
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

        private void PlaceRooms()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "Data");
            filePath = Path.Combine(filePath, "StationTemplate.xml");
            string stationTemplateXmlText = File.ReadAllText(filePath);

            var reader = new XmlTextReader(new StringReader(stationTemplateXmlText));
            
            if (reader.ReadToDescendant("Rooms"))
            {
                if (reader.ReadToDescendant("Room"))
                {
                    do { 
                        // Since it's "heuristically impossible" for the reader
                        // to be null, it's better to disable this particular warning
                        // ReSharper disable AssignNullToNotNullAttribute
                        int width = int.Parse(reader.GetAttribute("width"));
                        int height = int.Parse(reader.GetAttribute("height"));
                        int anchorX = int.Parse(reader.GetAttribute("anchorX"));
                        int anchorY = int.Parse(reader.GetAttribute("anchorY"));
                        for (int x = anchorX; x < width + anchorX; x++)
                        {
                            for (int y = anchorY; y < height + anchorY; y++)
                            {
                                World.WorldInstance.GetTileAt(x, y).Type = TileType.Floor;
                            }
                        }

                        var roomReader = reader.ReadSubtree();
                        
                        if (roomReader.ReadToDescendant("Furniture"))
                        {
                            do
                            {
                                string objectType = roomReader.GetAttribute("objectType");
                                int x = int.Parse(roomReader.GetAttribute("x")) + anchorX;
                                int y = int.Parse(roomReader.GetAttribute("y")) + anchorY;

                                var roomTile = World.WorldInstance.GetTileAt(x, y);
                                roomTile.Type = TileType.Floor;

                                World.WorldInstance.PlaceFurniture(objectType, roomTile);
                            } while (roomReader.ReadToNextSibling("Furniture"));
                        }

                    } while (reader.ReadToNextSibling("Room"));
                }
                else
                {
                    Debug.LogError("The room definitions file doesn't have any 'Room' elements.");
                }
            }
            else
            {
                Debug.LogError("Did not find a 'Rooms' element in the template definition file.");
            }
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
