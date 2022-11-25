using UnityEngine;

namespace PaintSystem
{
    public class Paintable : MonoBehaviour
    {
        private const int PAINT_TEXTURE_SIZE = 4096;

        public RenderTexture Mask { get; private set; }
        public RenderTexture Support { get; private set; }
        public Renderer Renderer { get; private set; }

        private static readonly int s_maskTextureID = Shader.PropertyToID("_Mask");

        private void Start()
        {
            Mask = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            Mask.filterMode = FilterMode.Bilinear;

            Support = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            Support.filterMode = FilterMode.Bilinear;

            Renderer = GetComponent<Renderer>();
            Renderer.material.SetTexture(s_maskTextureID, Mask);
        }

        private void OnDisable()
        {
            Mask.Release();
            Support.Release();
        }
    }
}