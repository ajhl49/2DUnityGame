using UnityEngine;

public class AutomaticHorizontalSize : MonoBehaviour
{

    public float ChildWidth = 150f;

	// Use this for initialization
	private void Start () {
        AdjustSize();
    }

    public void AdjustSize()
    {
        var size = GetComponent<RectTransform>().sizeDelta;
        size.x = transform.childCount * ChildWidth;
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
