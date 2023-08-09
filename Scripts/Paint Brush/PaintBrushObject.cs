using System.Collections.Generic;
using UnityEngine;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = PAINT_BRUSH_OBJECT_NAME, menuName = PAINT_BRUSH_OBJECT_PATH)]
    public class PaintBrushObject : ScriptableObject, IPaintBrushProvider
    {
        private const string PAINT_BRUSH_OBJECT_NAME = "Paint Brush Object";
        private const string PAINT_BRUSH_OBJECT_PATH = "Scriptable Objects/" + PAINT_BRUSH_OBJECT_NAME;

        [field: SerializeReference] public IPaintBrush PaintBrush { get; private set; } = PaintBrushData.Default;
        public IEnumerable<IPaintBrush> Brushes => new IPaintBrush[] { PaintBrush };

        [ContextMenu(nameof(SetPaintBrush))]
        private void SetPaintBrush() => PaintBrush = PaintBrushData.Default;

        [ContextMenu(nameof(SetPBRBrush))]
        private void SetPBRBrush() => PaintBrush = PBRPaintBrushData.Default;

        [ContextMenu(nameof(SetTexturedBrush))]
        private void SetTexturedBrush() => PaintBrush = new TexturedPaintBrushData(PaintBrush);
    }
}
