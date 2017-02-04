using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class ghostAI
    {
        public float X
        {
            get { return Mathf.Lerp(currTile.X, destTile.X, movementPercentage); }
        }

        public float Y
        {
            get { return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage); }
        }

        private float movementPercentage; // 0-1 how far between tiles
        private float speed = 2f; //tiles/sec

        Action<ghostAI> cbGhostChanged;

        public Tile currTile { get; protected set; }
        public Tile destTile { get; protected set; } // when stopped destTile = currtile
        private Tile finTile;
        Path_AStar pathAStar;
        private Tile nextTile;


        public ghostAI(Tile tile)
        {
            currTile = destTile = tile;

        }

        void Update_DoMovement(float deltaTime)
        {
            if (currTile == destTile)
            {
                pathAStar = null;
                return; // We're already were we want to be.
            }

            if (nextTile == null || nextTile == currTile)
            {
                // Get the next tile from the pathfinder.
                if (pathAStar == null || pathAStar.Length() == 0)
                {
                    // Generate a path to our destination
                    pathAStar = new Path_AStar(currTile._world, currTile, destTile); // This will calculate a path from curr to dest.
                    if (pathAStar.Length() == 0)
                    {
                        Debug.LogError("Path_AStar returned no path to destination!");
                       
                        pathAStar = null;
                        return;
                    }
                }

                // Grab the next waypoint from the pathing system!
                nextTile = pathAStar.Dequeue();

                if (nextTile == currTile)
                {
                    Debug.LogError("Update_DoMovement - nextTile is currTile?");
                }
            }

            /*		if(pathAStar.Length() == 1) {
                        return;
                    }
            */
            // At this point we should have a valid nextTile to move to.

            // What's the total distance from point A to point B?
            // We are going to use Euclidean distance FOR NOW...
            // But when we do the pathfinding system, we'll likely
            // switch to something like Manhattan or Chebyshev distance
            float distToTravel = Mathf.Sqrt(
                Mathf.Pow(currTile.X - nextTile.X, 2) +
                Mathf.Pow(currTile.Y - nextTile.Y, 2)
            );


            // How much distance can be travel this Update?
            float distThisFrame = speed * deltaTime;

            // How much is that in terms of percentage to our destination?
            float percThisFrame = distThisFrame / distToTravel;

            // Add that to overall percentage travelled.
            movementPercentage += percThisFrame;

            if (movementPercentage >= 1)
            {
                // We have reached our destination

                // TODO: Get the next tile from the pathfinding system.
                //       If there are no more tiles, then we have TRULY
                //       reached our destination.

                currTile = nextTile;
                movementPercentage = 0;
                // FIXME?  Do we actually want to retain any overshot movement?
            }


        }

        public void Update(float deltaTime)
        {
            //Debug.Log("Character Update");

            
            Update_DoMovement(deltaTime);

            if (cbGhostChanged != null)
                cbGhostChanged(this);

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
