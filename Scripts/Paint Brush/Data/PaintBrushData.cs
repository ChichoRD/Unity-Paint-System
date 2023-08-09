using System;
using UnityEngine;

namespace PaintSystem
{
    [Serializable]
    public struct PaintBrushData : IPaintBrush
    {
        public static readonly PaintBrushData Default = new PaintBrushData()
        {
            BrushRadius = 1f,
            BrushHardness = 0.5f,
            BrushStrength = 0.5f,
            PaintColor = Color.white
        };

        [field: SerializeField][field: Min(0)] public float BrushRadius { get; set; }
        [field: SerializeField][field: Range(0f, 1f)] public float BrushHardness { get; set; }
        [field: SerializeField][field: Range(0f, 1f)] public float BrushStrength { get;  set; }
        [field: SerializeField] public Color PaintColor { get; set; }

        public readonly PaintTarget PaintTarget => PaintTarget.Albedo;
    }

}
