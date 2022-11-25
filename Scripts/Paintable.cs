using UnityEngine;

namespace PaintSystem
{
    public class Paintable : MonoBehaviour
    {
        private const int PAINT_TEXTURE_SIZE = 4096;

        private RenderTexture _mask;
        public RenderTexture Mask
        {
            get => _mask;
            
            set
            {
                _mask = value;
                Renderer.material.SetTexture(s_maskTextureID, _mask);
            }
        }
        
        public RenderTexture Support { get; private set; }
        public Renderer Renderer { get; private set; }

        private static readonly int s_maskTextureID = Shader.PropertyToID("_Mask");

        private void Awake()
        {
            Support = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            Support.filterMode = FilterMode.Bilinear;

            _mask = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            _mask.filterMode = FilterMode.Bilinear;

            Renderer = GetComponent<Renderer>();
            Renderer.material.SetTexture(s_maskTextureID, _mask);
        }

        private void OnDisable()
        {
            Mask.Release();
            Support.Release();
        }
    }
}