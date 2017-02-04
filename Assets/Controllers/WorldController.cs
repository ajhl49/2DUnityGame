using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; protected set; }

        public World World { get; protected set; }

        // Use this for initialization
        void Start ()
        {

            if (Instance != null)
            {
                Debug.LogError("There should never be two world controllers.");
            }
            Instance = this;

            CreateEmptyWorld();
        }

        private void CreateEmptyWorld()
        {
            World = new World();
        }

        private void Update()
        {
            
        }

        public Tile GetTileAtWorldCoord(Vector3 coord)
        {
            int x = Mathf.FloorToInt(coord.x);
            int y = Mathf.FloorToInt(coord.y);

            return World.GetTileAt(x, y);
        }

        public void NewWorld()
        {
            Debug.Log("Creating new world...");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
