using System;
using UnityEngine;

namespace PaintSystem
{
    [Serializable]
    public struct TexturedPaintBrushData : ITexturedPaintBrush
    {
        public TexturedPaintBrushData(IPaintBrush paintBrushData) : this()
        {
            PaintBrushData = paintBrushData;
        }

        public static readonly TexturedPaintBrushData Default = new TexturedPaintBrushData()
        {
            PaintTexture = null,
            PaintTextureRotation = Vector3.zero,
            PaintTextureScale = Vector2.one,
            PaintTextureOffset = Vector2.zero
        };

        [field: SerializeReference] public IPaintBrush PaintBrushData { get; set; }
        [field: SerializeField] public Texture2D PaintTexture { get; set; }
        [field: SerializeField] public Vector3 PaintTextureRotation { get; set; }
        [field: SerializeField] public Vector2 PaintTextureScale { get; set; }
        [field: SerializeField] public Vector2 PaintTextureOffset { get; set; }

        public readonly float BrushRadius => PaintBrushData.BrushRadius;
        public readonly float BrushHardness => PaintBrushData.BrushHardness;
        public readonly float BrushStrength => PaintBrushData.BrushStrength;
        public readonly Color PaintColor => PaintBrushData.PaintColor;

        public readonly PaintTarget PaintTarget => PaintBrushData.PaintTarget;
    }
}
