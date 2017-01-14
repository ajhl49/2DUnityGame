using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour {

    void Update()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        Material mat = mr.material;

        Vector2 offset = mat.GetTextureOffset("_MainTex");

        offset.x += Time.deltaTime / 10f;

        mat.SetTextureOffset("_MainTex", offset);
    }
}
