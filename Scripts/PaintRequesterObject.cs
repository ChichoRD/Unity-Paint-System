using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = PAINT_REQUESTER_OBJECT_NAME, menuName = PAINT_REQUESTER_OBJECT_PATH)]
    public class PaintRequesterObject : ScriptableObject
    {
        private const string PAINT_REQUESTER_OBJECT_NAME = "Paint Requester Object";
        private const string PAINT_REQUESTER_OBJECT_PATH = "Scriptable Objects/" + PAINT_REQUESTER_OBJECT_NAME;

        private const string PAINT_EFFECT_SHADER_PATH = "Hidden/Paint Composite";
        private Material _paintMaterial;
        private ProfilingSampler _profilingSampler;
        private bool Initialized => _paintMaterial != null;

        private static readonly int s_painterPositionID = Shader.PropertyToID("_PainterPosition");
        private static readonly int s_radiusID = Shader.PropertyToID("_Radius");
        private static readonly int s_hardnessID = Shader.PropertyToID("_Hardness");
        private static readonly int s_strengthID = Shader.PropertyToID("_Strength");
        private static readonly int s_painterColorID = Shader.PropertyToID("_PainterColor");
        private static readonly int s_maskTextureID = Shader.PropertyToID("_MaskTex");

        private static readonly int s_paintTextureID = Shader.PropertyToID("_PaintTex");
        private static readonly int s_paintTextureRotationID = Shader.PropertyToID("_PaintTexRotation");
        private static readonly int s_paintTextureTilingID = Shader.PropertyToID("_PaintTexTiling");
        private static readonly int s_paintTextureOffsetID = Shader.PropertyToID("_PaintTexOffset");


        public void Initialize()
        {
            if (Initialized) return;

            Shader paintEffectShader = Shader.Find(PAINT_EFFECT_SHADER_PATH);
            _paintMaterial = new Material(paintEffectShader);

            _profilingSampler = new ProfilingSampler($"{nameof(PaintRequesterObject)}: {name}");
        }

        public void InitializePaintable(Paintable paintable)
        {
            if (!Initialized) 
                Initialize();

            var strength0Brush = new PaintBrushData()
            {
                BrushStrength = 0.0f,
            };

            Paint(paintable, Vector3.zero, strength0Brush);
            Paint(paintable, Vector3.zero, new PBRPaintBrushData(strength0Brush));
        }

        public void Paint(Paintable paintable, Vector3 position, IPaintBrushProvider paintBrushProvider)
        {
            foreach (var paintBrush in paintBrushProvider.Brushes)
                Paint(paintable, position, paintBrush);
        }

        private void Paint(Paintable paintable, Vector3 position, IPaintBrush paintBrush)
        {
            Renderer renderer = paintable.Renderer;
            (RenderTexture mask, RenderTexture support) = paintBrush.PaintTarget switch
            {
                PaintTarget.Albedo => (paintable.ColorMaskRT, paintable.ColorMaskSupport),
                PaintTarget.MetallicSmoothness => (paintable.MetallicSmoothnessMaskRT, paintable.MetallicSmoothnessMaskSupport),
                _ => throw new NotImplementedException(),
            };

            _paintMaterial.SetVector(s_painterPositionID, position);
            _paintMaterial.SetFloat(s_radiusID, paintBrush.BrushRadius);
            _paintMaterial.SetFloat(s_hardnessID, paintBrush.BrushHardness);
            _paintMaterial.SetFloat(s_strengthID, paintBrush.BrushStrength);
            _paintMaterial.SetColor(s_painterColorID, paintBrush.PaintColor);
            _paintMaterial.SetTexture(s_maskTextureID, support);

            if (paintBrush is ITexturedPaintBrush texturedPaintBrush)
            {
                _paintMaterial.SetTexture(s_paintTextureID, texturedPaintBrush.PaintTexture);
                _paintMaterial.SetVector(s_paintTextureRotationID, texturedPaintBrush.PaintTextureRotation);
                _paintMaterial.SetVector(s_paintTextureTilingID, texturedPaintBrush.PaintTextureTiling);
                _paintMaterial.SetVector(s_paintTextureOffsetID, texturedPaintBrush.PaintTextureOffset);
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            Graphics.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.SetRenderTarget(mask);
                cmd.DrawRenderer(renderer, _paintMaterial, 0, paintBrush is ITexturedPaintBrush ? 1 : 0);

                cmd.Blit(mask, support);
            }

            Graphics.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            cmd.Clear();
        }
    }
}
