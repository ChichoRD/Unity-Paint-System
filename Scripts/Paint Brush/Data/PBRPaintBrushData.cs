using System;
using UnityEngine;

namespace PaintSystem
{
    [Serializable]
    public struct PBRPaintBrushData : IPaintBrush
    {
        public PBRPaintBrushData(PaintBrushData paintBrushData) : this()
        {
            PaintBrushData = paintBrushData;
        }

        public static readonly PBRPaintBrushData Default = new PBRPaintBrushData()
        {
            PaintBrushData = PaintBrushData.Default,
            PaintMetallic = 0.5f,
            PaintSmoothness = 0.5f
        };

        [field: SerializeField] public PaintBrushData PaintBrushData { get; set; }
        [field: SerializeField][field: Range(0f, 1f)] public float PaintMetallic { get; set; }
        [field: SerializeField][field: Range(0f, 1f)] public float PaintSmoothness { get; set; }

        public readonly float BrushRadius => PaintBrushData.BrushRadius;
        public readonly float BrushHardness => PaintBrushData.BrushHardness;
        public readonly float BrushStrength => PaintBrushData.BrushStrength;

        public readonly Color PaintColor => new Color(PaintMetallic, PaintSmoothness, 0.0f, 1.0f);

        public readonly PaintTarget PaintTarget => PaintTarget.MetallicSmoothness;
    }

}
