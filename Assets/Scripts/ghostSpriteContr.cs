using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Controllers;
using UnityEngine;

public class ghostSpriteContr : MonoBehaviour
{
    private Dictionary<ghostAI, GameObject> ghostGameObjectMap;
    private Dictionary<string, Sprite> ghostSprites;

    private World world
    {
        get { return WorldController.Instance.World; }
    }


	// Use this for initialization
	void Start ()
	{
	    LoadSprites();

        ghostGameObjectMap = new Dictionary<ghostAI, GameObject>();

	    world.RegisterGhostCreated(onGhostCreated);

        world.createGhost(world.GetTileAt(world.Width/2, world.Height/2));

	}

    private void LoadSprites()
    {
        ghostSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Assets/Sprites/SS13Assets/Icons/mob");
        foreach (Sprite s in sprites)
        {
            //Debug.Log(s);
            ghostSprites[s.name] = s;
        }

    }

    private void onGhostCreated(ghostAI ghost)
    {
        GameObject ghost_go = new GameObject();
        ghost_go.name = "ghost";
        ghost_go.transform.position = new Vector3(ghost.currTile.X,ghost.currTile.Y,0);
        ghost_go.transform.SetParent(this.transform, true);
        ghost_go.AddComponent<SpriteRenderer>().sprite = ghostSprites["ghost_drone_46"];

        //ghost.RegisterOnchangedCallback(onGhostChanged);

    }


    // Update is called once per frame
    void Update () {
		
	}
}
