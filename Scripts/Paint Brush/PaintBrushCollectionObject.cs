using System.Collections.Generic;
using UnityEngine;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = PAINT_BRUSH_COLLECTION_OBJECT_NAME, menuName = PAINT_BRUSH_COLLECTION_OBJECT_PATH)]
    public class PaintBrushCollectionObject : ScriptableObject, IPaintBrushProvider
    {
        private const string PAINT_BRUSH_COLLECTION_OBJECT_NAME = "Paint Brush Collection";
        private const string PAINT_BRUSH_COLLECTION_OBJECT_PATH = "Scriptable Objects/" + PAINT_BRUSH_COLLECTION_OBJECT_NAME;
        [field: SerializeField] public PaintBrushObject[] PaintBrushes { get; private set; } = new PaintBrushObject[0];
        [field: SerializeField] public bool UseRandomBrush { get; private set; } = false;
        public PaintBrushObject GetRandomSettings() => PaintBrushes.Length == 0 ? null : PaintBrushes[Random.Range(0, PaintBrushes.Length)];
        [field: SerializeField] public int SelectedPaintBrushesIndex { get; set; } = 0;
        public IEnumerable<IPaintBrush> Brushes => UseRandomBrush ? GetRandomSettings().Brushes : PaintBrushes[SelectedPaintBrushesIndex].Brushes;
    }
}
