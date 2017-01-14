using UnityEngine;

public class ScrollUV : MonoBehaviour {

    void Update()
    {
        var meshRenderer = GetComponent<MeshRenderer>();

        var meshRendererMaterial = meshRenderer.material;

        var offset = meshRendererMaterial.GetTextureOffset("_MainTex");

        offset.x += Time.deltaTime / 10f;

        meshRendererMaterial.SetTextureOffset("_MainTex", offset);
    }
}
