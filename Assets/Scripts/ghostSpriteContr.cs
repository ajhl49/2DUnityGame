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
        Debug.Log("ghost start!!!");
	    LoadSprites();

        ghostGameObjectMap = new Dictionary<ghostAI, GameObject>();

	    world.RegisterGhostCreated(onGhostCreated);

        world.CreateGhost(world.GetTileAt(world.Width/2, world.Height/2));

	}

    private void LoadSprites()
    {
        ghostSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Assets/Sprites/SS13Assets/Icons/mob/");
        //Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites)
        {
            //Debug.Log(s);
            ghostSprites[s.name] = s;
        }

    }

    private void onGhostCreated(ghostAI ghost)
    {
        GameObject char_go = new GameObject();

        // Add our tile/GO pair to the dictionary.
        ghostGameObjectMap.Add(ghost, char_go);

        char_go.name = "Character";
        char_go.transform.position = new Vector3(ghost.currTile.X, ghost.currTile.Y, 0);
        char_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();
        sr.sprite = ghostSprites["ghost_drone_0"];
        sr.sortingLayerName = "Characters";

        //ghost.RegisterOnchangedCallback(onGhostChanged);

    }


    // Update is called once per frame
    void Update () {
		
	}
}
