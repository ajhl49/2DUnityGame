using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticHorizontalSize : MonoBehaviour
{

    public float childWidth = 150f;

	// Use this for initialization
	void Start () {
        AdjustSize();
    }

    public void AdjustSize()
    {
        var size = this.GetComponent<RectTransform>().sizeDelta;
        size.x = this.transform.childCount * childWidth;
        this.GetComponent<RectTransform>().sizeDelta = size;
    }
}
