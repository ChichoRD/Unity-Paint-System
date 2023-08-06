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

        private static readonly int s_paintTextureID = Shader.PropertyToID("_PaintTex");
        private static readonly int s_paintTextureRotationID = Shader.PropertyToID("_PaintTexRotation");
        private static readonly int s_paintTextureScaleID = Shader.PropertyToID("_PaintTexScale");
        private static readonly int s_paintTextureOffsetID = Shader.PropertyToID("_PaintTexOffset");

        private static LocalKeyword s_alphaToRedID;

        public void Initialize()
        {
            if (Initialized) return;

            Shader paintEffectShader = Shader.Find("Hidden/Paint Effect");
            _paintMaterial = new Material(paintEffectShader);

            s_alphaToRedID = new LocalKeyword(paintEffectShader, "ALPHA_TO_RED");
        }

        private void PaintAllParameters(Paintable paintable, Vector3 position, PaintSettingsObject paintSettings, Vector3 paintRotation, Texture2D paintTexture)
        {
            Color c = Color.white * paintSettings.PaintColor;
            PaintAlbedo(paintable, position, paintSettings, paintRotation, paintTexture, c);

            c = Color.white * paintSettings.PaintMetallic;
            c.a = 1;
            PaintMetallic(paintable, position, paintSettings, paintRotation, paintTexture, c);

            c = Color.white * paintSettings.PaintSmoothness;
            c.a = 1;
            PaintSmoothness(paintable, position, paintSettings, paintRotation, paintTexture, c);
        }

        private void PaintSmoothness(Paintable paintable, Vector3 position, PaintSettingsObject paintSettings, Vector3 paintRotation, Texture2D paintTexture, Color c)
        {
            _paintMaterial.SetKeyword(s_alphaToRedID, true);
            Paint(paintable, paintable.SmoothnessMask, paintable.SmoothnessSupport, position, paintSettings.BrushRadius, paintSettings.BrushHardness, paintSettings.BrushStrength, c, paintTexture, paintRotation, paintSettings.PaintTextureScale, paintSettings.PaintTextureOffset);
        }

        private void PaintMetallic(Paintable paintable, Vector3 position, PaintSettingsObject paintSettings, Vector3 paintRotation, Texture2D paintTexture, Color c)
        {
            _paintMaterial.SetKeyword(s_alphaToRedID, true);
            Paint(paintable, paintable.MetallicMask, paintable.MetallicSupport, position, paintSettings.BrushRadius, paintSettings.BrushHardness, paintSettings.BrushStrength, c, paintTexture, paintRotation, paintSettings.PaintTextureScale, paintSettings.PaintTextureOffset);
        }

        private void PaintAlbedo(Paintable paintable, Vector3 position, PaintSettingsObject paintSettings, Vector3 paintRotation, Texture2D paintTexture, Color c)
        {
            _paintMaterial.SetKeyword(s_alphaToRedID, false);
            Paint(paintable, paintable.ColorMask, paintable.ColorSupport, position, paintSettings.BrushRadius, paintSettings.BrushHardness, paintSettings.BrushStrength, c, paintTexture, paintRotation, paintSettings.PaintTextureScale, paintSettings.PaintTextureOffset);
        }

        public void Paint(Paintable paintable, Vector3 position, PaintSettingsCollectionObject paintSettingsCollection)
        {
            PaintSettingsObject paintSettings = paintSettingsCollection.UseRandomSettings ? paintSettingsCollection.GetRandomSettings() : paintSettingsCollection.PaintSettings[paintSettingsCollection.SelectedPaintSettingsIndex];
            Vector3 paintRotation = paintSettingsCollection.UseRandomRotation ? paintSettingsCollection.GetRandomRotation() : paintSettings.PaintTextureRotation;
            Texture2D paintTexture = paintSettingsCollection.UseRandomTexture ? paintSettingsCollection.GetRandomTextureFromSettings() : paintSettings.PaintTexture;

            PaintAllParameters(paintable, position, paintSettings, paintRotation, paintTexture);
        }

        public void Paint(Paintable paintable, Vector3 position, PaintSettingsObject paintSettings)
        {
            PaintAllParameters(paintable, position, paintSettings, paintSettings.PaintTextureRotation, paintSettings.PaintTexture);
        }

        private void Paint(Paintable paintable, RenderTexture mask, RenderTexture support, Vector3 position, float radius = 1f, float hardness = .5f, float strength = .5f, Color color = default, Texture2D paintTexture = null, Vector3 paintTextureRotation = default, Vector2 paintTextureScale = default, Vector2 paintTextureOffset = default)
        {
            Renderer renderer = paintable.Renderer;

            _paintMaterial.SetVector(s_painterPositionID, position);
            _paintMaterial.SetFloat(s_radiusID, radius);
            _paintMaterial.SetFloat(s_hardnessID, hardness);
            _paintMaterial.SetFloat(s_strengthID, strength);
            _paintMaterial.SetColor(s_painterColorID, color);
            _paintMaterial.SetTexture(s_textureID, support);

            _paintMaterial.SetTexture(s_paintTextureID, paintTexture);
            _paintMaterial.SetVector(s_paintTextureRotationID, paintTextureRotation);
            _paintMaterial.SetVector(s_paintTextureScaleID, paintTextureScale);
            _paintMaterial.SetVector(s_paintTextureOffsetID, paintTextureOffset);

            _cmd = CommandBufferPool.Get();

            {
                _cmd.SetRenderTarget(mask);
                _cmd.DrawRenderer(renderer, _paintMaterial, 0, paintTexture == null ? 0 : 1);
                _cmd.SetRenderTarget(support);
                _cmd.Blit(mask, support);

                Graphics.ExecuteCommandBuffer(_cmd);
                _cmd.Clear();
            }

            CommandBufferPool.Release(_cmd);
        }
    }
}
