using UnityEngine;

namespace PaintSystem
{
    public class Paintable : MonoBehaviour
    {
        private const int PAINT_TEXTURE_SIZE = 4096;

        private RenderTexture _colorMask;
        public RenderTexture ColorMask
        {
            get => _colorMask;

            set
            {
                _colorMask = value;
                ColorSupport = new RenderTexture(_colorMask);
                Renderer.material.SetTexture(s_colorMaskTextureID, _colorMask);
            }
        }

        private RenderTexture _metallicMask;
        public RenderTexture MetallicMask
        {
            get => _metallicMask;

            set
            {
                _metallicMask = value;
                MetallicSupport = new RenderTexture(_metallicMask);
                Renderer.material.SetTexture(s_metallicMaskTextureID, _metallicMask);
            }
        }
        
        private RenderTexture _smoothnessMask;
        public RenderTexture SmoothnessMask
        {
            get => _smoothnessMask;

            set
            {
                _smoothnessMask = value;
                SmoothnessSupport = new RenderTexture(_smoothnessMask);
                Renderer.material.SetTexture(s_smoothnessMaskTextureID, _smoothnessMask);
            }
        }

        public RenderTexture ColorSupport { get; private set; }
        public RenderTexture MetallicSupport { get; private set; }
        public RenderTexture SmoothnessSupport { get; private set; }
        public Renderer Renderer { get; private set; }

        private static readonly int s_colorMaskTextureID = Shader.PropertyToID("_Mask");
        private static readonly int s_metallicMaskTextureID = Shader.PropertyToID("_MetallicMask");
        private static readonly int s_smoothnessMaskTextureID = Shader.PropertyToID("_SmoothnessMask");

        private void Awake()
        {
            ColorSupport = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            ColorSupport.filterMode = FilterMode.Bilinear;
            
            MetallicSupport = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            MetallicSupport.filterMode = FilterMode.Bilinear;
            
            SmoothnessSupport = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            SmoothnessSupport.filterMode = FilterMode.Bilinear;

            _colorMask = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            _colorMask.filterMode = FilterMode.Bilinear;

            _metallicMask = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            _metallicMask.filterMode = FilterMode.Bilinear;

            _smoothnessMask = new RenderTexture(PAINT_TEXTURE_SIZE, PAINT_TEXTURE_SIZE, 0);
            _smoothnessMask.filterMode = FilterMode.Bilinear;

            Renderer = GetComponent<Renderer>();
            Renderer.material.SetTexture(s_colorMaskTextureID, _colorMask);
            Renderer.material.SetTexture(s_metallicMaskTextureID, _metallicMask);
            Renderer.material.SetTexture(s_smoothnessMaskTextureID, _smoothnessMask);
        }

        private void OnDisable()
        {
            ColorMask.Release();
            MetallicMask.Release();
            SmoothnessMask.Release();

            ColorSupport.Release();
            MetallicSupport.Release();
            SmoothnessSupport.Release();
        }
    }
}