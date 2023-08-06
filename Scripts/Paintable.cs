using System;
using UnityEngine;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace PaintSystem
{
    public class Paintable : MonoBehaviour
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }
        [SerializeField] private ShadowResolution _paintTextureResolution = ShadowResolution._256;
        private Texture2D _paintReadtexture;

        public RenderTexture ColorMask { get; private set; }
        public RenderTexture MetallicMask { get; private set; }
        public RenderTexture SmoothnessMask { get; private set; }

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

            ColorMask = new RenderTexture(colorDescriptor);
            ColorMask.filterMode = FilterMode.Bilinear;
            
            MetallicMask = new RenderTexture(metallicDescriptor);
            MetallicMask.filterMode = FilterMode.Bilinear;
            
            SmoothnessMask = new RenderTexture(smoothnessDescriptor);
            SmoothnessMask.filterMode = FilterMode.Bilinear;

            Renderer.material.SetTexture(s_colorMaskTextureID, ColorMask);
            Renderer.material.SetTexture(s_metallicMaskTextureID, MetallicMask);
            Renderer.material.SetTexture(s_smoothnessMaskTextureID, SmoothnessMask);
        }

        private void OnDisable()
        {
            ColorMask.Release();
            MetallicMask.Release();
            SmoothnessMask.Release();
        }

        public Color32 GetColorAt(Vector2 uv)
        {
            RenderTexture.active = ColorMask;

            Rect rect = new Rect(Mathf.FloorToInt(uv.x * ColorMask.width), Mathf.FloorToInt(uv.y * ColorMask.height), 1, 1);
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