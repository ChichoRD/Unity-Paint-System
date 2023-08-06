using System;
using UnityEngine;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace PaintSystem
{
    public class Paintable : MonoBehaviour
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }
        [SerializeField] private ShadowResolution _paintTextureResolution = ShadowResolution._256;
        private Texture2D _paintReadTexture;

        public RenderTexture ColorMaskRT { get; private set; }
        public RenderTexture MetallicSmoothnessMaskRT { get; private set; }
        
        public RenderTexture ColorMaskSupport { get; private set; }
        public RenderTexture MetallicSmoothnessMaskSupport { get; private set; }

        private static readonly int s_colorMaskTextureID = Shader.PropertyToID("_ColorMask");
        private static readonly int s_metallicSmoothnessMaskTextureID = Shader.PropertyToID("_MetallicSmoothnessMask");

        private void Awake()
        {
            int resolution = (int)_paintTextureResolution;
            _paintReadTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);

            RenderTextureDescriptor colorDescriptor = new RenderTextureDescriptor(resolution, resolution, RenderTextureFormat.ARGB32);
            RenderTextureDescriptor metallicSmoothnessDescriptor = new RenderTextureDescriptor(resolution, resolution, RenderTextureFormat.RG16);

            ColorMaskRT = new RenderTexture(colorDescriptor);
            ColorMaskRT.filterMode = FilterMode.Bilinear;
            MetallicSmoothnessMaskRT = new RenderTexture(metallicSmoothnessDescriptor);
            MetallicSmoothnessMaskRT.filterMode = FilterMode.Bilinear;

            ColorMaskSupport = new RenderTexture(colorDescriptor);
            ColorMaskSupport.filterMode = FilterMode.Bilinear;
            MetallicSmoothnessMaskSupport = new RenderTexture(metallicSmoothnessDescriptor);
            MetallicSmoothnessMaskSupport.filterMode = FilterMode.Bilinear;

            Renderer.material.SetTexture(s_colorMaskTextureID, ColorMaskRT);
            Renderer.material.SetTexture(s_metallicSmoothnessMaskTextureID, MetallicSmoothnessMaskRT);
        }

        private void OnDisable()
        {
            ColorMaskRT.Release();
            MetallicSmoothnessMaskRT.Release();

            ColorMaskSupport.Release();
            MetallicSmoothnessMaskSupport.Release();
        }

        public Color32 GetColorAt(Vector2 uv)
        {
            RenderTexture.active = ColorMaskRT;

            Rect rect = new Rect(Mathf.FloorToInt(uv.x * ColorMaskRT.width), Mathf.FloorToInt(uv.y * ColorMaskRT.height), 1, 1);
            _paintReadTexture.ReadPixels(rect, 0, 0, false);

            Color32 color = _paintReadTexture.GetPixel(0, 0);
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