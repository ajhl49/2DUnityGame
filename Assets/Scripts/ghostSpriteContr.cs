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

        ghostAI g = world.CreateGhost(world.GetTileAt(world.Width/2, world.Height/2));
        g.SetDestination(world.GetTileAt((world.Width/ 2)+2, world.Height / 2));

	}

    private void LoadSprites()
    {
        ghostSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/SS13Assets/icons/mob");
        
        foreach (Sprite s in sprites)
        {
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
        
        sr.sprite = ghostSprites["ghost_drone_38"];
        sr.sortingLayerName = "Characters";

        //ghost.RegisterOnchangedCallback(onGhostChanged);
        ghost.RegisterOnChangedCallback(OnGhostChanged);

    }


    // Update is called once per frame
    void Update () {
		
	}
    void OnGhostChanged(ghostAI c)
    {
        //Debug.Log("OnFurnitureChanged");
        // Make sure the furniture's graphics are correct.

        if (ghostGameObjectMap.ContainsKey(c) == false)
        {
            Debug.LogError("OnGhostChanged -- trying to change visuals for character not in our map.");
            return;
        }

        GameObject char_go = ghostGameObjectMap[c];
        //Debug.Log(furn_go);
        //Debug.Log(furn_go.GetComponent<SpriteRenderer>());

        //char_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

        char_go.transform.position = new Vector3(c.X, c.Y, 0);
    }
}
