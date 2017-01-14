using UnityEngine;

public class FollowUV : MonoBehaviour
{
    public float Parralax = 2f;

    private void Update()
    {
        var meshRenderer = GetComponent<MeshRenderer>();

        var meshRendererMaterial = meshRenderer.material;

        var offset = meshRendererMaterial.GetTextureOffset("_MainTex");

        offset.x = transform.position.x / transform.localScale.x / Parralax;
        offset.y = transform.position.y / transform.localScale.y / Parralax;

        meshRendererMaterial.SetTextureOffset("_MainTex", offset);
    }
}
