using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class MouseController : MonoBehaviour
    {
        public GameObject circleCursorPrefab;

        private bool buildModeIsObjects = false;
        TileType buildModeTile = TileType.Floor;
        private string buildModeObjectType;

        private Vector3 _currFramePosition;
        private Vector3 _lastFramePosition;
        private Vector3 _dragStartPosition;

        private List<GameObject> _dragPreviewGameObjects;

        // Use this for initialization
        void Start()
        {
            _dragPreviewGameObjects = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            _currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _currFramePosition.z = 0;

            //UpdateCursor();
            EditorTileDrag();
            UpdateCameraMovement();

            // Save mouse position from this frame
            // Redone since dragging causes sputtering due to desync (camera may have moved while Update was being called)
            _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _lastFramePosition.z = 0;
        }

        private void UpdateCameraMovement()
        {
            // Handle screen dragging
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) // Right or Middle Mouse Button
            {
                var diff = _lastFramePosition - _currFramePosition;
                //Debug.Log(diff);
                Camera.main.transform.Translate(diff);
            }

            Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 10f);
        }

        private void EditorTileDrag()
        {
            // If over UI element, BAIL
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Start Editor Drag
            if (Input.GetMouseButtonDown(0))
            {
                _dragStartPosition = _currFramePosition;
            }

            int startX = Mathf.FloorToInt(_dragStartPosition.x);
            int endX = Mathf.FloorToInt(_currFramePosition.x);
            if (endX < startX)
            {
                int tmp = endX;
                endX = startX;
                startX = tmp;
            }

            int startY = Mathf.FloorToInt(_dragStartPosition.y);
            int endY = Mathf.FloorToInt(_currFramePosition.y);
            if (endY < startY)
            {
                int tmp = endY;
                endY = startY;
                startY = tmp;
            }

            // Clean up old drag previews
            while (_dragPreviewGameObjects.Count > 0)
            {
                var go = _dragPreviewGameObjects[0];
                _dragPreviewGameObjects.RemoveAt(0);
                SimplePool.Despawn(go);
            }

            if (Input.GetMouseButton(0))
            {
                // Display a preview of the drag area
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        var t = WorldController.Instance.World.GetTileAt(x, y);
                        if (t != null)
                        {
                            // Display the building hint on top of this tile position

                            var go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                            go.transform.SetParent(transform, true);
                            _dragPreviewGameObjects.Add(go);
                        }
                    }
                }
            }

            // End Editor Drag
            if (Input.GetMouseButtonUp(0))
            {
                // TODO: Separate for Base/Room generation code from normal gameplay
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        var t = WorldController.Instance.World.GetTileAt(x, y);
                        
                        if (t != null)
                        {
                            if (buildModeIsObjects)
                            {
                                // Create the InstalledObject and assign it to the tile
                                // TODO: We are currently assuming walls only

                                WorldController.Instance.World.PlaceInstalledObject(buildModeObjectType, t);
                            }
                            else
                            {
                                // We are in tile-changing mode
                                t.Type = buildModeTile;
                            }
                            
                        }
                    }
                }
            }
        }


        //private void UpdateCursor()
        //{
        //    // Update tile selection position
        //    Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
        //    if (tileUnderMouse != null)
        //    {
        //        circleCursor.SetActive(true);
        //        Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
        //        circleCursor.transform.position = cursorPosition;
        //    }
        //    else
        //    {
        //        circleCursor.SetActive(false);
        //    }
        //}

        
        
        public void SetMode_BuildFloor()
        {
            buildModeIsObjects = false;
            buildModeTile = TileType.Floor;
        }

        public void SetMode_Bulldoze()
        {
            buildModeIsObjects = false;
            buildModeTile = TileType.Empty;
        }

        public void SetMode_BuildInstalledObject(string objectType)
        {
            buildModeIsObjects = true;
            buildModeObjectType = objectType;
        }
    }
}
