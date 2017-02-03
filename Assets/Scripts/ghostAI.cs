using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class ghostAI
    {
        public float X
        {
            get { return Mathf.Lerp(currTile.X, destTile.X, MovePercentage); }
        }

        public float Y
        {
            get { return Mathf.Lerp(currTile.X, destTile.X, MovePercentage); }
        }

        private float MovePercentage; // 0-1 how far between tiles
        private float speed = 2f; //tiles/sec

        public Tile currTile { get; protected set; }
        public Tile destTile; // when stopped destTile = currtile

        public ghostAI(Tile tile)
        {
            currTile = destTile = tile;

        }

        public void Update(float deltaTime)
        {
            if (currTile == destTile)
                return;

            float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.Y - destTile.Y, 2));
            float distThisFrame = speed * deltaTime;
            float percThisFrame = distThisFrame / distToTravel;
            MovePercentage += percThisFrame;

            if (MovePercentage >= 1)
            {
                currTile = destTile;
                MovePercentage = 0;
            }

        }

        public void setDestination(Tile tile)
        {
            if (!currTile.isNeighboor(tile, true))
            {
                Debug.Log("tiles not adjcent");
            }
            destTile = tile;
        }
        

    }
}
