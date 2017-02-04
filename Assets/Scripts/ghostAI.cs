using System;
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

        Action<ghostAI> cbGhostChanged;

        public Tile currTile { get; protected set; }
        public Tile destTile { get; protected set; } // when stopped destTile = currtile

        public ghostAI(Tile tile)
        {
            currTile = destTile = tile;

        }

        public void Update(float deltaTime)
        {
            Debug.Log("Ghost update");
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
            if (cbGhostChanged != null)
            {
                cbGhostChanged(this);
            }



        }

        public void SetDestination(Tile tile)
        {
            if (currTile.IsNeighbour(tile, true) == false)
            {
                Debug.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour.");
            }

            destTile = tile;
        }
        public void RegisterOnChangedCallback(Action<ghostAI> cb)
        {
            cbGhostChanged += cb;
        }

        public void UnregisterOnChangedCallback(Action<ghostAI> cb)
        {
            cbGhostChanged -= cb;
        }

    }
}
