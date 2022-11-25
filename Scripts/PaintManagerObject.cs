using UnityEngine;
using UnityEngine.Rendering;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = "New Paint Manager Object", menuName = "Scriptable Objects/Paint Manager Object")]
    public class PaintManagerObject : ScriptableObject
    {    
        private CommandBuffer _cmd;
        private Material _paintMaterial;
        private bool Initialized => _paintMaterial != null;

        private static readonly int s_painterPositionID = Shader.PropertyToID("_PainterPosition");
        private static readonly int s_radiusID = Shader.PropertyToID("_Radius");
        private static readonly int s_hardnessID = Shader.PropertyToID("_Hardness");
        private static readonly int s_strengthID = Shader.PropertyToID("_Strength");
        private static readonly int s_painterColorID = Shader.PropertyToID("_PainterColor");
        private static readonly int s_textureID = Shader.PropertyToID("_MainTex");

        public void Initialize()
        {
            if (Initialized) return;
        
            _paintMaterial = new Material(Shader.Find("Hidden/Paint Effect"));
        }

        public void Paint(Paintable paintable, Vector3 position, PaintSettingsObject paintSettings)
        {
            Color c = Color.white * paintSettings.PaintColor;
            Paint(paintable, paintable.ColorMask, paintable.ColorSupport, position, paintSettings.BrushRadius, paintSettings.BrushHardness, paintSettings.BrushStrength, c);

            c = Color.white * paintSettings.PaintMetallic;
            c.a = 1;
            Paint(paintable, paintable.MetallicMask, paintable.MetallicSupport, position, paintSettings.BrushRadius, paintSettings.BrushHardness, paintSettings.BrushStrength, c);

            c = Color.white * paintSettings.PaintSmoothness;
            c.a = 1;
            Paint(paintable, paintable.SmoothnessMask, paintable.SmoothnessSupport, position, paintSettings.BrushRadius, paintSettings.BrushHardness, paintSettings.BrushStrength, c);
        }

        private void Paint(Paintable paintable, RenderTexture mask, RenderTexture support, Vector3 position, float radius = 1f, float hardness = .5f, float strength = .5f, Color color = default)
        {
            Renderer renderer = paintable.Renderer;

            _paintMaterial.SetVector(s_painterPositionID, position);
            _paintMaterial.SetFloat(s_radiusID, radius);
            _paintMaterial.SetFloat(s_hardnessID, hardness);
            _paintMaterial.SetFloat(s_strengthID, strength);
            _paintMaterial.SetColor(s_painterColorID, color);
            _paintMaterial.SetTexture(s_textureID, support);

            _cmd = CommandBufferPool.Get();

            {
                _cmd.SetRenderTarget(mask);
                _cmd.DrawRenderer(renderer, _paintMaterial, 0, 0);
                _cmd.SetRenderTarget(support);
                _cmd.Blit(mask, support);

                Graphics.ExecuteCommandBuffer(_cmd);
                _cmd.Clear();
            }

            CommandBufferPool.Release(_cmd);
        }
    }
}
