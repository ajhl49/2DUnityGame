using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour
{

    public EventSystem EventSystem;
    public GameObject SelectedObject;

    private bool _buttonSelected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    private void Update () {
        if (!(Math.Abs(Input.GetAxisRaw("Vertical")) > float.Epsilon) || _buttonSelected) return;
        EventSystem.SetSelectedGameObject(SelectedObject);
        _buttonSelected = true;
    }

    private void OnDisable()
    {
        _buttonSelected = false;
    }
}
