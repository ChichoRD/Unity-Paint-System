using UnityEngine;

namespace PaintSystem
{
    public interface ITexturedPaintBrush : IPaintBrush
    {
        Texture2D PaintTexture { get; }
        Vector3 PaintTextureRotation { get; }
        Vector2 PaintTextureTiling { get; }
        Vector2 PaintTextureOffset { get; }
    }
}
