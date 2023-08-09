using System.Collections.Generic;
using UnityEngine;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = PAINT_BRUSH_OBJECT_NAME, menuName = PAINT_BRUSH_OBJECT_PATH)]
    public class MultichannelPaintBrushObject : ScriptableObject, IPaintBrushProvider
    {
        private const string PAINT_BRUSH_OBJECT_NAME = "Multichannel Paint Brush Object";
        private const string PAINT_BRUSH_OBJECT_PATH = "Scriptable Objects/" + PAINT_BRUSH_OBJECT_NAME;

        [field: SerializeReference]
        public List<IPaintBrush> PaintBrushes { get; private set; } = new List<IPaintBrush>()
        {
            PaintBrushData.Default,
            PBRPaintBrushData.Default,
        };

        public IEnumerable<IPaintBrush> Brushes => PaintBrushes;

        [ContextMenu(nameof(AddPaintBrush))]
        private void AddPaintBrush() => PaintBrushes.Add(PaintBrushData.Default);

        [ContextMenu(nameof(AddPBRBrush))]
        private void AddPBRBrush() => PaintBrushes.Add(PBRPaintBrushData.Default);

        [ContextMenu(nameof(AddTexturedPaintBrush))]
        private void AddTexturedPaintBrush() => PaintBrushes.Add(new TexturedPaintBrushData(PaintBrushData.Default));

        [ContextMenu(nameof(AddTexturedPBRBrush))]
        private void AddTexturedPBRBrush() => PaintBrushes.Add(new TexturedPaintBrushData(PBRPaintBrushData.Default));
    }
}
