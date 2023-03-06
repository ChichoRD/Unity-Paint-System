using System;
using UnityEngine;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace PaintSystem
{
    public class Paintable : MonoBehaviour
    {
        [SerializeField] private ShadowResolution _paintTextureResolution = ShadowResolution._256;
        private Texture2D _paintReadtexture;

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
            int resolution = (int)_paintTextureResolution;
            _paintReadtexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);

            RenderTextureDescriptor colorDescriptor = new RenderTextureDescriptor(resolution, resolution, RenderTextureFormat.ARGB32);
            RenderTextureDescriptor metallicDescriptor = new RenderTextureDescriptor(resolution, resolution, RenderTextureFormat.R8);
            RenderTextureDescriptor smoothnessDescriptor = new RenderTextureDescriptor(resolution, resolution, RenderTextureFormat.R8);

            ColorSupport = new RenderTexture(colorDescriptor);
            ColorSupport.filterMode = FilterMode.Bilinear;
            _colorMask = new RenderTexture(colorDescriptor);
            _colorMask.filterMode = FilterMode.Bilinear;
            
            MetallicSupport = new RenderTexture(metallicDescriptor);
            MetallicSupport.filterMode = FilterMode.Bilinear;
            _metallicMask = new RenderTexture(metallicDescriptor);
            _metallicMask.filterMode = FilterMode.Bilinear;
            
            SmoothnessSupport = new RenderTexture(smoothnessDescriptor);
            SmoothnessSupport.filterMode = FilterMode.Bilinear;
            _smoothnessMask = new RenderTexture(smoothnessDescriptor);
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

        public Color32 GetColorAt(Vector2 uv)
        {
            RenderTexture.active = _colorMask;

            Rect rect = new Rect(Mathf.FloorToInt(uv.x * _colorMask.width), Mathf.FloorToInt(uv.y * _colorMask.height), 1, 1);
            _paintReadtexture.ReadPixels(rect, 0, 0, false);

            Color32 color = _paintReadtexture.GetPixel(0, 0);
            RenderTexture.active = null;
            return color;
        }

        public bool IsOnColor(Vector2 uv, Color test, float comparisonEpsilon = float.Epsilon * 10e35f)
        {
            Color standingColor = GetColorAt(uv);
            Color difference = standingColor * standingColor.a - test * test.a;
            return (difference.r * difference.r + difference.g * difference.g + difference.b * difference.b) < comparisonEpsilon;
        }

        [Serializable]
        private enum TextureSize
        {
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096,
            _8192 = 8192,
            _16384 = 16384
        }
    }
}