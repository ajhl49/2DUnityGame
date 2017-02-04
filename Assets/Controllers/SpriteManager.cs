using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Controllers
{
    public class SpriteManager : MonoBehaviour
    {

        public static SpriteManager SpriteManagerInstance;

        private Dictionary<string, Sprite> _sprites;

        void OnEnable()
        {
            SpriteManagerInstance = this;

            LoadSprites();
        }

        private void LoadSprites()
        {
            _sprites = new Dictionary<string, Sprite>();

            string filePath = Path.Combine(Application.streamingAssetsPath, "Images");

            LoadSpritesFromDirectory(filePath);
        }

        private void LoadSpritesFromDirectory(string filePath)
        {
            var subDirectories = Directory.GetDirectories(filePath);
            foreach (string subDirectory in subDirectories)
            {
                LoadSpritesFromDirectory(subDirectory);
            }

            var filesInDirectory = Directory.GetFiles(filePath);
            foreach (string file in filesInDirectory)
            {
                string spriteCategory = new DirectoryInfo(filePath).Name;

                LoadImage(spriteCategory, file);
            }
        }

        private void LoadImage(string spriteCategory, string filePath)
        {
            if (filePath.Contains(".xml") || filePath.Contains(".meta"))
                return;

            var imageBytes = File.ReadAllBytes(filePath);

            var imageTexture = new Texture2D(2, 2);

            if (imageTexture.LoadImage(imageBytes))
            {
                string baseSpriteName = Path.GetFileNameWithoutExtension(filePath);
                string basePath = Path.GetDirectoryName(filePath);

                string xmlPath = Path.Combine(basePath, baseSpriteName + ".xml");

                if (File.Exists(xmlPath))
                {
                    string xmlText = File.ReadAllText(xmlPath);

                    var reader = new XmlTextReader(new StringReader(xmlText));

                    if (reader.ReadToDescendant("Sprites") && reader.ReadToDescendant("Sprite"))
                    {
                        do
                        {
                            ReadSpriteFromXml(spriteCategory, reader, imageTexture);
                        } while (reader.ReadToNextSibling("Sprite"));
                    }
                    else
                    {
                        Debug.LogError("Cound not find matching <Sprite> tag.");
                    }
                }
                else
                {
                    LoadSprite(spriteCategory, baseSpriteName, imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), 32);
                }
            }
        }

        private void ReadSpriteFromXml(string spriteCategory, XmlReader reader, Texture2D imageTexture)
        {
            if (reader == null)
            {
                Debug.LogError("ReadSpriteFromXml -- XmlReader was null.");
                return;
            }
            string spriteName = reader.GetAttribute("name");
            // Null value is "heuristically impossible", according to ReSharper
            // ReSharper disable AssignNullToNotNullAttribute
            int x = int.Parse(reader.GetAttribute("x"));
            int y = int.Parse(reader.GetAttribute("y"));
            int w = int.Parse(reader.GetAttribute("w"));
            int h = int.Parse(reader.GetAttribute("h"));
            int pixelsPerUnit = int.Parse(reader.GetAttribute("pixelsPerUnit"));

            LoadSprite(spriteCategory, spriteName, imageTexture, new Rect(x, y, w, h), pixelsPerUnit);
        }

        private void LoadSprite(string spriteCategory, string spriteName, Texture2D imageTexture, Rect spriteCoordinates, int pixelsPerUnit)
        {
            spriteName = spriteCategory + "/" + spriteName;

            var pivotPoint = new Vector2(0f, 0f);

            var loadedSprite = Sprite.Create(imageTexture, spriteCoordinates, pivotPoint, pixelsPerUnit);

            _sprites[spriteName] = loadedSprite;
        }

        public Sprite GetSprite(string categoryName, string spriteName)
        {
            spriteName = categoryName + "/" + spriteName;

            return _sprites.ContainsKey(spriteName) == false ? null : _sprites[spriteName];
        }
    }
}
